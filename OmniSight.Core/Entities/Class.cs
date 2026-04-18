using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("LopHoc")]
    public class Class
    {
        [Key]
        [Column("MaLop")]
        public int ClassId { get; set; }

        [Required, MaxLength(100)]
        [Column("TenLop")]
        public string ClassName { get; set; }

        [MaxLength(10)]
        [Column("MaThamGia")]
        public string JoinCode { get; set; }

        [Column("MaGiaoVien")]
        public int TeacherId { get; set; }

        [Column("MaMonHoc")]
        public int SubjectId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }

        public virtual ICollection<ClassMember> ClassMembers { get; set; }
        public virtual ICollection<Stream> Streams { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
    }
}