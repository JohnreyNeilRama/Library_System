using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        // GET: Admin/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Admin/AllBooks
        public IActionResult AllBooks()
        {
            return View();
        }

        // GET: Admin/ManageBooks
        public IActionResult ManageBooks()
        {
            return View();
        }

        // GET: Admin/ReturnedBooks
        public IActionResult ReturnedBooks()
        {
            return View();
        }

        // GET: Admin/TransactionHistory
        public IActionResult TransactionHistory()
        {
            return View();
        }

        // GET: Admin/Categories
        public IActionResult Categories()
        {
            return View();
        }

        // GET: Admin/ManageUsers
        public IActionResult ManageUsers()
        {
            return View();
        }

        // GET: Admin/Reports
        public IActionResult Reports()
        {
            return View();
        }

        // GET: Admin/Reservations
        public IActionResult Reservations()
        {
            return View();
        }
    }
}