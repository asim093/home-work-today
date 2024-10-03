using bus_reservation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bus_reservation.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeDashboardController : Controller
    {
        
        private readonly BusReservationSystemContext db;

        public EmployeeDashboardController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
