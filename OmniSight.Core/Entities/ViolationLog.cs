using System;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{   
    public class ViolationLog
    {

        [Key]
        public int LogId { get; set; }

        public int ResultId { get; set; }   
        [MaxLength(50)]
        public string ViolationType { get; set; }
        [MaxLength(255)]
        public string ImageUrl { get; set; }
        public DateTime Timestamp { get; set; }
        [MaxLength(50)]
        public string Status { get; set; }

        // Navigation properties
        public virtual ExamResult ExamResult { get; set; }
    }
}
