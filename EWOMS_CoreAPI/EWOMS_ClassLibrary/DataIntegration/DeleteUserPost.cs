using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class DeleteUserPost
    {
        [Key]
        public int SNo { get; set; }
        public int UId { get; set; }  
        public string UserId { get; set; }
        public byte[] ProfileImage { get; set; }
        public string Caption { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime DeletedAt { get; set; }
        public string? Reason { get; set; }
    
    }
}
