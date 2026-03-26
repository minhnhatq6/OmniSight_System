using Microsoft.EntityFrameworkCore;
using OmniSight.Core.Entities;
using StreamEntity = OmniSight.Core.Entities.Stream;

namespace OmniSight.Data
{
    public class OmniSightDbContext : DbContext
    {
        // THÊM CONSTRUCTOR NÀY VÀO:
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
                optionsBuilder.UseSqlServer("Server=NHAT;Database=OmniSightDb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
        // THÊM ĐOẠN NÀY VÀO:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Sửa lỗi cho Streams (Bảng tin) - MỚI BỔ SUNG
            modelBuilder.Entity<StreamEntity>()
                .HasOne(s => s.Author) // Hoặc s.User tùy theo file Stream.cs
                .WithMany()
                .HasForeignKey(s => s.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            // 2. Sửa lỗi cho ClassMember (Sinh viên tham gia lớp)
            modelBuilder.Entity<ClassMember>()
                .HasOne(cm => cm.Student)
                .WithMany()
                .HasForeignKey(cm => cm.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. Sửa lỗi cho Assignment (Người tạo bài tập)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Creator)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // 4. Sửa lỗi cho Submission (Sinh viên nộp bài)
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // 5. Sửa lỗi cho ExamResult (Sinh viên làm bài thi)
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.Student)
                .WithMany()
                .HasForeignKey(er => er.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // 6. Sửa lỗi cho ViolationLog (Bản ghi vi phạm)
            // Lưu ý: Nếu báo lỗi ở v.Result, hãy kiểm tra tên thuộc tính trong file ViolationLog.cs
            modelBuilder.Entity<ViolationLog>()
                .HasOne(v => v.ExamResult)
                .WithMany()
                .HasForeignKey(v => v.ResultId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
