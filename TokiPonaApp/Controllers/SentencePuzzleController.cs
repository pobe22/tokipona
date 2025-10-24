using Microsoft.AspNetCore.Mvc;
using TokiPonaQuiz.Models;
using System.Text.Json;

namespace TokiPonaQuiz.Controllers
{
    public class SentencePuzzleController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public SentencePuzzleController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPuzzleData()
        {
            var jsonPath = Path.Combine(_env.WebRootPath, "data", "beispielsaetze.json");
            var jsonData = System.IO.File.ReadAllText(jsonPath);
            var sentences = JsonSerializer.Deserialize<List<Sentence>>(jsonData);
            return Json(sentences);
        }

        [HttpPost]
        public IActionResult ValidateAnswer([FromBody] PuzzleAnswer answer)
        {
            // Logic to validate the puzzle answer
            var isCorrect = ValidatePuzzle(answer);
            return Json(new { correct = isCorrect });
        }

        private bool ValidatePuzzle(PuzzleAnswer answer)
        {
            // Validation logic
            return true;
        }
    }

    public class PuzzleAnswer
    {
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
