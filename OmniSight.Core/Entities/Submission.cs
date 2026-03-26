using System;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }

        public int AssignmentId { get; set; }
        public int StudentId { get; set; }

        [MaxLength(255)]
        public string FileOrLinkUrl { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public float? Score { get; set; }
        public string Feedback { get; set; }

        // Navigation properties
        public virtual Assignment Assignment { get; set; }
        public virtual User Student { get; set; }
    }
}
