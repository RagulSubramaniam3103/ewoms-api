using EWOMS_ClassLibrary.DataControlled;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands
{
    public class MasterUser_MarkSingleNotificationReadHandler
    {
        private readonly ApplicationDbContext _context;

        public MasterUser_MarkSingleNotificationReadHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(int notificationId)
        {
            var notification = await _context.masterNotifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
