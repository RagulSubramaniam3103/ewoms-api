using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ClassLibrary.DataIntegration.ChatMessenger;
using EWOMS_ClassLibrary.Services;
using EWOMS_ExternalClassLibrary_DTO.UserData_DTO.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EWOMS_CoreAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatUserController : ControllerBase
    {
        private readonly UserManager<MasterUser> _userManager;
        private readonly ApplicationDbContext _connection;
        private readonly SignInManager<MasterUser> _signInManager;
        private readonly UserConnectionManager _online;

        public ChatUserController(UserManager<MasterUser> userManager, 
            ApplicationDbContext connection, 
            SignInManager<MasterUser> signInManager,
            UserConnectionManager online)
        {
            _userManager = userManager;
            _connection = connection;
            _signInManager = signInManager;
            _online = online;
        }
        [HttpGet("SearchUser")]
        public IActionResult SearchUsers(string key)
        {
            if (string.IsNullOrEmpty(key)) return Ok(new List<object>());

            var users = _connection.Users
                .Where(x => x.UserName.Contains(key) || x.FullName.Contains(key) || x.Id == key)
                .Select(x => new
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    ProfileImage = x.ProfileImage
                }).ToList();
            return Ok(users);
        }

        [HttpGet("GetSuggestedUsers")]
        [Authorize]
        public IActionResult GetSuggestedUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // 1. Get IDs of users you already have a relationship with (Accepted or Pending)
            var existingConnectionIds = _connection.EWOMS_FriendRequests
                .Where(fr => fr.SenderId == userId || fr.ReceiverId == userId)
                .Select(fr => fr.SenderId == userId ? fr.ReceiverId : fr.SenderId)
                .Distinct()
                .ToList();

            // Add self to exclusion list
            existingConnectionIds.Add(userId);

            // 2. Fetch users NOT in that list
            var suggested = _connection.Users
                .Where(u => !existingConnectionIds.Contains(u.Id))
                .OrderByDescending(u => u.Id) // Simple heuristic for 'new users'
                .Take(5)
                .Select(u => new
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    ProfileImage = u.ProfileImage,
                    Role = "Personnel" // Default role
                })
                .ToList();

            return Ok(suggested);
        }

        [Authorize]
        [HttpPost("SendFriendRequest")]
        public IActionResult SendFriendRequest([FromBody] FriendRequestSendDTO dto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (senderId == dto.ReceiverId)
                return BadRequest("You cannot send request to yourself");

            // check receiver user
            var receiver = _connection.Users
                .FirstOrDefault(x => x.Id == dto.ReceiverId);

            if (receiver == null)
                return NotFound("User not found");

            // already exists check
            var exists = _connection.EWOMS_FriendRequests.Any(x =>
                (x.SenderId == senderId && x.ReceiverId == dto.ReceiverId) ||
                (x.SenderId == dto.ReceiverId && x.ReceiverId == senderId));

            if (exists)
                return BadRequest(new { message = "Request already exists or friend already added" });

            // ✅ CASE 1: PUBLIC PROFILE → AUTO ACCEPT
            if (receiver.IsPrivate == false)
            {
                var autoFriend = new FriendRequests
                {
                    SenderId = senderId,
                    ReceiverId = dto.ReceiverId,
                    Status = FriendRequestStatus.Accepted,
                    RequestDate = DateTime.Now
                };

                _connection.EWOMS_FriendRequests.Add(autoFriend);
                
                // ✅ SYNC: Auto-Follow each other
                SyncMutualFollow(senderId!, dto.ReceiverId!);
                
                _connection.SaveChanges();

                return Ok(new { message = "Successfully added as friend (Public Account)" });
            }

            // 🔒 CASE 2: PRIVATE PROFILE → NEED APPROVAL
            var request = new FriendRequests
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Status = FriendRequestStatus.Pending,
                RequestDate = DateTime.Now
            };

            _connection.EWOMS_FriendRequests.Add(request);
            _connection.SaveChanges();

            return Ok(new { message = "Friend request sent for approval (Private Account)" });
        }

        [Authorize]
        [HttpPost("AcceptFriendRequest/{requestId}")]
        public IActionResult AcceptFriendRequest(int requestId)
        {
            if (requestId <= 0) return BadRequest("Invalid request ID");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var request = _connection.EWOMS_FriendRequests.FirstOrDefault(x => x.Id == requestId);

            if (request == null)
                return NotFound("Request not found");

            // Security Check: Only the receiver can accept the request
            if (request.ReceiverId != userId)
                return Unauthorized("You are not authorized to accept this request");

            request.Status = FriendRequestStatus.Accepted;

            // ✅ SYNC: Auto-Follow each other
            SyncMutualFollow(request.SenderId!, request.ReceiverId!);

            _connection.SaveChanges();

            return Ok(new { Message = "Friend request accepted", RequestId = requestId });
        }

        [Authorize]
        [HttpPost("DeclineFriendRequest/{requestId}")]
        public IActionResult DeclineFriendRequest(int requestId)
        {
            if (requestId <= 0) return BadRequest("Invalid request ID");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var request = _connection.EWOMS_FriendRequests.FirstOrDefault(x => x.Id == requestId);

            if (request == null)
                return NotFound("Request not found");

            // Security Check: Only the receiver can decline the request
            if (request.ReceiverId != userId)
                return Unauthorized("You are not authorized to decline this request");

            _connection.EWOMS_FriendRequests.Remove(request);
            _connection.SaveChanges();

            return Ok(new { Message = "Friend request declined", RequestId = requestId });
        }

        private void SyncMutualFollow(string userA, string userB)
        {
            try {
                // User A follows B
                if (!_connection.EWOMS_Followers.Any(f => f.FollowerId == userA && f.FollowingId == userB))
                {
                    _connection.EWOMS_Followers.Add(new UserFollower { FollowerId = userA, FollowingId = userB, FollowedAt = DateTime.Now });
                }
                // User B follows A
                if (!_connection.EWOMS_Followers.Any(f => f.FollowerId == userB && f.FollowingId == userA))
                {
                    _connection.EWOMS_Followers.Add(new UserFollower { FollowerId = userB, FollowingId = userA, FollowedAt = DateTime.Now });
                }
            } catch(Exception ex) {
                Console.WriteLine($"Social Sync Error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("GetFriendRequests")]
        public IActionResult GetFriendRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = _connection.EWOMS_FriendRequests
            .Where(x => x.ReceiverId == userId && x.Status == FriendRequestStatus.Pending)
            .Select(x => new
            {
                x.Id,
                x.SenderId,
                x.ReceiverId,
                x.RequestDate,
                x.Status,

                Sender = _connection.Users
                    .Where(u => u.Id == x.SenderId)
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.FullName,
                        u.ProfileImage
                    })
                    .FirstOrDefault()
            })
            .ToList();

            return Ok(requests);
        }

        [Authorize]
        [HttpGet("GetChatUsers")]
        public IActionResult GetChatUsers()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("GetChatUsers: No userId found in claims");
                    return Unauthorized("No User ID found in token.");
                }

                Console.WriteLine($"GetChatUsers: Loading friends for user: {userId}");

                // STEP 1: Get accepted friends
                // Use explicit int comparison to avoid EF Core enum mapping pitfalls
                int acceptedStatus = (int)FriendRequestStatus.Accepted;
                var friendIds = _connection.EWOMS_FriendRequests
                    .Where(fr =>
                        (fr.SenderId == userId || fr.ReceiverId == userId)
                        && (int)fr.Status == acceptedStatus)
                    .Select(fr => fr.SenderId == userId ? fr.ReceiverId : fr.SenderId)
                    .Where(id => id != null) // Safety check for null IDs
                    .Distinct()
                    .ToList();

                Console.WriteLine($"GetChatUsers: Found {friendIds.Count} friends");

                if (!friendIds.Any())
                {
                    return Ok(new List<object>());
                }

                // STEP 2: Get users in ONE query
                var users = _connection.Users
                    .Where(u => friendIds.Contains(u.Id))
                    .ToList();

                Console.WriteLine($"Users fetched: {users.Count}");

                // STEP 3: Get last messages
                var messages = _connection.EWOMS_ChatMessages
                    .Where(m =>
                        (m.SenderId == userId && friendIds.Contains(m.ReceiverId)) ||
                        (friendIds.Contains(m.SenderId) && m.ReceiverId == userId))
                    .ToList();

                // STEP 4: Build result
                var result = users.Select(user =>
                {
                    var conversationMessages = messages
                        .Where(m =>
                            (m.SenderId == userId && m.ReceiverId == user.Id) ||
                            (m.SenderId == user.Id && m.ReceiverId == userId))
                        .ToList();

                    var lastMsg = conversationMessages
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault();

                    var unreadCount = conversationMessages
                        .Count(m => m.ReceiverId == userId && !m.IsRead);

                    return new
                    {
                        UserId = user.Id,
                        FullName = string.IsNullOrEmpty(user.FullName) ? "Unknown User" : user.FullName,
                        LastMessage = lastMsg?.Message,
                        LastMessageDate = lastMsg?.SentAt,
                        ProfileImage = user.ProfileImage,
                        UnreadCount = unreadCount
                    };
                })
                .OrderByDescending(x => x.LastMessageDate ?? DateTime.MinValue)
                .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetChatUsers: CRITICAL ERROR: {ex.Message}");
                // Log full exception to help debugging
                return StatusCode(500, new { 
                    error = "Internal Server Error during GetChatUsers", 
                    message = ex.Message, 
                    inner = ex.InnerException?.Message 
                });
            }
        }
        [Authorize]
        [HttpGet("GetTotalUnreadCount")]
        public IActionResult GetTotalUnreadCount()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var count = _connection.EWOMS_ChatMessages
                    .Count(m => m.ReceiverId == userId && !m.IsRead);

                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("GetFriends")]
        public IActionResult GetFriends()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var friendIds = _connection.EWOMS_FriendRequests
                .Where(x =>
                    (x.SenderId == userId || x.ReceiverId == userId)
                    && x.Status == FriendRequestStatus.Accepted)
                .Select(x => x.SenderId == userId ? x.ReceiverId : x.SenderId)
                .Distinct()
                .ToList();

            var users = _connection.Users
                         .Where(u => friendIds.Contains(u.Id))
                         .Select(u => new
                         {
                             u.Id,
                             u.UserName,
                             u.FullName,
                             ProfileImage = u.ProfileImage
                         })
                         .ToList();

            return Ok(users);
        }


        [Authorize]
        [HttpPost("SendMessage")]
        public IActionResult SendMessage([FromBody] SendMessageDTO dto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(dto.ReceiverId) || 
                (string.IsNullOrEmpty(dto.Message) && 
                 string.IsNullOrEmpty(dto.Image) && 
                 string.IsNullOrEmpty(dto.Video) && 
                 string.IsNullOrEmpty(dto.Document)))
                return BadRequest("Invalid message");

            var msg = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Message = dto.Message,
                Image = dto.Image,
                Video = dto.Video,
                Document = dto.Document,
                FileName = dto.FileName,
                SentAt = DateTime.Now,
                IsRead = false,
                IsDelivered = false
            };

            _connection.EWOMS_ChatMessages.Add(msg);
            _connection.SaveChanges();

            return Ok(msg);
        }
        [Authorize]
        [HttpGet("GetMessages/{friendId}")]
        public IActionResult GetMessages(string friendId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = _connection.EWOMS_ChatMessages
                .Where(x =>
                    (x.SenderId == userId && x.ReceiverId == friendId) ||
                    (x.SenderId == friendId && x.ReceiverId == userId))
                .OrderBy(x => x.SentAt)
                .ToList();

            var unread = messages
                .Where(x => x.ReceiverId == userId && !x.IsRead)
                .ToList();

            foreach (var msg in unread)
            {
                msg.IsRead = true;
                msg.ReadAt = DateTime.Now;
            }

            _connection.SaveChanges();

            return Ok(messages);
        }

        [HttpGet("HealthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "Healthy",
                serviceRegistered = _online != null,
                timestamp = DateTime.Now
            });
        }

        [Authorize]
        [HttpGet("GetUserInfo/{userId}")]
        public IActionResult GetUserInfo(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return BadRequest("User ID is required");

            var user = _connection.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                ProfileImage = user.ProfileImage,
                IsPrivate = user.IsPrivate
            });
        }

        [Authorize]
        [HttpGet("GetAboutInfo/{userId}")]
        public IActionResult GetAboutInfo(string userId)
        {
            // Try to find in MasterAdmin
            var admin = _connection.Set<MasterAdmin>().FirstOrDefault(u => (u.UserId.ToString()) == userId);
            if (admin != null)
            {
                return Ok(new {
                    PhoneNumber = admin.PhoneNumber,
                    Address = $"{admin.Address1} {admin.Address2}".Trim(),
                    City = admin.City, State = admin.State, Country = admin.Country,
                    PostalCode = admin.PostalCode, Role = "Administrator"
                });
            }

            // Try to find in MasterManager
            var manager = _connection.Set<MasterManager>().FirstOrDefault(u => u.UserId.ToString() == userId);
            if (manager != null)
            {
                return Ok(new {
                    PhoneNumber = manager.PhoneNumber,
                    Address = $"{manager.Address1} {manager.Address2}".Trim(),
                    City = manager.City, State = manager.State, Country = manager.Country,
                    PostalCode = manager.PostalCode, Role = "Manager"
                });
            }

            // Try to find in MasterUserDetails
            var userDetail = _connection.Set<MasterUserDetails>().FirstOrDefault(u => u.UserId.ToString() == userId);
            if (userDetail != null)
            {
                return Ok(new {
                    PhoneNumber = userDetail.PhoneNumber,
                    Address = $"{userDetail.Address1} {userDetail.Address2}".Trim(),
                    City = userDetail.City, State = userDetail.State, Country = userDetail.Country,
                    PostalCode = userDetail.PostalCode, Role = "Team Member"
                });
            }

            return NotFound("User profile details not found");
        }

        [HttpGet("IsUserOnline/{userId}")]
        public IActionResult IsUserOnline(string userId)
        {
            return Ok(new
            {
                userId,
                isOnline = _online.IsOnline(userId)
            });
        }

        // =========================
        // SOCIAL FOLLOWER SYSTEM
        // =========================

        [Authorize]
        [HttpPost("ToggleFollow/{targetUserId}")]
        public IActionResult ToggleFollow(string targetUserId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == targetUserId)
                return BadRequest("You cannot follow yourself");

            var targetUser = _connection.Users.Any(u => u.Id == targetUserId);
            if (!targetUser) return NotFound("Target user not found");

            var existing = _connection.EWOMS_Followers
                .FirstOrDefault(f => f.FollowerId == currentUserId && f.FollowingId == targetUserId);

            if (existing != null)
            {
                // UNFOLLOW
                _connection.EWOMS_Followers.Remove(existing);
                _connection.SaveChanges();
                return Ok(new { status = "unfollowed", message = "You have unfollowed this user" });
            }
            else
            {
                // FOLLOW
                var follow = new UserFollower
                {
                    FollowerId = currentUserId,
                    FollowingId = targetUserId,
                    FollowedAt = DateTime.Now
                };
                _connection.EWOMS_Followers.Add(follow);
                _connection.SaveChanges();
                return Ok(new { status = "followed", message = "You are now following this user" });
            }
        }

        [Authorize]
        [HttpGet("GetFollowStats/{userId}")]
        public IActionResult GetFollowStats(string userId)
        {
            var followersCount = _connection.EWOMS_Followers.Count(f => f.FollowingId == userId);
            var followingCount = _connection.EWOMS_Followers.Count(f => f.FollowerId == userId);
            
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isFollowing = _connection.EWOMS_Followers
                .Any(f => f.FollowerId == currentUserId && f.FollowingId == userId);

            return Ok(new
            {
                followersCount,
                followingCount,
                isFollowing
            });
        }

        [Authorize]
        [HttpGet("GetFollowers/{userId}")]
        public IActionResult GetFollowers(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followers = _connection.EWOMS_Followers
                .Where(f => f.FollowingId == userId)
                .Select(f => new
                {
                    UserId = f.Follower.Id,
                    FullName = f.Follower.FullName,
                    ProfileImage = f.Follower.ProfileImage,
                    UserName = f.Follower.UserName,
                    IsFollowing = _connection.EWOMS_Followers.Any(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowerId)
                }).ToList();

            return Ok(followers);
        }

        [Authorize]
        [HttpGet("GetFollowing/{userId}")]
        public IActionResult GetFollowing(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var following = _connection.EWOMS_Followers
                .Where(f => f.FollowerId == userId)
                .Select(f => new
                {
                    UserId = f.Following.Id,
                    FullName = f.Following.FullName,
                    ProfileImage = f.Following.ProfileImage,
                    UserName = f.Following.UserName,
                    IsFollowing = _connection.EWOMS_Followers.Any(x => x.FollowerId == currentUserId && x.FollowingId == f.FollowingId)
                }).ToList();

            return Ok(following);
        }

        // =============================================
        // GROUP CHAT ENDPOINTS
        // =============================================

        [Authorize]
        [HttpPost("CreateGroup")]
        public IActionResult CreateGroup([FromBody] CreateGroupDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest("Group name is required.");

            var group = new ChatGroup
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedByUserId = userId,
                CreatedAt = DateTime.Now
            };
            _connection.ChatGroups.Add(group);
            _connection.SaveChanges();

            // Add creator as admin member
            _connection.ChatGroupMembers.Add(new ChatGroupMember
            {
                GroupId = group.Id,
                UserId = userId,
                IsAdmin = true,
                JoinedAt = DateTime.Now
            });

            // Add selected members
            if (dto.MemberIds != null)
            {
                foreach (var memberId in dto.MemberIds.Where(m => m != userId))
                {
                    _connection.ChatGroupMembers.Add(new ChatGroupMember
                    {
                        GroupId = group.Id,
                        UserId = memberId,
                        IsAdmin = false,
                        JoinedAt = DateTime.Now
                    });
                }
            }

            _connection.SaveChanges();

            return Ok(new { groupId = group.Id, name = group.Name, message = "Group created successfully" });
        }

        [Authorize]
        [HttpGet("GetMyGroups")]
        public IActionResult GetMyGroups()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var groups = _connection.ChatGroupMembers
                .Where(m => m.UserId == userId)
                .Select(m => new
                {
                    GroupId = m.GroupId,
                    Name = m.Group.Name,
                    Description = m.Group.Description,
                    CreatedAt = m.Group.CreatedAt,
                    IsAdmin = m.IsAdmin,
                    MemberCount = m.Group.Members.Count,
                    LastMessage = _connection.EWOMS_ChatMessages
                        .Where(msg => msg.GroupId == m.GroupId)
                        .OrderByDescending(msg => msg.SentAt)
                        .Select(msg => msg.Message)
                        .FirstOrDefault(),
                    LastMessageDate = _connection.EWOMS_ChatMessages
                        .Where(msg => msg.GroupId == m.GroupId)
                        .OrderByDescending(msg => msg.SentAt)
                        .Select(msg => (DateTime?)msg.SentAt)
                        .FirstOrDefault()
                })
                .OrderByDescending(g => g.LastMessageDate ?? g.CreatedAt)
                .ToList();

            return Ok(groups);
        }

        [Authorize]
        [HttpGet("GetGroupMessages/{groupId}")]
        public IActionResult GetGroupMessages(int groupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verify membership
            var isMember = _connection.ChatGroupMembers.Any(m => m.GroupId == groupId && m.UserId == userId);
            if (!isMember) return Forbid();

            var messages = _connection.EWOMS_ChatMessages
                .Where(m => m.GroupId == groupId)
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.Message,
                    m.Image,
                    m.Video,
                    m.Document,
                    m.FileName,
                    m.SentAt,
                    SenderName = _connection.Users
                        .Where(u => u.Id == m.SenderId)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),
                    SenderImage = _connection.Users
                        .Where(u => u.Id == m.SenderId)
                        .Select(u => u.ProfileImage)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(messages);
        }

        [Authorize]
        [HttpGet("GetGroupMembers/{groupId}")]
        public IActionResult GetGroupMembers(int groupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isMember = _connection.ChatGroupMembers.Any(m => m.GroupId == groupId && m.UserId == userId);
            if (!isMember) return Forbid();

            var members = _connection.ChatGroupMembers
                .Where(m => m.GroupId == groupId)
                .Select(m => new
                {
                    UserId = m.UserId,
                    FullName = m.User.FullName,
                    ProfileImage = m.User.ProfileImage,
                    IsAdmin = m.IsAdmin,
                    JoinedAt = m.JoinedAt
                }).ToList();

            return Ok(members);
        }

        [Authorize]
        [HttpPost("AddGroupMember/{groupId}/{memberId}")]
        public IActionResult AddGroupMember(int groupId, string memberId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isAdmin = _connection.ChatGroupMembers.Any(m => m.GroupId == groupId && m.UserId == userId && m.IsAdmin);
            if (!isAdmin) return Forbid();

            var alreadyMember = _connection.ChatGroupMembers.Any(m => m.GroupId == groupId && m.UserId == memberId);
            if (alreadyMember) return BadRequest("User is already a member.");

            _connection.ChatGroupMembers.Add(new ChatGroupMember
            {
                GroupId = groupId,
                UserId = memberId,
                IsAdmin = false,
                JoinedAt = DateTime.Now
            });
            _connection.SaveChanges();

            return Ok(new { message = "Member added successfully" });
        }

        [Authorize]
        [HttpDelete("LeaveGroup/{groupId}")]
        public IActionResult LeaveGroup(int groupId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var member = _connection.ChatGroupMembers.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
            if (member == null) return NotFound("You are not a member of this group.");

            _connection.ChatGroupMembers.Remove(member);
            _connection.SaveChanges();

            return Ok(new { message = "Left group successfully" });
        }

        [Authorize]
        [HttpPost("SendGroupMessage")]
        public IActionResult SendGroupMessage([FromBody] SendGroupMessageDTO dto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isMember = _connection.ChatGroupMembers.Any(m => m.GroupId == dto.GroupId && m.UserId == senderId);
            if (!isMember) return Forbid();

            var msg = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = null,
                GroupId = dto.GroupId,
                Message = dto.Message ?? "",
                Image = dto.Image,
                Video = dto.Video,
                Document = dto.Document,
                FileName = dto.FileName,
                SentAt = DateTime.Now,
                IsDelivered = true,
                IsRead = false
            };

            _connection.EWOMS_ChatMessages.Add(msg);
            _connection.SaveChanges();

            return Ok(msg);
        }
    }
}
