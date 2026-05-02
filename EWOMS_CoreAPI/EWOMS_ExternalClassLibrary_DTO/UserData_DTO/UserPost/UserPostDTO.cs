using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ExternalClassLibrary_DTO.UserData_DTO.UserPost
{
    public class UserPostDTO
    {
        public string? UserId { get; set; }
        public string? Caption { get; set; }
        public byte[]? Image { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
