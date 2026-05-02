using System;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class PostComment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation (Optional for EF)
        // public virtual MasterUser User { get; set; }
    }
}
