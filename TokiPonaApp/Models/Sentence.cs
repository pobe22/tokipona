namespace TokiPonaQuiz.Models
{
    public class Sentence
    {
        public int Id { get; set; }
        public string TokiPonaSentence { get; set; }
        public string GermanSentence { get; set; }
        public List<string> Options { get; set; }
        public List<string> WordPool { get; set; }
    }
}
