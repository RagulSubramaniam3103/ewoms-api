using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EWOMS_ClassLibrary.DataIntegration.ChatMessenger
{
    [Table("EWO_Followers")]
    public class UserFollower
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FollowerId { get; set; } // The person who IS FOLLOWING

        [Required]
        public string FollowingId { get; set; } // The person BEING FOLLOWED

        public DateTime FollowedAt { get; set; } = DateTime.Now;

        // Navigation properties (optional but helpful)
        [ForeignKey("FollowerId")]
        public virtual MasterUser Follower { get; set; }

        [ForeignKey("FollowingId")]
        public virtual MasterUser Following { get; set; }
    }
}
