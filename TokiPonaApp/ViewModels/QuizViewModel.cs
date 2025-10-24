namespace TokiPonaQuiz.ViewModels
{
    public class QuizViewModel
    {
        public int CurrentQuestionIndex { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string Feedback { get; set; }
        public bool IsLoggedIn { get; set; }
        public string Username { get; set; }
    }
}
