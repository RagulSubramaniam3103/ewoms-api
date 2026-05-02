using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class ConversationMember
    {
        [Key]
        public int Id { get; set; }

        public int ConversationId { get; set; }
        [ForeignKey("ConversationId")]
        public Conversation Conversation { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public MasterUser User { get; set; }
    }
}
