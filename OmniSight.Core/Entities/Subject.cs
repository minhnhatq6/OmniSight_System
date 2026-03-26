using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required, MaxLength(100)]
        public string SubjectName { get; set; }

        // Navigation properties
        public virtual ICollection<Class> Classes { get; set; }
    }
}
