using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TokiPonaQuiz.Models
{
    public class Vocabulary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string TokiPonaWord { get; set; }

        [Required]
        [MaxLength(200)]
        public string GermanTranslation { get; set; }

        public string[] OptionsArray { get; set; } 
 
        [NotMapped]
        public List<string> Options
        {
            get => OptionsArray?.ToList() ?? new List<string>();
            set => OptionsArray = value?.ToArray();
        }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
