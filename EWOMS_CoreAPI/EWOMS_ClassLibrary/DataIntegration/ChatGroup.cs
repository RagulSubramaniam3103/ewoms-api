using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class ChatGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<ChatGroupMember> Members { get; set; }
    }
}
