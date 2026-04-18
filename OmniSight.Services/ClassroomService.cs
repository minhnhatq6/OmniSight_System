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

        public string GenerateJoinCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // --- HÀM ĐÃ SỬA: TÌM HOẶC TẠO MÔN HỌC RIÊNG CHO GV ---
        public async Task<Subject> GetOrCreateSubjectAsync(string subjectName, int teacherId)
        {
            // Tìm môn trùng tên thuộc về GV này HOẶC là môn dùng chung (null)
            var existingSubject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.SubjectName.ToLower() == subjectName.ToLower()
                                       && (s.TeacherId == teacherId || s.TeacherId == null));

            if (existingSubject != null) return existingSubject;

            // Nếu chưa có thì tạo mới môn học GẮN VỚI ID GIÁO VIÊN
            var newSubject = new Subject
            {
                SubjectName = subjectName,
                TeacherId = teacherId
            };
            _context.Subjects.Add(newSubject);
            await _context.SaveChangesAsync();

            return newSubject;
        }

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

        public async Task<List<Class>> GetOwnedClassesAsync(int teacherId)
        {
            return await _context.Classes
                .Include(c => c.Subject)
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();
        }

        // --- HÀM ĐÃ SỬA: LẤY MÔN HỌC THEO GIÁO VIÊN ---
        public async Task<List<Subject>> GetSubjectsByTeacherAsync(int teacherId)
        {
            return await _context.Subjects
                .Where(s => s.TeacherId == null || s.TeacherId == teacherId)
                .OrderBy(s => s.SubjectName)
                .ToListAsync();
        }

        public async Task<List<Class>> GetJoinedClassesAsync(int studentId)
        {
            return await _context.Classes
                .Include(c => c.Subject)
                .AsNoTracking()
                .Where(c => c.ClassMembers.Any(cm => cm.StudentId == studentId))
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

        public async Task<bool> IsTeacherOfClassAsync(int classId, int userId)
        {
            return await _context.Classes.AnyAsync(c => c.ClassId == classId && c.TeacherId == userId);
        }
    }
}