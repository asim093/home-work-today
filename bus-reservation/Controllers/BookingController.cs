using bus_reservation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bus_reservation.Controllers
{
    public class BookingController : Controller
    {

        private readonly BusReservationSystemContext db;

        public BookingController(BusReservationSystemContext _db)
        {
            db = _db;
        }
        public async Task<IActionResult> Index()
        {
            var bookings = await db.Bookings
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.StartingPlaceNavigation) // Include Starting Location
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.DestinationPlaceNavigation) // Include Destination Location
                .Include(b => b.Employee)
                .Include(b => b.Branch)
                .Include(b => b.Admin)
                .Select(b => new BookingViewModel
                {
                    BookingId = b.BookingId,
                    CustomerName = b.CustomerName,
                    CustomerAge = b.CustomerAge,
                    CustomerContact = b.CustomerContact,
                    CustomerEmail = b.CustomerEmail ?? "N/A",
                    BookingDate = b.BookingDate,
                    SeatNumber = b.SeatNumber,
                    Fare = b.Fare,
                    StatusBooking = b.StatusBooking,
                    CreatedAt = b.CreatedAt,
                    BusCode = b.Bus.BusNumber,
                    BusTypeName = b.Bus.BusType.BusTypeName,
                    RouteName = b.Bus.Route.RouteName,
                    StartingPlace = b.Bus.Route.StartingPlaceNavigation.LocationName, // Fetch starting place name
                    DestinationPlace = b.Bus.Route.DestinationPlaceNavigation.LocationName, // Fetch destination place name
                    Distance = b.Bus.Route.Distance,
                    BookingManagedBy = b.EmployeeId != null && b.BranchId != null ?
                                       $"Employee Name: {b.Employee.Name} | Branch Code: {b.Branch.BranchCode}" :
                                       (b.AdminId != null ? "Admin" : "N/A")
                })
                .ToListAsync();

            return View(bookings);
        }

        public IActionResult Create()
        {
            var buses = (from b in db.Buses
                         join bt in db.BusTypes on b.BusTypeId equals bt.BusTypeId
                         join r in db.Routes on b.RouteId equals r.RouteId
                         join startLoc in db.Locations on r.StartingPlace equals startLoc.LocationId
                         join destLoc in db.Locations on r.DestinationPlace equals destLoc.LocationId
                         where b.AvailableSeats > 0
                         select new
                         {
                             b.BusId,
                             BusDetails = $"{b.BusNumber} - {bt.BusTypeName} - {r.RouteName} ({startLoc.LocationName} to {destLoc.LocationName}) - " +
                                          $"{b.DepartureTime.ToString("hh:mm tt")} to {b.ArrivalTime.ToString("hh:mm tt")} - Available Seats: {b.AvailableSeats}"
                         })
                         .ToList();

            ViewBag.BusId = new SelectList(buses, "BusId", "BusDetails");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            // Get the currently logged-in user's role from the claims
            var role = User.FindFirstValue(ClaimTypes.Role); // This will give "Admin" or "Employee"
            var employeeId = User.FindFirstValue("EmployeeID");
            var adminId = User.FindFirstValue("AdminID");
            var branchId = User.FindFirstValue("BranchID");

            // Set the status and booking date (optional as default is set in DB)
            booking.StatusBooking = "Confirmed";
            booking.BookingDate = DateTime.Now;

            // Check if the model is valid
            //if (ModelState.IsValid)
            //{
                // Get the bus details from the database
                var bus = db.Buses.FirstOrDefault(b => b.BusId == booking.BusId);
                if (bus != null)
                {
                    // Check if there are available seats
                    if (bus.AvailableSeats > 0)
                    {
                        // Assign the next available seat number to the booking
                        booking.SeatNumber = bus.TotalSeats - bus.AvailableSeats + 1;

                        // Update the bus's available seats
                        bus.AvailableSeats--;

                        // Calculate the fare
                        var route = db.Routes.FirstOrDefault(r => r.RouteId == bus.RouteId);
                        if (route != null)
                        {
                            // Fetch distance from the route
                            var distance = route.Distance; // Assuming 'Distance' is a property in the Route table

                            // Fetch bus type
                            var busType = db.BusTypes.FirstOrDefault(bt => bt.BusTypeId == bus.BusTypeId);
                            if (busType != null)
                            {
                                // Calculate fare based on bus type
                                decimal farePerKm = 2; // Base fare for Express
                                switch (busType.BusTypeName)
                                {
                                    case "Luxury":
                                        farePerKm = 5; // Add suitable fare for Luxury
                                        break;
                                    case "Volvo (Non A/C)":
                                        farePerKm = 7; // Add suitable fare for Volvo (Non A/C)
                                        break;
                                    case "Volvo (A/C)":
                                        farePerKm = 12; // Add suitable fare for Volvo (A/C)
                                        break;
                                }

                                // Calculate total fare based on distance
                                decimal totalFare = farePerKm * distance;

                                // Adjust total fare based on customer's age
                                if (booking.CustomerAge <= 5)
                                {
                                    totalFare = 0; // No charge for age <= 5
                                }
                                else if (booking.CustomerAge > 5 && booking.CustomerAge <= 12)
                                {
                                    totalFare *= 0.5m; // 50% charge for age 5 to 12
                                }
                                else if (booking.CustomerAge > 50)
                                {
                                    totalFare *= 0.7m; // 30% discount for age > 50
                                }

                                // Set the calculated fare to the booking
                                booking.Fare = (int)totalFare; // Assuming Fare is an INT type in Booking table

                                // Set Employee/Branch/Admin ID based on the role
                                if (role == "Employee")
                                {
                                    booking.BranchId = branchId != null ? int.Parse(branchId) : (int?)null;
                                    booking.EmployeeId = employeeId != null ? int.Parse(employeeId) : (int?)null;
                                }
                                else if (role == "Admin")
                                {
                                    booking.AdminId = adminId != null ? int.Parse(adminId) : (int?)null;
                                }

                                // Save the booking to the database
                                db.Bookings.Add(booking);
                                db.SaveChanges();

                            return RedirectToAction("Ticket", new { id = booking.BookingId }); 
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Route not found.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "No available seats on this bus.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Bus not found.");
                }
            //}

            // If there's an issue with the model, return the same view to fix errors
            return View(booking);
        }

        public async Task<IActionResult> Ticket(int id)
        {
            var booking = await db.Bookings
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.StartingPlaceNavigation) // Include Starting Location
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.Route)
                .ThenInclude(route => route.DestinationPlaceNavigation) // Include Destination Location
                .Include(b => b.Bus)
                .ThenInclude(bus => bus.BusType) // Include Bus Type
                .Include(b => b.Employee)
                .Include(b => b.Branch)
                .Include(b => b.Admin)
                .Select(b => new BookingViewModel
                {
                    BookingId = b.BookingId,
                    CustomerName = b.CustomerName,
                    CustomerAge = b.CustomerAge,
                    CustomerContact = b.CustomerContact,
                    CustomerEmail = b.CustomerEmail ?? "N/A", // Show "N/A" if email is null
                    BookingDate = b.BookingDate,
                    SeatNumber = b.SeatNumber,
                    Fare = b.Fare,
                    StatusBooking = b.StatusBooking,
                    CreatedAt = b.CreatedAt,
                    BusCode = b.Bus.BusNumber,
                    BusTypeName = b.Bus.BusType.BusTypeName,
                    RouteName = b.Bus.Route.RouteName,
                    StartingPlace = b.Bus.Route.StartingPlaceNavigation.LocationName, // Fetch starting place name
                    DestinationPlace = b.Bus.Route.DestinationPlaceNavigation.LocationName, // Fetch destination place name
                    Distance = b.Bus.Route.Distance,
                    BookingManagedBy = b.EmployeeId != null && b.BranchId != null ?
                                       $"Employee Name: {b.Employee.Name} | Branch Code: {b.Branch.BranchCode}" :
                                       (b.AdminId != null ? "Admin" : "N/A")
                })
                .FirstOrDefaultAsync(b => b.BookingId == id);


            return View(booking); // Return the booking details to the Ticket view
        }




    }
}
