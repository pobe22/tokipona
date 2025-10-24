using Microsoft.AspNetCore.Mvc;
using TokiPonaQuiz.Models;

namespace TokiPonaQuiz.Controllers
{
    public class StatsController : Controller
    {
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");

            if (username == null)
            {
                return View(new UserStats());
            }

            // Retrieve user stats from database or session
            var stats = GetUserStats(username);
            return View(stats);
        }

        private UserStats GetUserStats(string username)
        {
            // Logic to retrieve user statistics
            // For now, return mock data
            return new UserStats
            {
                Username = username,
                Accuracy = 0.85,
                TotalQuestions = 50,
                CorrectAnswers = 42
            };
        }
    }
}
