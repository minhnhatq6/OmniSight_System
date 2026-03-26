using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    public class Class
    {
        [Key]
        public int ClassId { get; set; }

        [Required, MaxLength(100)]
        public string ClassName { get; set; }

        [MaxLength(10)]
        public string JoinCode { get; set; }

        public int TeacherId { get; set; }
        public int SubjectId { get; set; }

        // Navigation properties
        public virtual User Teacher { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<ClassMember> ClassMembers { get; set; }
        public virtual ICollection<Stream> Streams { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
    }
}
