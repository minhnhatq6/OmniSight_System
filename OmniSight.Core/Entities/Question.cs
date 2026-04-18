using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    [Table("CauHoi")]
    public class Question
    {
        [Key]
        [Column("MaCauHoi")]
        public int QuestionId { get; set; }

        [Column("MaDeThi")]
        public int ExamId { get; set; }

        [Column("NoiDung")]
        public string Content { get; set; }

        [Column("DapAnA")]
        public string OptionA { get; set; }

        [Column("DapAnB")]
        public string OptionB { get; set; }

        [Column("DapAnC")]
        public string OptionC { get; set; }

        [Column("DapAnD")]
        public string OptionD { get; set; }

        [MaxLength(1)]
        [Column("DapAnDung")]
        public string CorrectOption { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }
    }
}