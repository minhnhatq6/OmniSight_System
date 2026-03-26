using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    [PrimaryKey(nameof(ClassId), nameof(StudentId))]
    public class ClassMember
    {   
        [Key]
        public int ClassId { get; set; }
        [Key]
        public int StudentId { get; set; }

        public DateTime JoinedAt { get; set; }

        // Navigation properties
        public virtual Class Class { get; set; }
        public virtual User Student { get; set; }

    }

}
