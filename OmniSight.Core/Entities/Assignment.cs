using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("BaiTap")]
    public class Assignment
    {
        [Key]
        [Column("MaBaiTap")]
        public int AssignmentId { get; set; }

        [Column("MaLop")]
        public int ClassId { get; set; }

        [Required, MaxLength(200)]
        [Column("TieuDe")]
        public string Title { get; set; }

        [Column("MoTa")]
        public string Description { get; set; }

        [MaxLength(255)]
        [Column("LinkDinhKem")]
        public string AttachmentUrl { get; set; }

        [Column("HanNop")]
        public DateTime? DueDate { get; set; }

        [Column("MaNguoiTao")]
        public int CreatedBy { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User Creator { get; set; }

        public virtual ICollection<Submission> Submissions { get; set; }
    }
}