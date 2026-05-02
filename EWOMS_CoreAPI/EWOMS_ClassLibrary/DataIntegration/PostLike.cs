using System;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class PostLike
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public DateTime LikedAt { get; set; }
    }
}
