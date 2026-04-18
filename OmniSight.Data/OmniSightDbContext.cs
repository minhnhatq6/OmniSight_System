using Microsoft.EntityFrameworkCore;
using OmniSight.Core.Entities;
using StreamEntity = OmniSight.Core.Entities.Stream;

namespace OmniSight.Data
{
    public class OmniSightDbContext : DbContext
    {
        public OmniSightDbContext(DbContextOptions<OmniSightDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AuthToken> AuthTokens { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassMember> ClassMembers { get; set; }
        public DbSet<StreamEntity> Streams { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<ViolationLog> ViolationLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // MARS=True giúp xử lý đa luồng tốt hơn
                optionsBuilder.UseSqlServer("Server=NHAT;Database=OmniSightDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 0. Cấu hình Khóa chính kép cho Thành viên lớp
            modelBuilder.Entity<ClassMember>().HasKey(cm => new { cm.ClassId, cm.StudentId });

            // 1. Cấu hình Bảng tin (Stream)
            modelBuilder.Entity<StreamEntity>()
                .HasOne(s => s.Author)
                .WithMany(u => u.Streams) // Kết nối với danh sách Streams trong User.cs
                .HasForeignKey(s => s.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            // 2. Cấu hình Thành viên lớp (ClassMember)
            modelBuilder.Entity<ClassMember>()
                .HasOne(cm => cm.Student)
                .WithMany(u => u.ClassMemberships) // Kết nối với ClassMemberships trong User.cs
                .HasForeignKey(cm => cm.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. Cấu hình Bài tập (Assignment)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Creator)
                .WithMany(u => u.AssignmentsCreated) // Kết nối với AssignmentsCreated trong User.cs
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // 4. Cấu hình Bài nộp (Submission) - QUAN TRỌNG: Xóa cột UserId thừa
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany(u => u.Submissions) // Kết nối với Submissions trong User.cs
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // 5. Cấu hình Kết quả thi (ExamResult) - QUAN TRỌNG: Xóa cột UserId thừa
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Student)
                .WithMany(u => u.ExamResults) // Kết nối với ExamResults trong User.cs
                .HasForeignKey(er => er.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // 6. Cấu hình Lớp học (Class)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany(u => u.TeachingClasses) // Kết nối với TeachingClasses trong User.cs
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            // 7. Cấu hình Nhật ký vi phạm (ViolationLog)
            modelBuilder.Entity<ViolationLog>()
                .HasOne(v => v.ExamResult)
                .WithMany(er => er.ViolationLogs)
                .HasForeignKey(v => v.ResultId)
                .OnDelete(DeleteBehavior.NoAction);

            // 8. Cấu hình Môn học (Subject)
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany() // Nếu bảng User không có danh sách môn học thì để trống
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}