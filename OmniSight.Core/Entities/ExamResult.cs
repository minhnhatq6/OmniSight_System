using System;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    public class ExamResult
    {
        [Key]
        public int ResultId { get; set; }

        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public float? Score { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public virtual Exam Exam { get; set; }
        public virtual User Student { get; set; }
        public virtual ICollection<ViolationLog> ViolationLogs { get; set; }
    }
}
