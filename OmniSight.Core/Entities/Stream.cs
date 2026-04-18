using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("BangTin")]
    public class Stream
    {
        [Key]
        [Column("MaBaiDang")]
        public int PostId { get; set; }

        [Column("MaLop")]
        public int ClassId { get; set; }

        [Column("MaNguoiDang")]
        public int AuthorId { get; set; }

        [Column("NoiDung")]
        public string Content { get; set; }

        [Column("NgayTao")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [ForeignKey("AuthorId")]
        public virtual User Author { get; set; }
    }
}