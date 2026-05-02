using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class ChatGroupMember
    {
        [Key]
        public int Id { get; set; }

        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public ChatGroup Group { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public MasterUser User { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.Now;

        public bool IsAdmin { get; set; } = false;
    }
}
