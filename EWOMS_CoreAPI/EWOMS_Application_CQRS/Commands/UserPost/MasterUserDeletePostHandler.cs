using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class MasterUserDeletePostHandler
    {
        private readonly ApplicationDbContext _context;

        public MasterUserDeletePostHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<string> Handle(MasterUserDeletePostCommand command)
        {
            // ✅ FIX: Use AsNoTracking() to prevent the "Already being tracked" error
            var post = await _context.UserPost.AsNoTracking().FirstOrDefaultAsync(p => p.Id == command.PostId);
            if (post == null)
                return "Post not found";
            // ✅ Map to Deleted table (This will now work without conflict)
            var deletedPost = new DeleteUserPost
            {
                UId = post.Id, // Manual ID assignment is now safe
                UserId = post.UserId,
                ProfileImage = post.profileimage,
                Caption = post.Caption,
                CreatedAt = post.CreatedAt,
                DeletedBy = command.AdminId,
                DeletedAt = DateTime.UtcNow,
                Reason = command.Reason
            };
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DeleteUserPost.Add(deletedPost);
                
                // Add Audit Log
                var auditLog = new AdminAuditLog
                {
                    AdminId = command.AdminId ?? "Admin",
                    AdminName = "Administrator",
                    Action = "Post Moderation",
                    TargetId = post.Id.ToString(),
                    TargetName = $"Post by {post.UserId}",
                    Details = $"Post archived. Reason: {command.Reason}",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = "N/A"
                };
                _context.AdminAuditLogs.Add(auditLog);

                // ✅ Since we used AsNoTracking, we need to Re-Attach or use a Stub to remove
                var postStub = new EWOMS_ClassLibrary.DataIntegration.UserPost { Id = command.PostId };
                _context.UserPost.Remove(postStub);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "Post moved to deleted table successfully";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Error: {ex.Message}";
            }
        }
    }
}
