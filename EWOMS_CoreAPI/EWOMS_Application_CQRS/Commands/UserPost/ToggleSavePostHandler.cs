using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class ToggleSavePostHandler
    {
        private readonly ApplicationDbContext _context;

        public ToggleSavePostHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle(int postId, string userId)
        {
            var existingSave = await _context.SavedPosts
                .FirstOrDefaultAsync(s => s.PostId == postId && s.UserId == userId);

            bool isSaved;
            if (existingSave != null)
            {
                _context.SavedPosts.Remove(existingSave);
                isSaved = false;
            }
            else
            {
                var newSave = new SavedPost
                {
                    PostId = postId,
                    UserId = userId,
                    SavedAt = DateTime.Now
                };
                await _context.SavedPosts.AddAsync(newSave);
                isSaved = true;
            }

            await _context.SaveChangesAsync();

            return new
            {
                status = isSaved ? "saved" : "unsaved",
                isSaved = isSaved
            };
        }
    }
}
