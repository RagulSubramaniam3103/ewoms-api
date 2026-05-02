using EWOMS_ClassLibrary.DataControlled;
using EWOMS_ClassLibrary.DataIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_Application_CQRS.Queries.Audit
{
    public class GetAdminAuditLogsHandler
    {
        private readonly ApplicationDbContext _context;

        public GetAdminAuditLogsHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminAuditLog>> Handle()
        {
            return await _context.AdminAuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(1000) // Limit to last 1000 logs for performance
                .ToListAsync();
        }
    }
}
