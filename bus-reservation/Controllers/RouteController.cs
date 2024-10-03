using bus_reservation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Controllers
{
    public class RouteController : Controller
    {
        private readonly BusReservationSystemContext db;

        public RouteController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var routes = db.Routes
                   .Include(r => r.StartingPlaceNavigation)  // Eagerly load StartingPlaceNavigation (Location)
                   .Include(r => r.DestinationPlaceNavigation)  // Eagerly load DestinationPlaceNavigation (Location)
                   .ToList();
            return View(routes);
        }

        public IActionResult Create()
        {

            var locations = db.Locations.ToList();

            ViewBag.StartingPlace = new SelectList(locations, "LocationId", "LocationName");
            ViewBag.DestinationPlace = new SelectList(locations, "LocationId", "LocationName");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(bus_reservation.Models.Route route)
        {
            //if (!ModelState.IsValid)
            //{
            //    // In case of error, reload the locations for the select options
            //    var locations = db.Locations.ToList();
            //    ViewBag.StartingPlace = new SelectList(locations, "LocationId", "LocationName");
            //    ViewBag.DestinationPlace = new SelectList(locations, "LocationId", "LocationName");

            //    return View(route);
                
            //}
            db.Routes.Add(route);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
