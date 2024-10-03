using bus_reservation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Controllers
{
    public class Bus_TypeController : Controller
    {
        private readonly BusReservationSystemContext db;

        public Bus_TypeController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var Bustypes = db.BusTypes.ToList(); // Ensure this is not null
            return View(Bustypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BusType Busname)
        {

            db.BusTypes.Add(Busname);
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult Edit(int? id)
        {
            var bustype = db.BusTypes.Find(id);
            return View(bustype);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BusType Busname)
        {

            db.BusTypes.Update(Busname);
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult Delete(int? id)
        {
            var bustype = db.BusTypes.Find(id);
            db.BusTypes.Remove(bustype);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
