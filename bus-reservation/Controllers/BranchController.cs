using bus_reservation.Models;
using Microsoft.AspNetCore.Mvc;

namespace bus_reservation.Controllers
{
    public class BranchController : Controller
    {
        private readonly BusReservationSystemContext db;

        public BranchController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var Branches = db.Branches.ToList();
            return View(Branches);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Branch branch)
        {

            var duplicateZip = db.Branches.Any(b => b.ZipCode == branch.ZipCode);
            if (duplicateZip)
            {
                ModelState.AddModelError("ZipCode", "A branch with this Zip Code already exists.");
            }

            var duplicateContact = db.Branches.Any(b => b.Contact == branch.Contact);
            if (duplicateContact)
            {
                ModelState.AddModelError("Contact", "A branch with this Contact number already exists.");
            }

            var duplicateAdddress = db.Branches.Any(b => b.Address == branch.Address);
            if (duplicateAdddress)
            {
                ModelState.AddModelError("Address", "A branch with this Address already exists.");
            }

            if (!string.IsNullOrWhiteSpace(branch.Contact) &&
                !System.Text.RegularExpressions.Regex.IsMatch(branch.Contact, @"^0\d{10}$"))
            {
                ModelState.AddModelError("Contact", "The Contact number must be exactly 11 digits and start with '0'.");
            }

            if (!ModelState.IsValid)
            {
                return View(branch);
            }

            branch.BranchCode = GenerateUniqueBranchCode();

            db.Branches.Add(branch);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int? id)
        {
            var branch = db.Branches.Find(id);
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Branch branch)
        {

            var duplicateZip = db.Branches.Any(b => b.ZipCode == branch.ZipCode && b.Id != branch.Id);
            if (duplicateZip)
            {
                ModelState.AddModelError("ZipCode", "A branch with this Zip Code already exists.");
            }

            var duplicateContact = db.Branches.Any(b => b.Contact == branch.Contact && b.Id != branch.Id);
            if (duplicateContact)
            {
                ModelState.AddModelError("Contact", "A branch with this Contact number already exists.");
            }

            var duplicateAdddress = db.Branches.Any(b => b.Address == branch.Address && b.Id != branch.Id);
            if (duplicateAdddress)
            {
                ModelState.AddModelError("Address", "A branch with this Address already exists.");
            }

            if (!string.IsNullOrWhiteSpace(branch.Contact) &&
                 !System.Text.RegularExpressions.Regex.IsMatch(branch.Contact, @"^0\d{10}$"))
            {
                ModelState.AddModelError("Contact", "The Contact number must be exactly 11 digits and start with '0'.");
            }

            if (!ModelState.IsValid)
            {
                return View(branch);
            }

            db.Branches.Update(branch);
            db.SaveChanges();

            return RedirectToAction("Index");
        }






        private string GenerateUniqueBranchCode()
        {
            string branchCode;
            Random random = new Random();
            do
            {
                branchCode = random.Next(10000000, 99999999).ToString();
            }
            while (db.Branches.Any(b => b.BranchCode == branchCode));

            return branchCode;
        }
    }
}
