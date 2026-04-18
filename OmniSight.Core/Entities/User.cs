using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("NguoiDung")]
    public class User
    {
        [Key]
        [Column("MaNguoiDung")]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        [Column("TenDangNhap")]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        [Column("MatKhau")]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("HoTen")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("AnhDaiDien")]
        public string AvatarUrl { get; set; } = string.Empty;

        [Column("LaHocSinh")]
        public bool IsStudent { get; set; } = true;

        [Column("LaGiaoVien")]
        public bool IsTeacher { get; set; } = false;

        [MaxLength(15)]
        [Column("SoDienThoai")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("MaGoogle")]
        public string GoogleId { get; set; } = string.Empty;

        [Column("DaXacNhanEmail")]
        public bool IsEmailConfirmed { get; set; } = false;

        [Column("DuLieuKhuonMat")]
        public string FaceEmbedding { get; set; } = string.Empty;

        public virtual ICollection<Class> TeachingClasses { get; set; } = new List<Class>();
        public virtual ICollection<ClassMember> ClassMemberships { get; set; } = new List<ClassMember>();
        public virtual ICollection<Stream> Streams { get; set; } = new List<Stream>();
        public virtual ICollection<Assignment> AssignmentsCreated { get; set; } = new List<Assignment>();
        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    }
}