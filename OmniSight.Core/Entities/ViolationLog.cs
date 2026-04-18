using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("NhatKyViPham")]
    public class ViolationLog
    {
        [Key]
        [Column("MaViPham")]
        public int LogId { get; set; }

        [Column("MaKetQua")]
        public int ResultId { get; set; }

        [MaxLength(50)]
        [Column("LoaiViPham")]
        public string ViolationType { get; set; }

        [MaxLength(255)]
        [Column("AnhViPham")]
        public string ImageUrl { get; set; }

        [Column("ThoiGianViPham")]
        public DateTime Timestamp { get; set; }

        [MaxLength(50)]
        [Column("TrangThai")]
        public string Status { get; set; }

        [ForeignKey("ResultId")]
        public virtual ExamResult ExamResult { get; set; }
    }
}