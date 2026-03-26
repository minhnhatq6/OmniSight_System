using System;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    public class Stream
    {
        [Key]
        public int PostId { get; set; }

        public int ClassId { get; set; }
        public int AuthorId { get; set; }

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual Class Class { get; set; }
        public virtual User Author { get; set; }
    }
}
