using Microsoft.AspNetCore.Mvc;
using TokiPonaQuiz.Models;
using System.Text.Json;
using TokiPonaQuiz.ViewModels;

namespace TokiPonaQuiz.Controllers
{
    public class VocabTrainerController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public VocabTrainerController(IWebHostEnvironment env)
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
        public IActionResult GetVocabulary()
        {
            var jsonPath = Path.Combine(_env.WebRootPath, "data", "vokabeln.json");
            var jsonData = System.IO.File.ReadAllText(jsonPath);
            var vocab = JsonSerializer.Deserialize<List<Vocabulary>>(jsonData);
            return Json(vocab);
        }

        [HttpPost]
        public IActionResult SubmitAnswer([FromBody] AnswerSubmission submission)
        {
            var user = HttpContext.Session.GetString("Username");
            if (user != null)
            {
                // Update user statistics
                UpdateUserStats(user, submission.IsCorrect);
            }
            return Json(new { success = true });
        }

        private void UpdateUserStats(string username, bool isCorrect)
        {
            // Logic to update user statistics in database or session
        }
    }

    public class AnswerSubmission
    {
        public bool IsCorrect { get; set; }
    }
}
