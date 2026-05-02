using EWOMS_ClassLibrary.DataControlled;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class MasterUserBlurPostHandler
    {
        private readonly ApplicationDbContext _context;

        public MasterUserBlurPostHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(MasterUserBlurPostCommand command)
        {
            var post = await _context.UserPost.FirstOrDefaultAsync(x => x.Id == command.PostId);

            if (post == null)
                return "Post not found";

            // 🔥 Toggle Blur
            post.IsBlurred = !post.IsBlurred;
            post.ModeratedBy = command.AdminId;
            post.ModeratedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return post.IsBlurred ? "Post blurred successfully" : "Post unveiled successfully";
        }
    }
}
