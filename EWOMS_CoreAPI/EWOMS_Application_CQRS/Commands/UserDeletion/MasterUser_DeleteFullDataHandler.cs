using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_ClassLibrary.DataIntegration.ChatMessenger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserDeletion
{
    public class MasterUser_DeleteFullDataHandler
    {
        private readonly UserManager<MasterUser> _userManager;
        private readonly ApplicationDbContext _context;

        public MasterUser_DeleteFullDataHandler(
            UserManager<MasterUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<string> Handle(MasterUser_DeleteFullDataCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
                return "User not found.";

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 0. Backup User Data
                var roles = await _userManager.GetRolesAsync(user);
                var userBackup = new MasterUserBackup
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FullName = user.FullName,
                    UserRole = string.Join(", ", roles),
                    CreatedUser = user.CreatedUser,
                    ProfileImage = user.ProfileImage,
                    DeletedBy = command.AdminId ?? "Admin",
                    DeletedAt = DateTime.UtcNow,
                    Reason = command.Reason
                };
                await _context.MasterUserBackups.AddAsync(userBackup);

                // Add Audit Log
                var auditLog = new AdminAuditLog
                {
                    AdminId = command.AdminId ?? "Admin",
                    AdminName = "Administrator", // Ideally fetch from a service or claims
                    Action = "User Purge",
                    TargetId = user.Id,
                    TargetName = user.UserName,
                    Details = $"Full data purge. Reason: {command.Reason}",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "N/A" // Optional: capture from HttpContext if available
                };
                await _context.AdminAuditLogs.AddAsync(auditLog);

                await _context.SaveChangesAsync();

                // 1. Delete user posts and related data (likes, comments, saved posts)
                var posts = _context.UserPost.Where(p => p.UserId == user.Id).ToList();
                foreach (var post in posts)
                {
                    // Backup Post
                    var postBackup = new DeleteUserPost
                    {
                        UId = post.Id,
                        UserId = post.UserId,
                        ProfileImage = post.profileimage,
                        Caption = post.Caption,
                        CreatedAt = post.CreatedAt,
                        DeletedBy = command.AdminId ?? "Admin",
                        DeletedAt = DateTime.UtcNow,
                        Reason = command.Reason
                    };
                    await _context.DeleteUserPost.AddAsync(postBackup);

                    var likes = _context.PostLikes.Where(l => l.PostId == post.Id);
                    _context.PostLikes.RemoveRange(likes);

                    var comments = _context.PostComments.Where(c => c.PostId == post.Id);
                    _context.PostComments.RemoveRange(comments);

                    var savedPosts = _context.SavedPosts.Where(s => s.PostId == post.Id);
                    _context.SavedPosts.RemoveRange(savedPosts);
                    
                    var deletedPostRecord = _context.DeleteUserPost.Where(dp => dp.UId == post.Id);
                    _context.DeleteUserPost.RemoveRange(deletedPostRecord);
                }
                _context.UserPost.RemoveRange(posts);

                // 2. Delete user stories and views
                var stories = _context.UserStories.Where(s => s.UserId == user.Id).ToList();
                foreach (var story in stories)
                {
                    var views = _context.UserStoryViews.Where(v => v.StoryId == story.Id);
                    _context.UserStoryViews.RemoveRange(views);
                }
                _context.UserStories.RemoveRange(stories);
                
                var myViews = _context.UserStoryViews.Where(v => v.ViewerId == user.Id);
                _context.UserStoryViews.RemoveRange(myViews);

                // 3. Delete user interactions on other posts
                var myLikes = _context.PostLikes.Where(l => l.UserId == user.Id);
                _context.PostLikes.RemoveRange(myLikes);

                var myComments = _context.PostComments.Where(c => c.UserId == user.Id);
                _context.PostComments.RemoveRange(myComments);

                var mySavedPosts = _context.SavedPosts.Where(s => s.UserId == user.Id);
                _context.SavedPosts.RemoveRange(mySavedPosts);

                // 4. Delete followers/following
                var following = _context.EWOMS_Followers.Where(f => f.FollowerId == user.Id);
                _context.EWOMS_Followers.RemoveRange(following);

                var followers = _context.EWOMS_Followers.Where(f => f.FollowingId == user.Id);
                _context.EWOMS_Followers.RemoveRange(followers);

                // 5. Delete friend requests
                var sentRequests = _context.EWOMS_FriendRequests.Where(r => r.SenderId == user.Id);
                _context.EWOMS_FriendRequests.RemoveRange(sentRequests);

                var receivedRequests = _context.EWOMS_FriendRequests.Where(r => r.ReceiverId == user.Id);
                _context.EWOMS_FriendRequests.RemoveRange(receivedRequests);

                // 6. Delete Chat data
                var chatMessages = _context.ChatMessages.Where(m => m.SenderId == user.Id);
                _context.ChatMessages.RemoveRange(chatMessages);

                var conversationMembers = _context.ConversationMembers.Where(m => m.UserId == user.Id);
                _context.ConversationMembers.RemoveRange(conversationMembers);
                
                var groupMembers = _context.ChatGroupMembers.Where(m => m.UserId == user.Id);
                _context.ChatGroupMembers.RemoveRange(groupMembers);

                // 7. Delete password logs
                var passwordLogs = _context.Master_UserPasswordLogs.Where(l => l.UserId == user.Id);
                _context.Master_UserPasswordLogs.RemoveRange(passwordLogs);

                // 8. Delete role-specific data
                var adminData = _context.Master_Admins.Where(a => a.UserId == user.Id);
                _context.Master_Admins.RemoveRange(adminData);

                var managerData = _context.Master_MasterManager.Where(m => m.UserId == user.Id);
                _context.Master_MasterManager.RemoveRange(managerData);

                var userData = _context.Master_MasterUserDetails.Where(u => u.UserId == user.Id);
                _context.Master_MasterUserDetails.RemoveRange(userData);

                // 9. Save changes to DB before deleting Identity user (to avoid FK issues)
                await _context.SaveChangesAsync();

                // 10. Delete the user via UserManager
                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return "User deletion failed: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }

                await transaction.CommitAsync();

                return "User and all associated data deleted successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return "An error occurred: " + ex.Message;
            }
        }
    }
}
