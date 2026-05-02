using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class AdminAuditLog
    {
        [Key]
        public int Id { get; set; }
        public string AdminId { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // e.g., "Role Update", "User Purge", "Post Moderation"
        public string? TargetId { get; set; }
        public string? TargetName { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
    }
}
