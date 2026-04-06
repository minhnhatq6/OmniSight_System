using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    // Dùng Data Annotation này là đủ để xác định khóa chính kép[PrimaryKey(nameof(ClassId), nameof(StudentId))]
    public class ClassMember
    {
        public int ClassId { get; set; }
        public int StudentId { get; set; }

        public DateTime JoinedAt { get; set; }

        // Navigation properties
        public virtual Class Class { get; set; }
        public virtual User Student { get; set; }
    }
}