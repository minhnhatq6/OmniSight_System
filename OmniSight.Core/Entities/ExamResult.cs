using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("KetQuaThi")]
    public class ExamResult
    {
        [Key]
        [Column("MaKetQua")]
        public int ResultId { get; set; }

        [Column("MaDeThi")]
        public int ExamId { get; set; }

        [Column("MaHocSinh")]
        public int StudentId { get; set; }

        [Column("DiemSo")]
        public float? Score { get; set; }

        [Column("ThoiGianBatDau")]
        public DateTime? StartedAt { get; set; }

        [Column("ThoiGianKetThuc")]
        public DateTime? CompletedAt { get; set; }

        [Column("DuLieuDapAn")]
        public string? AnswersData { get; set; }

        [Column("TrangThaiLamLai")]
        [MaxLength(50)]
        public string? RetakeStatus { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        [ForeignKey("StudentId")]
        public virtual User Student { get; set; }

        public virtual ICollection<ViolationLog> ViolationLogs { get; set; }
    }
}