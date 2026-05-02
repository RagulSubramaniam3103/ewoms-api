using EWOMS_ClassLibrary.DataControlled;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Commands.UserPost
{
    public class GetCommentsHandler
    {
        private readonly ApplicationDbContext _context;

        public GetCommentsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<object>> Handle(int postId)
        {
            var comments = await (from c in _context.PostComments
                                  join u in _context.Users on c.UserId equals u.Id
                                  where c.PostId == postId
                                  orderby c.CreatedAt descending
                                  select new
                                  {
                                      c.Id,
                                      c.Content,
                                      c.CreatedAt,
                                      c.UserId,
                                      UserName = u.UserName,
                                      UserImage = u.ProfileImage != null ? Convert.ToBase64String(u.ProfileImage) : null
                                  })
                                  .ToListAsync();

            return comments.Cast<object>().ToList();
        }
    }
}
