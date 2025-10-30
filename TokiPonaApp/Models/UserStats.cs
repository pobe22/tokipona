using System.ComponentModel.DataAnnotations;

namespace TokiPonaQuiz.Models
{
    public class UserStats
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        public double Accuracy { get; set; }

        public int TotalQuestions { get; set; }

        public int CorrectAnswers { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
