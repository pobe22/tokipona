using Microsoft.AspNetCore.Mvc;
using TokiPonaQuiz.Models;
using System.Text.Json;
using TokiPonaQuiz.ViewModels;

namespace TokiPonaQuiz.Controllers
{
    public class SentenceExerciseController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public SentenceExerciseController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            var model = new QuizViewModel
            {
                IsLoggedIn = HttpContext.Session.GetString("Username") != null,
                Username = HttpContext.Session.GetString("Username")
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult GetSentences()
        {
            var jsonPath = Path.Combine(_env.WebRootPath, "data", "beispielsaetze.json");
            var jsonData = System.IO.File.ReadAllText(jsonPath);
            var sentences = JsonSerializer.Deserialize<List<Sentence>>(jsonData);
            return Json(sentences);
        }

        [HttpPost]
        public IActionResult CheckAnswer([FromBody] AnswerSubmission submission)
        {
            var user = HttpContext.Session.GetString("Username");
            if (user != null)
            {
                UpdateUserStats(user, submission.IsCorrect);
            }
            return Json(new { success = true });
        }

        private void UpdateUserStats(string username, bool isCorrect)
        {
            // Logic to update user statistics
        }
    }
}
