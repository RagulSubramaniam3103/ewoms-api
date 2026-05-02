using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class SeedStoryHandler
    {
        private readonly ApplicationDbContext _context;
        private readonly MasterUserStoryHandler _storyHandler;

        public SeedStoryHandler(ApplicationDbContext context, MasterUserStoryHandler storyHandler)
        {
            _context = context;
            _storyHandler = storyHandler;
        }

        public async Task<string> Handle(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return "User not found.";

            // Smallest valid PNG (Transparent 1x1) for testing
            byte[] testImage = new byte[] { 
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52, 
                0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53, 
                0xDE, 0x00, 0x00, 0x00, 0x0C, 0x49, 0x44, 0x41, 0x54, 0x08, 0xD7, 0x63, 0xF8, 0xFF, 0xFF, 0x3F, 
                0x00, 0x05, 0xFE, 0x02, 0xFE, 0xDC, 0x44, 0x74, 0x06, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 
                0x44, 0xAE, 0x42, 0x60, 0x82 
            };

            await _storyHandler.Handle(user.Id, "SYSTEM TEST: Intelligence Broadcast Operational", testImage);
            return $"Test story seeded successfully for {email}. UserID: {user.Id}";
        }
    }
}
