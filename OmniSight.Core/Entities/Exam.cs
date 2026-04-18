using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("DeThi")]
    public class Exam
    {
        [Key]
        [Column("MaDeThi")]
        public int ExamId { get; set; }

        [Column("MaLop")]
        public int ClassId { get; set; }

        [Required, MaxLength(200)]
        [Column("TieuDe")]
        public string Title { get; set; }

        [Column("ThoiGianLamBai")]
        public int DurationMinutes { get; set; }

        [Column("NgayTao")]
        public DateTime CreatedAt { get; set; }

        [Column("ThoiGianMoDe")]
        public DateTime? StartTime { get; set; }

        [Column("ThoiGianDongDe")]
        public DateTime? EndTime { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<ExamResult> ExamResults { get; set; }
    }
}