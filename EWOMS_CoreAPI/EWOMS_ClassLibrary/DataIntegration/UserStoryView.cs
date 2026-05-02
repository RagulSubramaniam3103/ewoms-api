using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWOMS_ClassLibrary.DataIntegration
{
    public class UserStoryView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StoryId { get; set; }
        public string ViewerId { get; set; }
        public DateTime ViewedAt { get; set; }

        // Navigation
        public virtual UserStory Story { get; set; }
    }
}
