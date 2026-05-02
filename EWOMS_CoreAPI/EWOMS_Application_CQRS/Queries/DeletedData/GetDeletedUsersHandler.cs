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
    public class GetDeletedUsersHandler
    {
        private readonly ApplicationDbContext _context;

        public GetDeletedUsersHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MasterUserBackup>> Handle()
        {
            return await _context.MasterUserBackups
                .OrderByDescending(u => u.DeletedAt)
                .ToListAsync();
        }
    }
}
