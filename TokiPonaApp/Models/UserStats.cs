namespace TokiPonaQuiz.Models
{
    public class UserStats
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public double Accuracy { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
    }
}
