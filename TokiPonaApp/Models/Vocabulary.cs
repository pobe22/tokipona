namespace TokiPonaQuiz.Models
{
    public class Vocabulary
    {
        public int Id { get; set; }
        public string TokiPonaWord { get; set; }
        public string GermanTranslation { get; set; }
        public List<string> Options { get; set; }
    }
}
