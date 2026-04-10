using System;
using System.Collections.Generic;
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

        // --- HÀM MỚI: TÌM HOẶC TẠO MÔN HỌC ---
        public async Task<Subject> GetOrCreateSubjectAsync(string subjectName)
        {
            // Kiểm tra xem tên môn học đã tồn tại chưa (không phân biệt hoa thường)
            var existingSubject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectName.ToLower() == subjectName.ToLower());

            if (existingSubject != null)
            {
                return existingSubject; // Nếu có rồi thì trả về luôn
            }

            // Nếu chưa có thì tạo mới
            var newSubject = new Subject { SubjectName = subjectName };
            _context.Subjects.Add(newSubject);
            await _context.SaveChangesAsync();

            return newSubject;
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
            return await _context.Subjects.ToListAsync();
        }

        // Lấy danh sách cho Sinh viên (KHÔNG lấy Join Code - Bảo mật)
        public async Task<List<Class>> GetJoinedClassesAsync(int studentId)
        {
            return await _context.ClassMembers
                .Where(m => m.StudentId == studentId)
                .Select(m => m.Class)
                .ToListAsync();
        }

        public async Task<List<ClassMember>> GetClassMembersAsync(int classId)
        {
            return await _context.ClassMembers
                .Include(m => m.Student)
                .Where(m => m.ClassId == classId)
                .ToListAsync();
        }

        public async Task<string> GetClassNameAsync(int classId)
        {
            var cls = await _context.Classes.FindAsync(classId);
            return cls?.ClassName ?? "N/A";
        }

        // 2. Chức năng Học sinh Tham gia lớp
        public async Task<bool> JoinClassAsync(string joinCode, int studentId)
        {
            var targetClass = await _context.Classes.FirstOrDefaultAsync(c => c.JoinCode == joinCode);
            if (targetClass == null) return false;

            var isAlreadyJoined = await _context.ClassMembers
                .AnyAsync(m => m.ClassId == targetClass.ClassId && m.StudentId == studentId);
            if (isAlreadyJoined) return false;

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