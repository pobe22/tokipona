using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TokiPonaQuiz.Models
{
    public class Sentence
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TokiPonaSentence { get; set; }

        [Required]
        public string GermanSentence { get; set; }

        [Required]
        public string Difficulty { get; set; } 

        public string WordPoolJson { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UsageCount { get; set; } = 0; 

        [NotMapped]
        public List<string> WordPool
        {
            get => string.IsNullOrEmpty(WordPoolJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(WordPoolJson);
            set => WordPoolJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public List<string> Options { get; set; } 
    }
}
