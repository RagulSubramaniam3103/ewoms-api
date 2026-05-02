using EWOMS_ClassLibrary.DataControlled;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands
{
    public class MasterUser_MarkNotificationsReadHandler
    {
        private readonly ApplicationDbContext _context;

        public MasterUser_MarkNotificationsReadHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle()
        {
            var unread = await _context.masterNotifications
                .Where(n => !n.IsRead)
                .ToListAsync();

            if (unread.Any())
            {
                foreach (var notification in unread)
                {
                    notification.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
