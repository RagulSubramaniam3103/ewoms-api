using EWOMS_ClassLibrary.DataControlled;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class MasterUserArchivePost
    {
        private readonly ApplicationDbContext _context;
        public MasterUserArchivePost(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<object> Handler()
        {
            var posts = await _context.DeleteUserPost
               .OrderByDescending(x => x.DeletedAt)
               .Select(x => new
               {
                   x.UId,
                   x.UserId,
                   x.Caption,
                   x.ProfileImage,
                   x.CreatedAt,
                   x.DeletedBy,
                   x.DeletedAt,
                   x.Reason
               })
               .ToListAsync();
            return posts;
        }
       
    }
}
