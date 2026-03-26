using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        // Chúng ta giữ lại Guid này nếu bạn muốn, nhưng nên gán mặc định
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        public string AvatarUrl { get; set; } = string.Empty;

        public bool IsStudent { get; set; } = true;  // Mặc định luôn là học sinh
        public bool IsTeacher { get; set; } = false; // Mặc định chưa phải giáo viên
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty; // Thêm số điện thoại cho profile

        // Quan trọng: Gán rỗng để Database không báo lỗi NULL
        public string FaceVector { get; set; } = string.Empty;

        [MaxLength(255)]
        public string FaceImageUrl { get; set; } = string.Empty;

        [MaxLength(100)]
        public string GoogleId { get; set; } = string.Empty;

        public bool IsEmailConfirmed { get; set; } = false;

        public string FaceEmbedding { get; set; } = string.Empty;

        // Khởi tạo sẵn các Collection để tránh lỗi NullReferenceException sau này
        public virtual ICollection<Class> TeachingClasses { get; set; } = new List<Class>();
        public virtual ICollection<ClassMember> ClassMemberships { get; set; } = new List<ClassMember>();
        public virtual ICollection<Stream> Streams { get; set; } = new List<Stream>();
        public virtual ICollection<Assignment> AssignmentsCreated { get; set; } = new List<Assignment>();
        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    }
}