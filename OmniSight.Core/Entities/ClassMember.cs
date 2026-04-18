using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OmniSight.Core.Entities
{
    [Table("ThanhVienLop")]
    [PrimaryKey(nameof(ClassId), nameof(StudentId))]
    public class ClassMember
    {
        [Column("MaLop")]
        public int ClassId { get; set; }

        [Column("MaHocSinh")]
        public int StudentId { get; set; }

        [Column("NgayThamGia")]
        public DateTime JoinedAt { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [ForeignKey("StudentId")]
        public virtual User Student { get; set; }
    }
}