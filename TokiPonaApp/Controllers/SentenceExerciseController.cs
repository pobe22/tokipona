using Microsoft.AspNetCore.Mvc;
using TokiPonaQuiz.Models;
using TokiPonaQuiz.ViewModels;
using TokiPonaQuiz.Services;

namespace TokiPonaQuiz.Controllers
{
    public class SentenceExerciseController : Controller
    {
        private readonly PerplexityService _perplexityService;

        public SentenceExerciseController(PerplexityService perplexityService)
        {
            _perplexityService = perplexityService;
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
        public async Task<IActionResult> GenerateSentence([FromQuery] string difficulty = "beginner")
        {
            try
            {
                var tokiPonaSentence = await _perplexityService.GenerateTokiPonaSentence(difficulty);
                var translation = await _perplexityService.TranslateSentence(tokiPonaSentence);
                var wordPool = await _perplexityService.GenerateWordPool(tokiPonaSentence);

                var sentence = new Sentence
                {
                    Id = new Random().Next(1000, 9999),
                    TokiPonaSentence = translation.TokiPonaSentence,
                    GermanSentence = translation.GermanTranslation,
                    WordPool = wordPool
                };

                return Json(sentence);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckAnswer([FromBody] AnswerSubmission submission)
        {
            try
            {
                var validation = await _perplexityService.ValidateSentence(
                    submission.UserAnswer,
                    submission.CorrectAnswer
                );

                var user = HttpContext.Session.GetString("Username");
                if (user != null)
                {
                    UpdateUserStats(user, validation.IsCorrect);
                }

                return Json(new
                {
                    success = true,
                    isCorrect = validation.IsCorrect,
                    feedback = validation.Feedback
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private void UpdateUserStats(string username, bool isCorrect)
        {
            // Ihre Statistik-Logik
        }
    }

    public class AnswerSubmission
    {
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
