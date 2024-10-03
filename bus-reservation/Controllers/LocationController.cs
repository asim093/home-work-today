using bus_reservation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bus_reservation.Controllers
{
    public class LocationController : Controller
    {

        private readonly BusReservationSystemContext db;

        public LocationController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var Location = db.Locations.ToList(); // Ensure this is not null
            return View(Location);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Location location)
        {
            // Check if the location name already exists
            bool locationExists = db.Locations.Any(l => l.LocationName == location.LocationName);

            if (locationExists)
            {
                // Add a model error if the location name already exists
                ModelState.AddModelError("LocationName", "Location name already exists.");

                // Return the view with the model to display validation errors
                return View(location);
            }

            if (ModelState.IsValid)
            {
                db.Locations.Add(location);
                db.SaveChanges();

                TempData["SuccessCreateMessage"] = "Location Added Successfully!";
                return RedirectToAction("Index");
            }

            // If the model state is invalid, return the view with errors
            return View(location);
        }


        public IActionResult Edit(int? id)
        {
            var Location = db.Locations.Find(id);
            return View(Location);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Location location)
        {
            // Check if the new name already exists in the database, excluding the current location being edited
            bool locationExists = db.Locations.Any(l => l.LocationName == location.LocationName && l.LocationId != location.LocationId);

            if (locationExists)
            {
                // Add a model error if the location name already exists (excluding the current location)
                ModelState.AddModelError("Name", "Location name already exists.");

                // Return the view with the model to display validation errors
                return View(location);
            }

            if (ModelState.IsValid)
            {
                db.Locations.Update(location);
                db.SaveChanges();

                TempData["SuccessEditMessage"] = "Location Edited Successfully!";
                return RedirectToAction("Index");
            }

            // If the model state is invalid, return the view with errors
            return View(location);
        }

    }
}
