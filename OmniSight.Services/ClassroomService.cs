using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OmniSight.Core.Entities;
using OmniSight.Data;

namespace OmniSight.Services
{
    public class ClassService
    {
        private readonly OmniSightDbContext _context;

        public ClassService(OmniSightDbContext context)
        {
            _context = context;
        }

        // Hàm sinh mã Join Code ngẫu nhiên 6 ký tự
        public string GenerateJoinCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // 1. Chức năng Giáo viên Tạo lớp
        public async Task<Class> CreateClassAsync(string className, int teacherId, int subjectId)
        {
            var newClass = new Class
            {
                ClassName = className,
                TeacherId = teacherId,
                SubjectId = subjectId,
                JoinCode = GenerateJoinCode()
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            // Xong! Không cần đụng tới bảng ClassMember.
            return newClass;
        }

        // Lấy danh sách cho Giáo viên (Thấy được Join Code)
        public async Task<List<Class>> GetOwnedClassesAsync(int teacherId)
        {
            return await _context.Classes
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<List<Subject>> GetSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync(); // Lấy toàn bộ môn học từ DB [cite: 61]
        }

        // Lấy danh sách cho Sinh viên (KHÔNG lấy Join Code - Bảo mật)
        public async Task<List<Class>> GetJoinedClassesAsync(int studentId)
        {
            return await _context.ClassMembers
                .Where(m => m.StudentId == studentId)
                .Select(m => m.Class) // Kết nối sang bảng Class để lấy thông tin lớp
                .ToListAsync();
        }

        public async Task<List<ClassMember>> GetClassMembersAsync(int classId)
        {
            return await _context.ClassMembers
                .Include(m => m.Student) // Kéo theo thông tin User để lấy tên/email [cite: 61]
                .Where(m => m.ClassId == classId)
                .ToListAsync();
        }

        // 2. Chức năng Học sinh Tham gia lớp (Dùng cho bước tiếp theo)
        public async Task<bool> JoinClassAsync(string joinCode, int studentId)
        {
            // Tìm lớp học dựa trên mã code
            var targetClass = await _context.Classes.FirstOrDefaultAsync(c => c.JoinCode == joinCode);
            if (targetClass == null) return false; // Mã code sai hoặc lớp không tồn tại

            // Kiểm tra xem học sinh đã ở trong lớp chưa
            var isAlreadyJoined = await _context.ClassMembers
                .AnyAsync(m => m.ClassId == targetClass.ClassId && m.StudentId == studentId);
            if (isAlreadyJoined) return false;

            // Thêm học sinh vào lớp
            var member = new ClassMember
            {
                ClassId = targetClass.ClassId,
                StudentId = studentId,
                JoinedAt = DateTime.Now
            };

            _context.ClassMembers.Add(member);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}