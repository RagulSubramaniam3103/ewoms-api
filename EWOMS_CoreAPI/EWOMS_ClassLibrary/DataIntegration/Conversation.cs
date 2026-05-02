using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<ConversationMember> Members { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
    }
}
