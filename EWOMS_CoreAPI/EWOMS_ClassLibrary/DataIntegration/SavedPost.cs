using System;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class SavedPost
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public DateTime SavedAt { get; set; }
    }
}
