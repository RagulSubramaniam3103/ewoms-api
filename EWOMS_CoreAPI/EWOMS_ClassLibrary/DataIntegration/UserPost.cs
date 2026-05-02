using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class UserPost
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public byte[] profileimage { get; set; }
        public string Caption { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;     // false = disabled
        public bool IsBlurred { get; set; } = false;   // true = blur content
        public bool IsDeleted { get; set; } = false;   // soft delete

        public string? ModeratedBy { get; set; }
        public DateTime? ModeratedAt { get; set; }

    }
}
