using Microsoft.AspNetCore.Mvc;

namespace TokiPonaQuiz.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public IActionResult Login([FromForm] string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                HttpContext.Session.SetString("Username", username);
                return Json(new { success = true, username });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Json(new { success = true });
        }
    }
}
