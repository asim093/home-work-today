using bus_reservation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace bus_reservation.Controllers
{
    public class HomeController : Controller
    {
        //private readonly AptechVisionProjectContext db;

        //public HomeController(AptechVisionProjectContext _db)
        //{
        //    db = _db;
        //}
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Faqs()
        {
            return View();
        }

        
    }
}
