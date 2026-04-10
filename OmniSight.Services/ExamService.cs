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
                .Where(e => e.ClassId == classId)
                .ToListAsync();
        }

        public async Task<List<Exam>> GetExamsForTeacherAsync(int teacherId)
        {
            var classIds = await _context.Classes
                .Where(c => c.TeacherId == teacherId)
                .Select(c => c.ClassId)
                .ToListAsync();

            return await _context.Exams
                .Where(e => classIds.Contains(e.ClassId))
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
            var result = new ExamResult
            {
                ExamId = examId,
                StudentId = studentId,
                StartedAt = DateTime.Now
            };

            _context.ExamResults.Add(result);
            await _context.SaveChangesAsync();
            return result;
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

        public async Task<Exam> UpdateExamAsync(Exam exam)
        {
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
            return exam;
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
    }
}
