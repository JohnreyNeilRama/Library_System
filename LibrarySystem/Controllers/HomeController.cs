using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibrarySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly Services.ILibraryService _libraryService;

        public HomeController(Services.ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewBag.BookCount = _libraryService.GetAllBooks().Count;
            ViewBag.UserCount = _libraryService.GetAllUsers().Count;
            ViewBag.TransactionCount = _libraryService.GetAllTransactions().Count;
            ViewBag.CategoryCount = _libraryService.GetAllCategories().Count;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
