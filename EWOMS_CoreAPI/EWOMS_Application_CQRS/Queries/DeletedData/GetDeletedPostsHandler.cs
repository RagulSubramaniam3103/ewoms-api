using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.DeletedData
{
    public class GetDeletedPostsHandler
    {
        private readonly ApplicationDbContext _context;

        public GetDeletedPostsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeleteUserPost>> Handle(string? userId = null)
        {
            var query = _context.DeleteUserPost.AsQueryable();
            
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(p => p.UserId == userId);
            }

            return await query
                .OrderByDescending(p => p.DeletedAt)
                .ToListAsync();
        }
    }
}
