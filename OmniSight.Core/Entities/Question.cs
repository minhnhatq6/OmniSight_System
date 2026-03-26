using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        public int ExamId { get; set; }
        public string Content { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        [MaxLength(1)]
        public string CorrectOption { get; set; }

        // Navigation properties
        public virtual Exam Exam { get; set; }
    }
}
