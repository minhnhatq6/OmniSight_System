using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OmniSight.Core.Entities;
using OmniSight.Data;
using Xceed.Words.NET;

namespace OmniSight.Services
{
    public class ExamService
    {
        private readonly OmniSightDbContext _context;

        public ExamService(OmniSightDbContext context)
        {
            _context = context;
        }

        public async Task<Exam> CreateExamAsync(int classId, string title, int durationMinutes)
        {
            var exam = new Exam
            {
                ClassId = classId,
                Title = title,
                DurationMinutes = durationMinutes,
                CreatedAt = DateTime.Now
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();
            return exam;
        }

        public async Task<List<Exam>> GetExamsByClassIdAsync(int classId)
        {
            return await _context.Exams
                .Include(e => e.Questions)
                .Include(e => e.ExamResults)
                .Where(e => e.ClassId == classId) // Lọc chính xác theo ID lớp học
                .ToListAsync();
        }

        public async Task<List<Exam>> GetExamsForTeacherAsync(int teacherId)
        {
            return await _context.Exams
                 // <--- THÊM DÒNG NÀY VÀO TẤT CẢ CÁC HÀM GET
                .Include(e => e.Class)
                .Include(e => e.Questions)
                .Include(e => e.ExamResults)
                .Where(e => e.Class.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<List<Exam>> GetExamsForStudentAsync(int studentId)
        {
            var classIds = await _context.ClassMembers
                .Where(m => m.StudentId == studentId)
                .Select(m => m.ClassId)
                .ToListAsync();

            return await _context.Exams
                .Where(e => classIds.Contains(e.ClassId))
                .ToListAsync();
        }

        public async Task<ExamResult> StartExamAsync(int examId, int studentId)
        {
            var existingResult = await _context.ExamResults.FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentId == studentId);

            if (existingResult != null)
            {
                if (existingResult.RetakeStatus == "Approved")
                {
                    // NẾU ĐƯỢC DUYỆT: Xóa trắng điểm cũ, đáp án cũ, cho thi lại
                    existingResult.Score = null;
                    existingResult.AnswersData = null;
                    existingResult.CompletedAt = null;
                    existingResult.StartedAt = DateTime.Now;
                    existingResult.RetakeStatus = "Retaken"; // Đánh dấu là đang làm lại

                    // Tùy chọn: Xóa cả log vi phạm cũ
                    var oldLogs = _context.ViolationLogs.Where(v => v.ResultId == existingResult.ResultId);
                    _context.ViolationLogs.RemoveRange(oldLogs);

                    await _context.SaveChangesAsync();
                    return existingResult;
                }

                if (existingResult.CompletedAt != null) throw new Exception("Bạn đã nộp bài thi này rồi!");
                return existingResult;
            }

            var newResult = new ExamResult { ExamId = examId, StudentId = studentId, StartedAt = DateTime.Now };
            _context.ExamResults.Add(newResult);
            await _context.SaveChangesAsync();
            return newResult;
        }

        public async Task<List<Question>> GetQuestionsByExamIdAsync(int examId)
        {
            return await _context.Questions
                .Where(q => q.ExamId == examId)
                .ToListAsync();
        }

        public async Task<ExamResult> UpdateExamResultAsync(ExamResult examResult)
        {
            _context.ExamResults.Update(examResult);
            await _context.SaveChangesAsync();
            return examResult;
        }

        public async Task UpdateExamAsync(Exam exam)
        {
            // 1. Kiểm tra xem trong bộ nhớ đệm (Local) của EF có bản ghi nào trùng ID đang kẹt không
            var trackedEntity = _context.Exams.Local.FirstOrDefault(e => e.ExamId == exam.ExamId);

            if (trackedEntity != null)
            {
                // 2. Nếu có, lệnh cho EF ngừng theo dõi bản ghi cũ đó (Detach)
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            // 3. Bây giờ mới thực hiện cập nhật bản mới
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExamAsync(int examId)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Question> CreateQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ExamResult>> GetExamResultsAsync(int examId)
        {
            return await _context.ExamResults
                .Where(r => r.ExamId == examId)
                .Include(r => r.Student)
                .Include(r => r.ViolationLogs)
                .ToListAsync();
        }

        public async Task<Exam> ImportExamFromWordAsync(int classId, string title, int duration, string filePath)
        {
            var exam = new Exam
            {
                ClassId = classId,
                Title = title,
                DurationMinutes = duration,
                CreatedAt = DateTime.Now,
                Questions = new List<Question>()
            };

            using (DocX document = DocX.Load(filePath))
            {
                Question currentQuestion = null;

                foreach (var para in document.Paragraphs)
                {
                    string text = para.Text.Trim();
                    if (string.IsNullOrEmpty(text)) continue;

                    if (text.StartsWith("Câu", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentQuestion != null) exam.Questions.Add(currentQuestion);
                        currentQuestion = new Question { Content = text };
                    }
                    else if (text.StartsWith("A.", StringComparison.OrdinalIgnoreCase)) currentQuestion.OptionA = text.Substring(2).Trim();
                    else if (text.StartsWith("B.", StringComparison.OrdinalIgnoreCase)) currentQuestion.OptionB = text.Substring(2).Trim();
                    else if (text.StartsWith("C.", StringComparison.OrdinalIgnoreCase)) currentQuestion.OptionC = text.Substring(2).Trim();
                    else if (text.StartsWith("D.", StringComparison.OrdinalIgnoreCase)) currentQuestion.OptionD = text.Substring(2).Trim();
                    else if (text.StartsWith("Đáp án:", StringComparison.OrdinalIgnoreCase))
                    {
                        currentQuestion.CorrectOption = text.Replace("Đáp án:", "").Trim().ToUpper();
                    }
                }
                if (currentQuestion != null) exam.Questions.Add(currentQuestion);
            }

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();
            return exam;
        }
        // Trong file ExamService.cs
        public async Task<ExamResult> GetExamResultByIdAsync(int resultId)
        {
            // Include(er => er.Student) để lấy kèm thông tin học sinh
            return await _context.ExamResults
                .Include(er => er.Student)
                .FirstOrDefaultAsync(er => er.ResultId == resultId);
        }
        // 1. Hàm cho Học sinh xin làm lại
        public async Task RequestRetakeAsync(int examId, int studentId)
        {
            var result = await _context.ExamResults.FirstOrDefaultAsync(e => e.ExamId == examId && e.StudentId == studentId);
            if (result != null)
            {
                result.RetakeStatus = "Pending"; // Đặt trạng thái đang chờ duyệt
                await _context.SaveChangesAsync();
            }
        }
        // ExamService.cs - thêm hàm này
        public async Task<int> GetViolationCountAsync(int resultId)
        {
            return await _context.ViolationLogs  // Đổi _db thành _context
                .Where(v => v.ResultId == resultId)
                .CountAsync();
        }
        // 2. Hàm cho Giáo viên duyệt làm lại
        public async Task ApproveRetakeAsync(int resultId, bool isApproved)
        {
            var result = await _context.ExamResults.FindAsync(resultId);
            if (result != null)
            {
                result.RetakeStatus = isApproved ? "Approved" : "Rejected";
                await _context.SaveChangesAsync();
            }
        }
    }
}
