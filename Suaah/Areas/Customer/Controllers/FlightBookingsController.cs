#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Suaah.Data;
using Suaah.Models;

namespace Suaah.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class FlightBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;


        public FlightBookingsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: FlightBookings
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? airline, string? order, string? ordersort, DateTime? timef, DateTime? timet, string? conf, string? cont,int? clas,double? min,double? max)
        {
            List<Flight> flights=null;
            List<string> names = await _context.Countries.Select(c => c.Name).ToListAsync();
            names.AddRange(await _context.Airports.Select(f => f.City).Distinct().ToListAsync());
            ViewBag.names = names;
            List<FlightClass> classes = await _context.FlightClassss.ToListAsync();
            List<Airline> airlines = await _context.Airlines.ToListAsync();
            bool classavtive = false;
            ViewBag.min = min;
            ViewBag.max = max;
            if (timef != null)
                ViewBag.timef = timef.GetValueOrDefault().ToString("yyyy-MM-dd");
            if (timet != null)
                ViewBag.timet = timet.GetValueOrDefault().ToString("yyyy-MM-dd");
            if(string.IsNullOrEmpty(airline))
                ViewBag.airlines = new SelectList(airlines,"Code", "Name");
            else
                ViewBag.airlines = new SelectList(airlines, "Code", "Name",airline);
            if (clas == null || clas == 0)
                ViewBag.classes = new SelectList(classes, "ID", "Class");
            else
            {
                ViewBag.classes = new SelectList(classes, "ID", "Class", clas);
                classavtive = true;
            }

            ViewBag.conf = conf;
            ViewBag.cont = cont;

            if (!string.IsNullOrEmpty(order))
                ViewBag.order = order;
            if(!string.IsNullOrEmpty(airline))
                flights = await _context.Flights
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).Where(f=> f.LeaveTime>DateTime.Now&& f.AirlineCode==airline).ToListAsync();
            if(classavtive)
            {
                if (flights == null)
                {
                    if(min!=null&& max!=null)
                    {
                        flights = await _context.Flights
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).Where(f => f.LeaveTime > DateTime.Now && f.FlightClasses.Any(a=>a.Price>=min&&a.Price<max && a.FlightClassId==clas)).ToListAsync();
                    }
                    else if (min == null && max != null)
                    {
                        flights = await _context.Flights
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).Where(f => f.LeaveTime > DateTime.Now && f.FlightClasses.Any(a => a.Price < max && a.FlightClassId == clas)).ToListAsync();
                    }
                    else if (min != null && max == null)
                    {
                        flights = await _context.Flights
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).Where(f => f.LeaveTime > DateTime.Now && f.FlightClasses.Any(a => a.Price >=min && a.FlightClassId == clas)).ToListAsync();
                    }

                }
                else
                {
                    if (min != null && max != null)
                    {
                        flights =  flights.Where(f => f.FlightClasses.Any(a => a.Price >= min && a.Price < max && a.FlightClassId == clas)).ToList();
                    }
                    else if (min == null && max != null)
                    {
                        flights = flights.Where(f => f.FlightClasses.Any(a => a.Price < max && a.FlightClassId == clas)).ToList();
                    }
                    else if (min != null && max == null)
                    {
                        flights = flights.Where(f => f.FlightClasses.Any(a => a.Price >= min && a.FlightClassId == clas)).ToList();
                    }
                }
            }
            if(flights==null)
            {
                if(timef!=null&&timet!=null)
                {
                    flights = await _context.Flights
                    .Where(f => f.LeaveTime > DateTime.Now && f.LeaveTime>=timef&&f.LeaveTime<timet)
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
                }
                else if(timef == null && timet != null)
                {
                    flights = await _context.Flights
                    .Where(f => f.LeaveTime > DateTime.Now && f.LeaveTime < timet)
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
                }
                else if (timef != null && timet == null)
                {
                    flights = await _context.Flights
                    .Where(f => f.LeaveTime > DateTime.Now && f.LeaveTime >= timef)
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
                }
                else if(conf!=null&&cont!=null)
                {
                    flights = await _context.Flights
                    .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass)
                   .Where(f=> f.LeaveTime > DateTime.Now && (f.DepartingAirport.City.ToLower().Contains(conf.ToLower()) || f.DepartingAirport.Country.Name.ToLower().Contains(conf.ToLower()))&& (f.ArrivingAirport.City.ToLower().Contains(cont.ToLower()) || f.ArrivingAirport.Country.Name.ToLower().Contains(cont.ToLower()))).ToListAsync();
                }
                else if (conf == null && cont != null)
                {
                    flights = await _context.Flights
                    .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass)
                   .Where(f => f.LeaveTime > DateTime.Now && (f.ArrivingAirport.City.ToLower().Contains(cont.ToLower()) || f.ArrivingAirport.Country.Name.ToLower().Contains(cont.ToLower()))).ToListAsync();
                }
                else if (conf != null && cont == null)
                {
                    flights = await _context.Flights
                    .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass)
                   .Where(f => f.LeaveTime > DateTime.Now && (f.DepartingAirport.City.ToLower().Contains(conf.ToLower()) || f.DepartingAirport.Country.Name.ToLower().Contains(conf.ToLower()))).ToListAsync();
                }
            }
            else
            {
                if (timef != null && timet != null)
                {
                    flights = flights
                    .Where(f => f.LeaveTime >= timef && f.LeaveTime < timet)
                   .ToList();
                }
                else if (timef == null && timet != null)
                {
                    flights =flights
                    .Where(f => f.LeaveTime < timet)
                   .ToList();
                }
                else if (timef != null && timet == null)
                {
                    flights = flights
                    .Where(f => f.LeaveTime >= timef)
                    .ToList();
                }
                if (conf != null && cont != null)
                {
                    flights = flights
                   .Where(f => (f.DepartingAirport.City.ToLower().Contains(conf.Trim().ToLower()) || f.DepartingAirport.Country.Name.ToLower().Contains(conf.Trim().ToLower())) && (f.ArrivingAirport.City.ToLower().Contains(cont.Trim().ToLower()) || f.ArrivingAirport.Country.Name.ToLower().Contains(cont.Trim().ToLower()))).ToList();
                }
                else if (conf == null && cont != null)
                {
                    flights = flights
                   .Where(f => (f.ArrivingAirport.City.ToLower().Contains(cont.Trim().ToLower()) || f.ArrivingAirport.Country.Name.ToLower().Contains(cont.Trim().ToLower()))).ToList();
                }
                else if (conf != null && cont == null)
                {
                    flights = flights
                   .Where(f => (f.DepartingAirport.City.ToLower().Contains(conf.Trim().ToLower()) || f.DepartingAirport.Country.Name.ToLower().Contains(conf.Trim().ToLower()))).ToList();
                }
            }
            if(flights==null)
            {
                flights = await _context.Flights
                    .Where(f=> f.LeaveTime > DateTime.Now)
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
            }
             if (order == "Time" && ordersort == "desc")
                flights = flights.OrderBy(f => f.LeaveTime).ToList();
            else if (order == "Airline" && ordersort == "desc")
                flights = flights.OrderBy(f => f.Airline.Name).ToList();
            else if (order == "From" && ordersort == "desc")
                flights = flights.OrderBy(f => f.DepartingAirport.Country.Name).ToList();
            else if (order == "To" && ordersort == "desc")
                flights = flights.OrderBy(f => f.ArrivingAirport.Country.Name).ToList();
            else if (order == "Time")
                flights = flights.OrderByDescending(f => f.LeaveTime).ToList();
            else if (order == "Airline")
                flights = flights.OrderByDescending(f => f.Airline.Name).ToList();
            else if (order == "From")
                flights = flights.OrderByDescending(f => f.DepartingAirport.Country.Name).ToList();
            else if (order == "To")
                flights = flights.OrderByDescending(f => f.ArrivingAirport.Country.Name).ToList();
            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";
            return View(flights);
        }
         [Authorize]
        // GET: FlightBookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightBooking = await _context.FlightBookings
                .Include(f => f.Customer)
                .Include(f => f.Flight)
                .FirstOrDefaultAsync(m => m.Booknum == id);
            if (flightBooking == null)
            {
                return NotFound();
            }

            return View(flightBooking);
        }

        // GET: FlightBookings/Edit/5
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Manager)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightBooking = await _context.FlightBookings.FindAsync(id);
            if (flightBooking == null)
            {
                return NotFound();
            }
            ViewData["FlightClassId"] = new SelectList(_context.Flights, "Id", "Class", flightBooking.FlightId);
            return View(flightBooking);
        }

        // POST: FlightBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Manager)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumberOfAdults,NumberOfChildren,NumberOfInfants,NumberOfSeats,TotalPrice,Date,FlightClassId,CustomerId")] FlightBooking flightBooking)
        {
            if (id != flightBooking.Booknum)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flightBooking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightBookingExists(flightBooking.Booknum))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlightClassId"] = new SelectList(_context.Flights, "Id", "Class", flightBooking.FlightId);
            return View(flightBooking);
        }

        // GET: FlightBookings/Delete/5
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Manager)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightBooking = await _context.FlightBookings
                .Include(f => f.Customer)
                .Include(f => f.Flight)
                .FirstOrDefaultAsync(m => m.Booknum == id);
            if (flightBooking == null)
            {
                return NotFound();
            }

            return View(flightBooking);
        }

        // POST: FlightBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Manager)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flightBooking = await _context.FlightBookings.FindAsync(id);
            _context.FlightBookings.Remove(flightBooking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightBookingExists(int id)
        {
            return _context.FlightBookings.Any(e => e.Booknum == id);
        }


        [HttpGet]
        [Authorize(Roles = SD.Role_Customer + "," + SD.Role_Manager + "," + SD.Role_Admin)]

        public async Task<IActionResult> Flightdetails(int? id)
        {
            Flight flight = await _context.Flights.Include(f => f.Airline)
                          .Include(f=>f.ArrivingAirport).ThenInclude(e=>e.Country)
                          .Include(f=>f.DepartingAirport).ThenInclude(e => e.Country)
                          .Include(f=>f.FlightClasses).ThenInclude(e=>e.FlightClass)
                          .FirstOrDefaultAsync(f=>f.Id==id);
            if(flight.LeaveTime < DateTime.Now)
            {
                return NotFound();
            }
            var claimidentity = (ClaimsIdentity) User.Identity;
            var claim=claimidentity.FindFirst(ClaimTypes.NameIdentifier);
            var customer = await _context.Customers.FirstOrDefaultAsync(f=>f.Id==claim.Value);
            FlightBooking booking=new FlightBooking()
            {
                Customer = customer,
                Flight = flight,
                FlightId= id.Value,
            };
            ViewBag.customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "Name");
            return View(booking);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Flightdetails(FlightBooking flightBooking)
        {
            Flight flight = await _context.Flights.Include(f => f.Airline)
                          .Include(f => f.ArrivingAirport).ThenInclude(e => e.Country)
                          .Include(f => f.DepartingAirport).ThenInclude(e => e.Country)
                          .Include(f => f.FlightClasses).ThenInclude(e => e.FlightClass)
                          .FirstOrDefaultAsync(f => f.Id == flightBooking.FlightId);
            FlightClass flightclass = await _context.FlightClassss.Include(f => f.Flights)
                          .FirstOrDefaultAsync(f => f.ID == flightBooking.FlightClassId);
            var customer = await _context.Customers.FirstOrDefaultAsync(f => f.Id == flightBooking.CustomerId);
            flightBooking.Flight = flight;
            flightBooking.FlightClass = flightclass;
            flightBooking.Customer = customer;
            if (ModelState.IsValid)
            {

                FlightBooking? booking = await _context.FlightBookings
                         .Where(f => f.FlightClassId == flightBooking.FlightClassId && f.FlightId == flightBooking.FlightId)
                         .Include(f => f.FlightClass).ThenInclude(e => e.Flights).FirstOrDefaultAsync();
                if (booking == null)
                {
                    flightBooking.TotalPrice = flightBooking.NumberOfSeats * flightBooking.FlightClass.Flights
                                           .FirstOrDefault(f => f.FlightId == flightBooking.FlightId).Price;
                    await _context.FlightBookings.AddAsync(flightBooking);
                }
                else
                {
                    booking.NumberOfSeats += flightBooking.NumberOfSeats;
                    booking.TotalPrice = booking.NumberOfSeats * booking.FlightClass.Flights
                        .FirstOrDefault(f => f.FlightId == booking.FlightId).Price;
                    _context.FlightBookings.Update(booking);
                }
                await _context.SaveChangesAsync();
                var claimidentity = (ClaimsIdentity)User.Identity;
                var claim = claimidentity.FindFirst(ClaimTypes.NameIdentifier);
                var check = await _context.Customers.FirstOrDefaultAsync(f => f.Id == claim.Value);
                if (check != null)
                    HttpContext.Session.SetInt32(SD.Session_FlightBooking, _context.FlightBookings.Where(u => u.CustomerId == flightBooking.CustomerId).ToList().Count);
                return RedirectToAction(nameof(Index));

            }

            ViewBag.customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "Name");
            return View(flightBooking);
        }

        [Authorize(Roles = SD.Role_Customer+","+SD.Role_Manager+","+SD.Role_Admin)]
        public async Task<IActionResult> BookingList()
        {
            var claimidentity = (ClaimsIdentity)User.Identity;
            var claim = claimidentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<FlightBooking> flightBookings=await _context.FlightBookings.Where(f=>f.CustomerId==claim.Value)
                                                      .Include(f=>f.FlightClass).ThenInclude(e=>e.Flights)
                                                      .Include(f=>f.Flight).ThenInclude(e=>e.Airline)
                                                      .Include(f=>f.Flight).ThenInclude(e=>e.ArrivingAirport).ThenInclude(s=>s.Country)
                                                      .Include(f => f.Flight).ThenInclude(e => e.DepartingAirport).ThenInclude(s => s.Country)
                                                      .ToListAsync();
            ViewBag.total= (double)flightBookings.Select(f=>f.TotalPrice).Sum(); 
            return View(flightBookings);
        }

        [Authorize(Roles = SD.Role_Customer + "," + SD.Role_Manager + "," + SD.Role_Admin)]
        public async Task<IActionResult> Plus(int id)
        {
            FlightBooking flight=await _context.FlightBookings
                .Include(f => f.FlightClass).ThenInclude(e => e.Flights)
                .FirstOrDefaultAsync(f=>f.Booknum == id);

            flight.NumberOfSeats++;
            flight.TotalPrice = flight.FlightClass.Flights.FirstOrDefault(e => e.FlightId == flight.FlightId).Price * flight.NumberOfSeats;
            _context.FlightBookings.Update(flight);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(BookingList));

        }

        [Authorize(Roles = SD.Role_Customer + "," + SD.Role_Manager + "," + SD.Role_Admin)]
        public async Task<IActionResult> mins(int id)
        {
            FlightBooking flight = await _context.FlightBookings
                .Include(f => f.FlightClass).ThenInclude(e => e.Flights)
                .FirstOrDefaultAsync(f => f.Booknum == id);
            if (flight.NumberOfSeats > 1)
            {
                flight.NumberOfSeats--;
                flight.TotalPrice = flight.FlightClass.Flights.FirstOrDefault(e => e.FlightId == flight.FlightId).Price * flight.NumberOfSeats;
                _context.FlightBookings.Update(flight);
            }
            else
            {
                _context.FlightBookings.Remove(flight);
                
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(BookingList));

        }

        [Authorize(Roles = SD.Role_Customer + "," + SD.Role_Manager + "," + SD.Role_Admin)]
        public async Task<IActionResult> remove(int id)
        {
            FlightBooking flight = await _context.FlightBookings
                .FirstOrDefaultAsync(f => f.Booknum == id);
            _context.FlightBookings.Remove(flight);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(BookingList));

        }


        [HttpGet]
        [Authorize(Roles = SD.Role_Customer + "," + SD.Role_Manager + "," + SD.Role_Admin)]
        public async Task<IActionResult> FlightSummary()
        {
            var claimidentity = (ClaimsIdentity)User.Identity;
            var claim = claimidentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<FlightBooking> flightBookings = await _context.FlightBookings
                                                      .Where(f => f.CustomerId == claim.Value)
                                                      .Include(f => f.FlightClass).ThenInclude(e => e.Flights)
                                                      .Include(f => f.Flight).ThenInclude(e => e.Airline)
                                                      .Include(f => f.Flight).ThenInclude(e => e.ArrivingAirport).ThenInclude(s => s.Country)
                                                      .Include(f => f.Flight).ThenInclude(e => e.DepartingAirport).ThenInclude(s => s.Country)
                                                      .ToListAsync();
            List<FlightBookingDetails> bookingDetails = new List<FlightBookingDetails>();
            foreach (var flight in flightBookings)
            {
                
                var details = new FlightBookingDetails()
                {
                    FlightId = flight.FlightId,
                    Flight = flight.Flight,
                    FlightClass = flight.FlightClass,
                    FlightClassId = flight.FlightClassId,
                    Customer = flight.Customer,
                    CustomerId = flight.CustomerId,
                    NumberOfSeats = flight.NumberOfSeats,
                    TotalPrice = flight.TotalPrice,
                    
                };
                bookingDetails.Add(details);
            }
            Models.Customer customer = await _context.Customers.Include(f=>f.IdentityUser).FirstOrDefaultAsync(f => f.Id == claim.Value);
            FlightBookingHeader header = new FlightBookingHeader()
            {
                FlightBookings= bookingDetails,
                CustomerID=customer.Id,
                Customer=customer,
                OrderTotal=(double) flightBookings.Select(f => f.TotalPrice).Sum(),
                
            };
            return View(header);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FlightSummary(FlightBookingHeader header)
        {
            IEnumerable<FlightBooking> flightBookings = await _context.FlightBookings
                                                      .Where(f => f.CustomerId == header.CustomerID)
                                                      .Include(f => f.FlightClass).ThenInclude(e => e.Flights)
                                                      .Include(f => f.Flight).ThenInclude(e => e.Airline)
                                                      .Include(f => f.Flight).ThenInclude(e => e.ArrivingAirport).ThenInclude(s => s.Country)
                                                      .Include(f => f.Flight).ThenInclude(e => e.DepartingAirport).ThenInclude(s => s.Country)
                                                      .ToListAsync();
            List<FlightBookingDetails> bookingDetails = new List<FlightBookingDetails>();
            foreach (var flight in flightBookings)
            {

                var details = new FlightBookingDetails()
                {
                    FlightId = flight.FlightId,
                    Flight= flight.Flight,
                    FlightClassId = flight.FlightClassId,
                    FlightClass = flight.FlightClass,
                    CustomerId = flight.CustomerId,
                    Customer= flight.Customer,
                    NumberOfSeats = flight.NumberOfSeats,
                    TotalPrice = flight.TotalPrice,

                };
                bookingDetails.Add(details);
            }
            header.FlightBookings= bookingDetails;
            header.PaymentStatus = SD.Payment_Pending;
            header.OrderStatus= SD.Status_Pending;
            header.OrderDate= DateTime.Now;
           await _context.FlightBookingHeader.AddAsync(header);
            await _context.SaveChangesAsync();
            foreach(var order in header.FlightBookings)
            {
                order.OrderID = header.ID;

                await _context.FlightBookingDetails.AddAsync(order);
            }
            await _context.SaveChangesAsync();
            string domain = "https://localhost:44310/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>()
                ,
                Mode = "payment",
                SuccessUrl = domain+$"Customer/FlightBookings/OrderConfirm?id={header.ID}",
                CancelUrl = domain+$"Customer/FlightBookings/index",
            };
            foreach(var order in header.FlightBookings)
            {

                var SessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long?)((order.TotalPrice/order.NumberOfSeats) * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Flight to " + order.Flight.ArrivingAirport.City + " ," + order.Flight.ArrivingAirport.Country.Name,
                            Description = order.FlightClass.Class
                        },

                    },
                    Quantity = order.NumberOfSeats,
                };
                options.LineItems.Add(SessionLineItem);
        
            }

            var service = new SessionService();
            Session session = service.Create(options);
            header.SessionId = session.Id;
            header.PaymentId = session.PaymentIntentId;
            _context.FlightBookingHeader.Update(header);
            await _context.SaveChangesAsync();  

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }


        [Authorize(Roles = SD.Role_Customer + "," + SD.Role_Manager + "," + SD.Role_Admin)]
        public async Task<IActionResult> OrderConfirm(int id)
        {
            FlightBookingHeader header=await _context.FlightBookingHeader.FirstOrDefaultAsync(f=>f.ID==id);
            var service=new SessionService();
            var session =await service.GetAsync(header.SessionId);
            if (session.PaymentStatus.ToLower()=="paid")
            {
                header.OrderStatus = SD.Status_Approved;
                header.PaymentStatus = SD.Payment_Approved;
                header.PaymentDate = DateTime.Now;
                _context.FlightBookingHeader.Update(header);
                await _context.SaveChangesAsync();
            }
            IEnumerable<FlightBooking> flightBookings = await _context.FlightBookings
                                                      .Where(f => f.CustomerId == header.CustomerID)
                                                      .ToListAsync();
            _context.FlightBookings.RemoveRange(flightBookings);
            await _context.SaveChangesAsync();
            return View(id);
        }
    }
}
