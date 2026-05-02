using EWOMS_Application_CQRS.Commands;
using EWOMS_Application_CQRS.Commands.UpdateUser;
using EWOMS_Application_CQRS.Commands.UserPost;
using EWOMS_Application_CQRS.Commands.UserDeletion;
using EWOMS_Application_CQRS.Queries.DeletedData;
using EWOMS_Application_CQRS.Queries.LockoutUser;
using EWOMS_Application_CQRS.Queries.LoginQueries;
using EWOMS_Application_CQRS.Queries.PasswordReset;
using EWOMS_Application_CQRS.Queries.RegisteredUser;
using EWOMS_ExternalClassLibrary_DTO.UserData_DTO;
using EWOMS_ExternalClassLibrary_DTO.UserData_DTO.UpdateUser;
using EWOMS_ExternalClassLibrary_DTO.UserData_DTO.UserPost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using EWOMS_Application_CQRS.Queries.Audit;

namespace EWOMS_CoreAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageUsersController : ControllerBase
    {
        private readonly MasterUser_RegisterHandler _registerHandler;
        private readonly MasterUser_LoginHandler _LoginHandler;
        private readonly MasterUser_LockoutHandler _lockoutHandler;
        private readonly MasterUser_ForgotPwdHandler _ForgotPwdHandler;
        private readonly MasterGetUserDetails_RoleHandler _UserDetails_RoleHandler;
        private readonly MasterUser_GetLockoutHandler _GetLockoutHandler;
        private readonly ForgotPassword_EmailSentHandler _EmailSentHandler;
        private readonly MasterForgotpassprofilehandler _masterpasswordhandler;
        private readonly MasterUser_NewNotificationHandler _NewNotificationHandler;
        private readonly MasterUser_MarkNotificationsReadHandler _MarkNotificationsReadHandler;
        private readonly MasterAdminUpdateHandler _masteradminupdate;
        private readonly MasterManagerUpdateHandler _mastermanagerHandler;
        private readonly MasterUserUpdateHandler _masterUserUpdateHandler;
        private readonly MasterUserPostGet_Handler _masteruserpostget;
        private readonly MasterUserDeletePostHandler _masterUserDeletePostHandler;
        private readonly MasterUserArchivePost _userarchievepost;
        private readonly MasterUserBlurPostHandler _masterUserBlurPostHandler;
        private readonly MasterUser_MarkSingleNotificationReadHandler _MarkSingleNotificationReadHandler;
        private readonly TogglePostLikeHandler _togglePostLikeHandler;
        private readonly ToggleSavePostHandler _toggleSavePostHandler;
        private readonly GetSavedPostsHandler _getSavedPostsHandler;
        private readonly GetDashboardStatsHandler _getDashboardStatsHandler;
        private readonly AddCommentHandler _addCommentHandler;
        private readonly GetCommentsHandler _getCommentsHandler;
        private readonly MasterUserStoryHandler _masterUserStoryHandler;
        private readonly MasterUser_DeleteFullDataHandler _deleteFullDataHandler;
        private readonly GetDeletedUsersHandler _getDeletedUsersHandler;
        private readonly GetDeletedPostsHandler _getDeletedPostsHandler;
        private readonly GetAdminAuditLogsHandler _getAdminAuditLogsHandler;
        private readonly UserManager<MasterUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        private readonly MasterUserPostHandler _masteruserpost;
        public ManageUsersController(
            MasterUser_RegisterHandler registerHandler,
            MasterUser_LoginHandler LoginHandler,
            MasterUser_LockoutHandler LockoutHandler,
            MasterUser_ForgotPwdHandler ForgotPwdHandler,
            MasterGetUserDetails_RoleHandler UserDetails_RoleHandler,
            MasterUser_GetLockoutHandler GetLockoutHandler,
            ForgotPassword_EmailSentHandler EmailSentHandler,
            MasterForgotpassprofilehandler masterpasswordhandler,
            MasterUser_NewNotificationHandler NewNotificationHandler,
            MasterUser_MarkNotificationsReadHandler MarkNotificationsReadHandler,
            MasterAdminUpdateHandler masteradminupdate,
            MasterManagerUpdateHandler mastermanagerHandler,
            MasterUserUpdateHandler masterUserUpdateHandler,
            MasterUserPostHandler masteruserpost,
            MasterUserPostGet_Handler masteruserpostget,
            MasterUserDeletePostHandler masterUserDeletePostHandler,
            MasterUserArchivePost userarchievepost,
            MasterUserBlurPostHandler masterUserBlurPost,
            MasterUser_MarkSingleNotificationReadHandler MarkSingleNotificationReadHandler,
            TogglePostLikeHandler togglePostLikeHandler,
            ToggleSavePostHandler toggleSavePostHandler,
            GetSavedPostsHandler getSavedPostsHandler,
            GetDashboardStatsHandler getDashboardStatsHandler,
            AddCommentHandler addCommentHandler,
            GetCommentsHandler getCommentsHandler,
            MasterUserStoryHandler storyHandler,
            MasterUser_DeleteFullDataHandler deleteFullDataHandler,
            GetDeletedUsersHandler getDeletedUsersHandler,
            GetDeletedPostsHandler getDeletedPostsHandler,
            GetAdminAuditLogsHandler getAdminAuditLogsHandler,
            UserManager<MasterUser> userManager,
            ApplicationDbContext dbContext)
        {
            _registerHandler = registerHandler;
            _LoginHandler = LoginHandler;
            _lockoutHandler = LockoutHandler;
            _ForgotPwdHandler = ForgotPwdHandler;
            _UserDetails_RoleHandler = UserDetails_RoleHandler;
            _GetLockoutHandler = GetLockoutHandler;
            _EmailSentHandler = EmailSentHandler;
            _masterpasswordhandler = masterpasswordhandler;
            _NewNotificationHandler = NewNotificationHandler;
            _MarkNotificationsReadHandler = MarkNotificationsReadHandler;
            _masteradminupdate = masteradminupdate;
            _mastermanagerHandler = mastermanagerHandler;
            _masterUserUpdateHandler = masterUserUpdateHandler;
            _masteruserpost = masteruserpost;
            _masteruserpostget = masteruserpostget;
            _masterUserDeletePostHandler = masterUserDeletePostHandler;
            _userarchievepost = userarchievepost;
            _masterUserBlurPostHandler = masterUserBlurPost;
            _MarkSingleNotificationReadHandler = MarkSingleNotificationReadHandler;
            _togglePostLikeHandler = togglePostLikeHandler;
            _toggleSavePostHandler = toggleSavePostHandler;
            _getSavedPostsHandler = getSavedPostsHandler;
            _getDashboardStatsHandler = getDashboardStatsHandler;
            _addCommentHandler = addCommentHandler;
            _getCommentsHandler = getCommentsHandler;
            _masterUserStoryHandler = storyHandler;
            _deleteFullDataHandler = deleteFullDataHandler;
            _getDeletedUsersHandler = getDeletedUsersHandler;
            _getDeletedPostsHandler = getDeletedPostsHandler;
            _getAdminAuditLogsHandler = getAdminAuditLogsHandler;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet("Login_MasterUser")]
        public async Task<IActionResult> LoginMaster([FromQuery] MasterUser_LoginDTO masterUser_Login)
        {
            var commands = new MasterUser_LoginCommand
            {
                Email = masterUser_Login.Email,
                Password = masterUser_Login.Password
            };
            var getresult = await _LoginHandler.Handler(commands);
            return Ok(new
            {
                getresult
            });
        }

        [HttpPost("ForgotPassword_EmailSent")]
        public async Task<IActionResult> ForgotPassword_EmailSent([FromQuery] Master_EmailSent master_Email)
        {
            var getemail = new ForgotPassword_EmailSentCommand
            {
                Email = master_Email.Email,
            };
            var existinguser = await _EmailSentHandler.Handler(getemail);
            return Ok(existinguser);
        }


        [Authorize(Policy = "AllowAll")]
        [HttpGet("ForgotPasswordChange")]
        public async Task<IActionResult> ForgotPasswordafterlogin([FromQuery] Master_ForgotPassProfile_DTO master_ForgotPass)
        {
            var commands = new MasterForgotpassprofilecommand
            {
                Email = master_ForgotPass.Email,
                OldPassword = master_ForgotPass.OldPassword,
                Password = master_ForgotPass.Password,
                ConfirmPassword = master_ForgotPass.ConfirmPassword,
            };

            var getresult = await _masterpasswordhandler.Handler(commands);
            return Ok(new
            {
                getresult
            });
        }


        [HttpPost("ForgotPassword_User")]
        public async Task<IActionResult> ForgotPassword([FromQuery] MasterUser_ForgotPWDDTO masterUser_ForgotPWD)
        {
            var commands = new MasterUser_ForgotPwdCommand
            {
                Email = masterUser_ForgotPWD.Email,
                OldPassword = masterUser_ForgotPWD.OldPassword,
                Password = masterUser_ForgotPWD.Password,
                ConfirmPassword = masterUser_ForgotPWD.ConfirmPassword,
                PasswordToken = masterUser_ForgotPWD.PasswordToken
            };
            var getresult = await _ForgotPwdHandler.Handler(commands);
            return Ok(new
            {
                getresult
            });
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("GetLockout_User")]
        public async Task<IActionResult> GetLockoutUser([FromQuery] Master_GetLockoutUserDTO User_Lockout)
        {
            var commands = new MasterUser_GetLockoutCommand
            {
                LockoutEndDate = User_Lockout.LockoutEndDate
            };
            var getresult = await _GetLockoutHandler.Handler(commands);
            return Ok(getresult);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("ReleaseLockout_User")]
        public async Task<IActionResult> LockoutUser([FromQuery] MasterUser_LockoutDTO masterUser_Lockout)
        {
            var commands = new MasterUser_LockoutCommand
            {
                Email = masterUser_Lockout.Email,
                ReleaseLockout = masterUser_Lockout.ReleaseLockout
            };
            var emailexists = await _lockoutHandler.Handler(commands);
            return Ok(new
            {
                emailexists
            });
        }

        //[Authorize(Policy= "AllowAll")]
        [HttpPost("Register_MasterUser")]
        public async Task<IActionResult> Registermasteruser([FromForm] Master_UserDTO data_DTO, IFormFile profileImage)
        {
            if (data_DTO != null)
            {
                byte[] imageBytes = null;

                if (profileImage != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await profileImage.CopyToAsync(ms);
                        imageBytes = ms.ToArray();
                    }
                }

                var commands = new MasterUser_RegisterCommand
                {
                    UserName = data_DTO.UserName,
                    FullName = data_DTO.FullName,
                    Email = data_DTO.Email,
                    Password = data_DTO.Password,
                    UserRoles = data_DTO.UserRole,
                    ProfileImage = imageBytes
                };
                var result = await _registerHandler.Handler(commands);
                return Ok(new
                {
                    Message = result
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = "Invalid user data."
                });
            }
        }
        //[Authorize(Policy = "Administration")]
        [HttpPost("GetUser_RoleWise")]
        public async Task<IActionResult> GetListUser([FromQuery] UserRoleDTO UserRole)
        {
            var UserRoles = new MasterGetUserDetails_RoleCommand
            {
                UserRole_Filter = UserRole.ToString()
            };
            var getresult = await _UserDetails_RoleHandler.Handle(UserRoles);
            return Ok(getresult);
        }
        [Authorize(Policy = "Administration")]
        [HttpGet("GetNewNotificationUserRegister")]
        public async Task<IActionResult> GetUserNotify()
        {
            var resultnotification = await _NewNotificationHandler.GetHanlder();
            return Ok(resultnotification);
        }

        [Authorize(Policy = "AllowAll")]
        [HttpPost("MarkAllNotificationsRead")]
        public async Task<IActionResult> MarkAllRead()
        {
            var result = await _MarkNotificationsReadHandler.Handle();
            return Ok(new { success = result, message = "All notifications marked as read" });
        }

        [Authorize(Policy = "AllowAll")]
        [HttpPost("MarkNotificationRead")]
        public async Task<IActionResult> MarkNotificationRead(int notificationId)
        {
            var result = await _MarkSingleNotificationReadHandler.Handle(notificationId);
            if (result)
                return Ok(new { success = true, message = "Notification marked as read" });

            return NotFound(new { success = false, message = "Notification not found" });
        }


        [Authorize(Policy = "Adminaccess")]
        [HttpPost("Update_Admin")]
        public async Task<IActionResult> UpdateAdmin([FromForm] MasterAdmin_updateDTO dto, IFormFile profileImage)
        {
            if (dto == null || string.IsNullOrEmpty(dto.UserId))
            {
                return BadRequest(new
                {
                    Message = "Invalid data"
                });
            }
            byte[] imageBytes = null;

            if (profileImage != null)
            {
                using (var ms = new MemoryStream())
                {
                    await profileImage.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }
            }
            var command = new MasterAdminUpdateCommand
            {
                UserId = dto.UserId,
                UserName = dto.UserName,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                ProfileImage = imageBytes,
                IsPrivate = dto.IsPrivate
            };
            var result = await _masteradminupdate.HanlderUpdate(command);
            return Ok(result);
        }

        [Authorize(Policy = "Manageraccess")]
        [HttpPost("Update_Manager")]
        public async Task<IActionResult> UpdateManager([FromForm] MasterManager_updateDTO dto, IFormFile profileImage)
        {
            if (dto == null || string.IsNullOrEmpty(dto.UserId))
            {
                return BadRequest(new
                {
                    Message = "Invalid data"
                });
            }
            byte[] imageBytes = null;

            if (profileImage != null)
            {
                using (var ms = new MemoryStream())
                {
                    await profileImage.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }
            }
            var command = new MasterManagerUpdateCommmand
            {
                UserId = dto.UserId,
                UserName = dto.UserName,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                ProfileImage = imageBytes,
                IsPrivate = dto.IsPrivate
            };
            var result = await _mastermanagerHandler.HanlderUpdate(command);
            return Ok(result);
        }
        [Authorize(Policy = "Useraccess")]
        [HttpPost("Update_User")]
        public async Task<IActionResult> UpdateUser([FromForm] MasterUser_updateDTO dto, IFormFile profileImage)
        {
            if (dto == null || string.IsNullOrEmpty(dto.UserId))
            {
                return BadRequest(new
                {
                    Message = "Invalid data"
                });
            }
            byte[] imageBytes = null;

            if (profileImage != null)
            {
                using (var ms = new MemoryStream())
                {
                    await profileImage.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }
            }
            var command = new MasterUserUpdateCommand
            {
                UserId = dto.UserId,
                UserName = dto.UserName,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                ProfileImage = imageBytes,
                IsPrivate = dto.IsPrivate
            };
            var result = await _masterUserUpdateHandler.HanlderUpdate(command);
            return Ok(result);
        }
        [HttpPost("UserPost")]
        public async Task<IActionResult> UserPost([FromForm] UserPostDTO dto, IFormFile postImage)
        {
            if (postImage == null)
            {
                return BadRequest("Image is required");
            }

            byte[] imageBytes;

            using (var ms = new MemoryStream())
            {
                await postImage.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            var command = new MasterUserPostCommand
            {
                UserId = dto.UserId,
                Caption = dto.Caption,
                Image = imageBytes,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _masteruserpost.Handle(command);

            return Ok(result);
        }

        [HttpGet("GetUserPosts")]
        public async Task<IActionResult> GetUserPosts([FromQuery] string? userId = null, [FromQuery] string? currentUserId = null)
        {
            var result = await _masteruserpostget.Handle(userId, currentUserId);

            return Ok(result);
        }

        [HttpPost("ShareStory")]
        public async Task<IActionResult> ShareStory([FromForm] UserStoryDTO dto, IFormFile storyImage)
        {
            try
            {
                // Safety: Ensure tables exist
                await EnsureStoryTablesExist();

                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (storyImage == null) return BadRequest("Story image is required.");
                if (string.IsNullOrEmpty(dto.UserId)) return BadRequest("UserId is required.");

                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    await storyImage.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                var result = await _masterUserStoryHandler.Handle(dto.UserId, dto.Caption, imageBytes);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Message = "An internal error occurred while sharing the broadcast.", 
                    Details = ex.Message 
                });
            }
        }

        private async Task EnsureStoryTablesExist()
        {
            string createStoryTable = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EWO_UserStory]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[EWO_UserStory] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [UserId] NVARCHAR(450) NOT NULL,
                        [StoryImage] VARBINARY(MAX) NOT NULL,
                        [Caption] NVARCHAR(200) NULL,
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
                        [ExpiresAt] DATETIME2 NOT NULL,
                        [IsActive] BIT NOT NULL DEFAULT 1,
                        [IsBlurred] BIT NOT NULL DEFAULT 0,
                        [ViewCount] INT NOT NULL DEFAULT 0
                    );
                END";

            string createViewTable = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EWO_UserStoryView]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[EWO_UserStoryView] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [StoryId] INT NOT NULL,
                        [ViewerId] NVARCHAR(450) NOT NULL,
                        [ViewedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT [FK_EWO_UserStoryView_EWO_UserStory] FOREIGN KEY ([StoryId]) REFERENCES [dbo].[EWO_UserStory] ([Id]) ON DELETE CASCADE
                    );
                END";

            await _dbContext.Database.ExecuteSqlRawAsync(createStoryTable);
            await _dbContext.Database.ExecuteSqlRawAsync(createViewTable);
        }

        private async Task EnsureBackupTableExists()
        {
            string createBackupTable = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EWO_MasterUserBackup]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[EWO_MasterUserBackup] (
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [UserId] NVARCHAR(450) NOT NULL,
                        [UserName] NVARCHAR(256) NOT NULL,
                        [Email] NVARCHAR(256) NOT NULL,
                        [FullName] NVARCHAR(MAX) NULL,
                        [UserRole] NVARCHAR(MAX) NULL,
                        [CreatedUser] DATETIME2 NULL,
                        [ProfileImage] VARBINARY(MAX) NULL,
                        [DeletedBy] NVARCHAR(MAX) NOT NULL,
                        [DeletedAt] DATETIME2 NOT NULL,
                        [Reason] NVARCHAR(MAX) NULL
                    );
                END";

            string createPostBackupTable = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EWO_DeleteUserPost]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[EWO_DeleteUserPost] (
                        [SNo] INT IDENTITY(1,1) PRIMARY KEY,
                        [UId] INT NOT NULL,
                        [UserId] NVARCHAR(450) NOT NULL,
                        [ProfileImage] VARBINARY(MAX) NULL,
                        [Caption] NVARCHAR(MAX) NULL,
                        [CreatedAt] DATETIME2 NOT NULL,
                        [DeletedBy] NVARCHAR(MAX) NOT NULL,
                        [DeletedAt] DATETIME2 NOT NULL,
                        [Reason] NVARCHAR(MAX) NULL
                    );
                END";

            await _dbContext.Database.ExecuteSqlRawAsync(createBackupTable);
            await _dbContext.Database.ExecuteSqlRawAsync(createPostBackupTable);
        }

        [HttpGet("GetStories")]
        public async Task<IActionResult> GetStories([FromQuery] string currentUserId)
        {
            var result = await _masterUserStoryHandler.GetStories(currentUserId);
            return Ok(result);
        }

        [HttpPost("MarkStoryAsSeen")]
        public async Task<IActionResult> MarkStoryAsSeen([FromQuery] int storyId, [FromQuery] string userId)
        {
            var result = await _masterUserStoryHandler.MarkAsSeen(storyId, userId);
            return Ok(new { Message = result });
        }

        [Authorize(Policy = "AllowAll")]
        [HttpPost("TogglePostLike")]
        public async Task<IActionResult> TogglePostLike([FromQuery] int postId, [FromQuery] string userId)
        {
            var result = await _togglePostLikeHandler.Handle(postId, userId);
            return Ok(result);
        }

        [Authorize(Policy = "AllowAll")]
        [HttpPost("ToggleSavePost")]
        public async Task<IActionResult> ToggleSavePost([FromQuery] int postId, [FromQuery] string userId)
        {
            var result = await _toggleSavePostHandler.Handle(postId, userId);
            return Ok(result);
        }

        [Authorize(Policy = "AllowAll")]
        [HttpGet("GetSavedPosts")]
        public async Task<IActionResult> GetSavedPosts([FromQuery] string userId)
        {
            var result = await _getSavedPostsHandler.Handle(userId);
            return Ok(result);
        }
        [Authorize(Policy = "AllowAll")]
        [HttpGet("GetDashboardStats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _getDashboardStatsHandler.Handle();
            return Ok(result);
        }

        [Authorize(Policy = "Adminaccess")]
        [HttpPost("DeleteAndArchivePost")]
        public async Task<IActionResult> DeleteAndArchivePost(int postId, string? reason)
        {
            var command = new MasterUserDeletePostCommand
            {
                PostId = postId,
                AdminId = User.Identity?.Name ?? "Admin",   
                Reason = reason
            };

            var result = await _masterUserDeletePostHandler.Handle(command);

            return Ok(new
            {
                Message = result
            });
        }
        [Authorize(Policy = "Adminaccess")]
        [HttpGet("GetArchivedPosts")]
        public async Task<IActionResult> GetArchivedPosts()
        {
            var result = await _userarchievepost.Handler();
            return Ok(result);
        }
        [Authorize(Policy = "Adminaccess")]
        [HttpPost("BlurPost")]
        public async Task<IActionResult> BlurPost(int postId)
        {
            var command = new MasterUserBlurPostCommand
            {
                PostId = postId,
                AdminId = User.Identity?.Name ?? "Admin"
            };
            var result = await _masterUserBlurPostHandler.Handle(command);

            return Ok(new
            {
                Message = result
            });
        }
        [Authorize(Policy = "AllowAll")]
        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromQuery] int postId, [FromQuery] string userId, [FromQuery] string content)
        {
            var result = await _addCommentHandler.Handle(postId, userId, content);
            return Ok(result);
        }

        [Authorize(Policy = "AllowAll")]
        [HttpGet("GetComments")]
        public async Task<IActionResult> GetComments([FromQuery] int postId)
        {
            var result = await _getCommentsHandler.Handle(postId);
            return Ok(result);
        }

        [Authorize(Policy = "Adminaccess")]
        [HttpPost("DeleteUserFullData")]
        public async Task<IActionResult> DeleteUserFullData([FromQuery] string userId, [FromQuery] string? reason)
        {
            await EnsureBackupTableExists();
            var command = new MasterUser_DeleteFullDataCommand
            {
                UserId = userId,
                AdminId = User.Identity?.Name ?? "Admin",
                Reason = reason
            };
            var result = await _deleteFullDataHandler.Handle(command);
            return Ok(new { Message = result });
        }
        [Authorize(Policy = "Adminaccess")]
        [HttpGet("GetDeletedUsers")]
        public async Task<IActionResult> GetDeletedUsers()
        {
            await EnsureBackupTableExists();
            var result = await _getDeletedUsersHandler.Handle();
            return Ok(result);
        }

        [Authorize(Policy = "Adminaccess")]
        [HttpGet("GetDeletedPosts")]
        public async Task<IActionResult> GetDeletedPosts([FromQuery] string? userId)
        {
            var result = await _getDeletedPostsHandler.Handle(userId);
            return Ok(result);
        }
        [Authorize(Policy = "Adminaccess")]
        [HttpGet("GetAuditLogs")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var result = await _getAdminAuditLogsHandler.Handle();
            return Ok(result);
        }

        [Authorize(Policy = "Adminaccess")]
        [HttpPost("UpdateUserRole")]
        public async Task<IActionResult> UpdateUserRole([FromQuery] string userId, [FromQuery] string newRole)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newRole))
            {
                return BadRequest(new { Message = "Identity Hash and Security Tier are required." });
            }

            try
            {
                // 1. Fetch the main Identity user
                var masterUser = await _userManager.FindByIdAsync(userId);
                if (masterUser == null) return NotFound(new { Message = "Target personnel identity not found in secure registry." });

                // 2. Synchronize Identity Roles
                var currentRoles = await _userManager.GetRolesAsync(masterUser);
                if (currentRoles.Count > 0)
                {
                    await _userManager.RemoveFromRolesAsync(masterUser, currentRoles);
                }
                await _userManager.AddToRoleAsync(masterUser, newRole);

                // 3. Identify and migrate professional details between role-specific tables
                // (Master_Admins, Master_MasterManager, Master_MasterUserDetails)
                
                // Fetch existing details from any of the three possible tiers
                var adminDetails = await _dbContext.Master_Admins.FirstOrDefaultAsync(x => x.UserId == userId);
                var managerDetails = await _dbContext.Master_MasterManager.FirstOrDefaultAsync(x => x.UserId == userId);
                var userDetails = await _dbContext.Master_MasterUserDetails.FirstOrDefaultAsync(x => x.UserId == userId);

                // Use the first one found as the source of truth for professional metadata
                var source = (object)adminDetails ?? (object)managerDetails ?? (object)userDetails;
                
                if (source != null)
                {
                    // Remove from old tables
                    if (adminDetails != null) _dbContext.Master_Admins.Remove(adminDetails);
                    if (managerDetails != null) _dbContext.Master_MasterManager.Remove(managerDetails);
                    if (userDetails != null) _dbContext.Master_MasterUserDetails.Remove(userDetails);

                    // Insert into the new appropriate tier table
                    if (newRole == "Admin")
                    {
                        var newAdmin = MapToNew<MasterAdmin>(source, userId);
                        _dbContext.Master_Admins.Add(newAdmin);
                    }
                    else if (newRole == "Manager")
                    {
                        var newManager = MapToNew<MasterManager>(source, userId);
                        _dbContext.Master_MasterManager.Add(newManager);
                    }
                    else
                    {
                        var newUser = MapToNew<MasterUserDetails>(source, userId);
                        _dbContext.Master_MasterUserDetails.Add(newUser);
                    }
                }

                await _dbContext.SaveChangesAsync();
                return Ok(new { Message = $"Personnel security tier successfully synchronized to {newRole} via Entity Framework." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    Message = "A system failure occurred during Entity-based security reassignment.", 
                    Details = ex.Message 
                });
            }
        }

        // Generic mapper to preserve professional metadata across detail tiers
        private T MapToNew<T>(object source, string userId) where T : new()
        {
            var target = new T();
            var sourceProps = source.GetType().GetProperties();
            var targetProps = typeof(T).GetProperties();

            foreach (var sp in sourceProps)
            {
                // Skip Id as it's an Identity column in the new table
                if (sp.Name == "Id") continue;

                var tp = targetProps.FirstOrDefault(p => p.Name == sp.Name && p.CanWrite);
                if (tp != null)
                {
                    tp.SetValue(target, sp.GetValue(source));
                }
            }
            
            // Ensure UserId is correctly set
            var userIdProp = targetProps.FirstOrDefault(p => p.Name == "UserId");
            if (userIdProp != null) userIdProp.SetValue(target, userId);

            return target;
        }
    }
}
