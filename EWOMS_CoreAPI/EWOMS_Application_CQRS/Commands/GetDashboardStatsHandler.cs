using EWOMS_ClassLibrary.DataControlled;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands
{
    public class GetDashboardStatsHandler
    {
        private readonly ApplicationDbContext _context;

        public GetDashboardStatsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle()
        {
            var totalMembers = await _context.Users.CountAsync();
            var totalPosts = await _context.UserPost.CountAsync(p => !p.IsDeleted && p.IsActive);
            
            // Mocking active sessions for now, or use actual session logic if available
            var activeSessions = await _context.Users.CountAsync(u => u.EmailConfirmed); // Using EmailConfirmed as a proxy for 'verified/active'
            
            var securityAlerts = await _context.Users.CountAsync(u => u.LockoutEnabled && u.LockoutEnd != null);

            return new
            {
                totalMembers = totalMembers,
                activeSessions = activeSessions,
                securityAlerts = securityAlerts,
                totalPosts = totalPosts
            };
        }
    }
}
