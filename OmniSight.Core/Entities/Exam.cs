using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }

        public int ClassId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public int DurationMinutes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual Class Class { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<ExamResult> ExamResults { get; set; }
    }
}
