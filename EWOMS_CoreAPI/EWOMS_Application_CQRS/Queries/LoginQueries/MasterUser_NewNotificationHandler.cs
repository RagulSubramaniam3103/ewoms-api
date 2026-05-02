using EWOMS_ClassLibrary.DataControlled;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.LoginQueries
{
    public class MasterUser_NewNotificationHandler
    {
        private readonly ApplicationDbContext _Context;
        public MasterUser_NewNotificationHandler(ApplicationDbContext Context)
        {
            _Context = Context;
        }
        public async Task<object> GetHanlder()
        {
            var notifications = await  _Context.masterNotifications
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Message,
                    x.CreatedDate,
                    x.IsRead
                })
                .ToListAsync();

            return new
            {
                Count = notifications.Count,
                Data = notifications
            };
        }
    }
}
