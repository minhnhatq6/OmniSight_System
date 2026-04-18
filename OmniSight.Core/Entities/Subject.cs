using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("MonHoc")]
    public class Subject
    {
        [Key]
        [Column("MaMonHoc")]
        public int SubjectId { get; set; }

        [Required, MaxLength(100)]
        [Column("TenMonHoc")]
        public string SubjectName { get; set; }

        [Column("MaGiaoVien")]
        public int? TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
    }
}