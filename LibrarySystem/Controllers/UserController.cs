using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}