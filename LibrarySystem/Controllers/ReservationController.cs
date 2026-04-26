using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    public class ReservationController : Controller
    {
        // GET: Reservation
        public IActionResult Index()
        {
            return View();
        }
    }
}