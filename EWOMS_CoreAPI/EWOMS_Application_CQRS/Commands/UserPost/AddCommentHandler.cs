using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using System;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class AddCommentHandler
    {
        private readonly ApplicationDbContext _context;

        public AddCommentHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle(int postId, string userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return new { status = "error", message = "Comment cannot be empty" };

            var comment = new PostComment
            {
                PostId = postId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            try
            {
                await _context.PostComments.AddAsync(comment);
                await _context.SaveChangesAsync();
                return new { status = "success", message = "Comment added" };
            }
            catch (Exception ex)
            {
                return new { status = "error", message = ex.Message, detail = ex.InnerException?.Message };
            }
        }
    }
}
