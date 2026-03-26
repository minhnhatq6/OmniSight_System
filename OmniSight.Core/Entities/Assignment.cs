using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public int ClassId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [MaxLength(255)]
        public string AttachmentUrl { get; set; }

        public DateTime? DueDate { get; set; }
        public int CreatedBy { get; set; }

        // Navigation properties
        public virtual Class Class { get; set; }
        public virtual User Creator { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
