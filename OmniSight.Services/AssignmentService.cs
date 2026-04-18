using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OmniSight.Core.Entities;
using OmniSight.Data;

namespace OmniSight.Services
{
    public class AssignmentService
    {
        private readonly OmniSightDbContext _context;

        public AssignmentService(OmniSightDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách bài tập của một lớp
        public async Task<List<Assignment>> GetAssignmentsByClassIdAsync(int classId)
        {
            return await _context.Assignments
                .Include(a => a.Creator)
                .Include(a => a.Submissions)
                .Where(a => a.ClassId == classId)
                .OrderByDescending(a => a.DueDate)
                .ToListAsync();
        }

        // Tạo bài tập mới (chỉ giáo viên)
        public async Task<Assignment> CreateAssignmentAsync(int classId, int createdBy, string title, string description, string attachmentUrl, DateTime? dueDate)
        {
            var assignment = new Assignment
            {
                ClassId = classId,
                CreatedBy = createdBy,
                Title = title,
                Description = description,
                AttachmentUrl = attachmentUrl,
                DueDate = dueDate
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        // Lấy thông tin chi tiết bài tập
        public async Task<Assignment> GetAssignmentByIdAsync(int assignmentId)
        {
            return await _context.Assignments
                .Include(a => a.Creator)
                .Include(a => a.Submissions)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
        }

        // Xoá bài tập (chỉ người tạo)
        public async Task<bool> DeleteAssignmentAsync(int assignmentId)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentId);
            if (assignment == null) return false;

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }

        // Nộp bài tập (học sinh)
        public async Task<Submission> SubmitAssignmentAsync(int assignmentId, int studentId, string fileOrLinkUrl)
        {
            // Kiểm tra xem học sinh đã nộp bài này chưa
            var existingSubmission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

            if (existingSubmission != null)
            {
                // Cập nhật lại nộp bài
                existingSubmission.FileOrLinkUrl = fileOrLinkUrl;
                existingSubmission.SubmittedAt = DateTime.Now;
                _context.Submissions.Update(existingSubmission);
            }
            else
            {
                // Tạo mới nộp bài
                var submission = new Submission
                {
                    AssignmentId = assignmentId,
                    StudentId = studentId,
                    FileOrLinkUrl = fileOrLinkUrl,
                    SubmittedAt = DateTime.Now
                };
                _context.Submissions.Add(submission);
            }

            await _context.SaveChangesAsync();
            return existingSubmission ?? new Submission { AssignmentId = assignmentId, StudentId = studentId };
        }

        // Lấy danh sách nộp bài của một bài tập
        public async Task<List<Submission>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .Where(s => s.AssignmentId == assignmentId)
                .OrderBy(s => s.SubmittedAt)
                .ToListAsync();
        }

        // Chấm điểm bài tập (giáo viên)
        public async Task<Submission> GradeSubmissionAsync(int submissionId, float score, string feedback)
        {
            var submission = await _context.Submissions.FindAsync(submissionId);
            if (submission == null) return null;

            submission.Score = score;
            submission.Feedback = feedback;
            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync();

            return submission;
        }
        public async Task UpdateAssignmentAsync(int id, string title, string desc, string url, DateTime? dueDate)
        {
            var asm = await _context.Assignments.FindAsync(id);
            if (asm != null)
            {
                asm.Title = title;
                asm.Description = desc;
                asm.AttachmentUrl = url;
                asm.DueDate = dueDate;
                await _context.SaveChangesAsync();
            }
        }
    }
}
