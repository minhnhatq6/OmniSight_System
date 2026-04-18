using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("BaiNop")]
    public class Submission
    {
        [Key]
        [Column("MaBaiNop")]
        public int SubmissionId { get; set; }

        [Column("MaBaiTap")]
        public int AssignmentId { get; set; }

        [Column("MaHocSinh")]
        public int StudentId { get; set; }

        [MaxLength(255)]
        [Column("LinkBaiLam")]
        public string FileOrLinkUrl { get; set; }

        [Column("NgayNop")]
        public DateTime? SubmittedAt { get; set; }

        [Column("DiemSo")]
        public float? Score { get; set; }

        [Column("NhanXet")]
        public string? Feedback { get; set; }

        [ForeignKey("AssignmentId")]
        public virtual Assignment Assignment { get; set; }

        [ForeignKey("StudentId")]
        public virtual User Student { get; set; }
    }
}