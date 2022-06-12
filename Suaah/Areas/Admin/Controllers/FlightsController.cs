#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Suaah.Data;
using Suaah.Models;

namespace Suaah.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Manager + "," + SD.Role_Admin)]
    public class FlightsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Flights
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, string? type, string? order, string? ordersort,DateTime? timef, DateTime? timet,string? conf,string? cont)
        {
            List<Flight> flights=null; 
            List<string> names=await _context.Countries.Select(c => c.Name).ToListAsync();
            names.AddRange(await _context.Airports.Select(f=>f.City).ToListAsync());
            ViewBag.names = names;
            List<string> airlines=await _context.Airlines.Select(f=>f.Name).ToListAsync();
            ViewBag.airlines = airlines; 
            List<string> types = new List<string>() { "Departing Gate", "Arrive Gate", "Airline" };
            ViewBag.search = search;
            if(timef != null)
            ViewBag.timef=timef.GetValueOrDefault().ToString("yyyy-MM-ddThh:mm");
            if(timet!=null)
            ViewBag.timet = timet.GetValueOrDefault().ToString("yyyy-MM-ddThh:mm");
            ViewBag.conf = conf;
            ViewBag.cont = cont;

            if (string.IsNullOrEmpty(type))
                ViewBag.types = new SelectList(types);
            else
                ViewBag.types = new SelectList(types, type);
            if (!string.IsNullOrEmpty(order))
                ViewBag.order = order;
            if (!string.IsNullOrEmpty(search))
            {
                if(type== "Departing Gate")
                flights = await _context.Flights
                   .Where(f => f.DepartingGate.ToLower().Contains(search.Trim().ToLower()))
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
                else if(type== "Arrive Gate")
                    flights = await _context.Flights
                   .Where(f => f.ArriveGate.ToLower().Contains(search.Trim().ToLower()))
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
                else if (type == "Airline") // airline??
                    flights = await _context.Flights
                   .Include(f => f.Airline)
                   .Where(f => f.Airline.Name.ToLower().Contains(search.Trim().ToLower()))
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();

            }
            if (flights == null)
            {
                if (timef != null && timet != null)
                {
                    flights = await _context.Flights
                    .Where(f => f.LeaveTime >= timef && f.LeaveTime < timet)
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
                }
                else if (timef == null && timet != null)
                {
                    flights = await _context.Flights
                    .Where(f => f.LeaveTime < timet) // > now by 8h
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
                }
                else if (timef != null && timet == null)
                {
                    flights = await _context.Flights
                    .Where(f => f.LeaveTime >= timef)
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
                }
                else if (conf != null && cont != null)
                {
                    flights = await _context.Flights
                    .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass)
                   .Where(f => (f.DepartingAirport.City.ToLower().Contains(conf.ToLower()) || f.DepartingAirport.Country.Name.ToLower().Contains(conf.ToLower())) && (f.ArrivingAirport.City.ToLower().Contains(cont.ToLower()) || f.ArrivingAirport.Country.Name.ToLower().Contains(cont.ToLower()))).ToListAsync();
                }
                else if (conf == null && cont != null)
                {
                    flights = await _context.Flights
                    .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass)
                   .Where(f => (f.ArrivingAirport.City.ToLower().Contains(cont.ToLower()) || f.ArrivingAirport.Country.Name.ToLower().Contains(cont.ToLower()))).ToListAsync();
                }
                else if (conf != null && cont == null)
                {
                    flights = await _context.Flights
                    .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass)
                   .Where(f => (f.DepartingAirport.City.ToLower().Contains(conf.ToLower()) || f.DepartingAirport.Country.Name.ToLower().Contains(conf.ToLower()))).ToListAsync();
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
                    flights = flights
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
                   .Where(f => (f.DepartingAirport.City.ToLower().Contains(conf.ToLower()) || f.DepartingAirport.Country.Name.ToLower().Contains(conf.ToLower())) && (f.ArrivingAirport.City.ToLower().Contains(cont.ToLower()) || f.ArrivingAirport.Country.Name.ToLower().Contains(cont.ToLower()))).ToList();
                }
                else if (conf == null && cont != null)
                {
                    flights = flights
                   .Where(f => (f.ArrivingAirport.City.ToLower().Contains(cont.ToLower()) || f.ArrivingAirport.Country.Name.ToLower().Contains(cont.ToLower()))).ToList();
                }
                else if (conf != null && cont == null)
                {
                    flights = flights
                   .Where(f => (f.DepartingAirport.City.ToLower().Contains(conf.ToLower()) || f.DepartingAirport.Country.Name.ToLower().Contains(conf.ToLower()))).ToList();
                }
            }
            if (flights == null)
            {
                flights = await _context.Flights
                   .Include(f => f.Airline)
                   .Include(f => f.ArrivingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.DepartingAirport).ThenInclude(a => a.Country)
                   .Include(f => f.FlightClasses).ThenInclude(a => a.FlightClass).ToListAsync();
            }
           
            if (order == "DepartingGate" && ordersort == "desc")
                flights = flights.OrderBy(f => f.DepartingGate).ToList();
            else if (order == "ArriveGate" && ordersort == "desc")
                flights = flights.OrderBy(f => f.ArriveGate).ToList();
            else if (order == "LeaveTime" && ordersort == "desc")
                flights = flights.OrderBy(f => f.LeaveTime).ToList();
            else if (order == "Airline" && ordersort == "desc")
                flights = flights.OrderBy(f => f.Airline.Name).ToList();
            else if (order == "From" && ordersort == "desc")
                flights = flights.OrderBy(f => f.DepartingAirport.Country.Name).ToList();
            else if (order == "To" && ordersort == "desc")
                flights = flights.OrderBy(f => f.ArrivingAirport.Country.Name).ToList();
            else if (order == "DepartingGate")
                flights = flights.OrderByDescending(f => f.DepartingGate).ToList();
            else if (order == "ArriveGate")
                flights = flights.OrderByDescending(f => f.ArriveGate).ToList();
            else if (order == "LeaveTime")
                flights = flights.OrderByDescending(f => f.LeaveTime).ToList();
            else if (order == "Airline" )
                flights = flights.OrderByDescending(f => f.Airline.Name).ToList();
            else if (order == "From" )
                flights = flights.OrderByDescending(f => f.DepartingAirport.Country.Name).ToList();
            else if (order == "To" )
                flights = flights.OrderByDescending(f => f.ArrivingAirport.Country.Name).ToList();
           
            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";
            return View(flights);
        }

        // GET: Flights/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.Airline)
                .Include(f => f.ArrivingAirport)
                .Include(f => f.DepartingAirport)
                .Include(f => f.FlightClasses)
                .ThenInclude(e => e.FlightClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // GET: Flights/Create
        public async Task<IActionResult> Create()
        {
            ViewData["AirlineCode"] = new SelectList(_context.Airlines, "Code", "Name");
            ViewData["ArrivingAirportId"] = new SelectList(_context.Airports, "Id", "Name");
            ViewData["DepartingAirportId"] = new SelectList(_context.Airports, "Id", "Name");
            ViewData["Classes"] = await _context.FlightClassss.ToListAsync();
            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( FlightVM flightvm)
        {
            if (ModelState.IsValid)
            {
                if(flightvm.Classes.Count != flightvm.Prices.Count)
                {
                    ModelState.AddModelError("Prices", "You must enter all the prices");
                    ViewData["AirlineCode"] = new SelectList(_context.Airlines, "Code", "Name", flightvm.Flight.AirlineCode);
                    ViewData["ArrivingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.ArrivingAirportId);
                    ViewData["DepartingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.DepartingAirportId);
                    ViewData["Classes"] = await _context.FlightClassss.ToListAsync();
                    return View(flightvm);

                }
                else if(flightvm.Flight.DepartingAirportId==flightvm.Flight.ArrivingAirportId)
                {
                    ModelState.AddModelError("Flight.ArrivingAirportId", "You must choose differnt airport than the Departing Airport");
                    ViewData["AirlineCode"] = new SelectList(_context.Airlines, "Code", "Name", flightvm.Flight.AirlineCode);
                    ViewData["ArrivingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.ArrivingAirportId);
                    ViewData["DepartingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.DepartingAirportId);
                    ViewData["Classes"] = await _context.FlightClassss.ToListAsync();
                    return View(flightvm);

                }
                else
                {

                    await _context.AddAsync(flightvm.Flight);
                    await _context.SaveChangesAsync();
                    int i = 0;
                    foreach (int item in flightvm.Classes)
                    {
                        FlightsClasses flightsClasses = new FlightsClasses()
                        {
                            FlightId = flightvm.Flight.Id,
                            FlightClassId = item,
                            Price = flightvm.Prices[i]
                        };
                        await _context.AddAsync(flightsClasses);
                        i++;
                    }
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                
            }
            if (flightvm.Prices.Count == 0)
                flightvm.Prices = null;
            ViewData["AirlineCode"] = new SelectList(_context.Airlines, "Code", "Name", flightvm.Flight.AirlineCode);
            ViewData["ArrivingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.ArrivingAirportId);
            ViewData["DepartingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.DepartingAirportId);
            ViewData["Classes"] =await _context.FlightClassss.ToListAsync();

            return View(flightvm);
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights.FindAsync(id);
            var classes=await _context.FlightsClasses.Where(e=>e.FlightId==id).ToListAsync();
            FlightVM flightVM = new FlightVM()
            {
                Flight=flight,
                Classes=classes.Select(e=>e.FlightClassId).ToList(),
                Prices=classes.Select(e=>e.Price).ToList(),

            };
            if (flight == null)
            {
                return NotFound();
            }
            ViewData["AirlineCode"] = new SelectList(_context.Airlines, "Code", "Code", flight.AirlineCode);
            ViewData["ArrivingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flight.ArrivingAirportId);
            ViewData["DepartingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flight.DepartingAirportId);
            ViewData["Classes"] = await _context.FlightClassss.ToListAsync();
            return View(flightVM);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,FlightVM flightvm)
        {
            if (id != flightvm.Flight.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if(flightvm.Classes.Count>flightvm.Prices.Count)
                {
                    ModelState.AddModelError("Prices", "You must enter all the prices");
                    ViewData["AirlineCode"] = new SelectList(_context.Airlines, "Code", "Name", flightvm.Flight.AirlineCode);
                    ViewData["ArrivingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.ArrivingAirportId);
                    ViewData["DepartingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.DepartingAirportId);
                    ViewData["Classes"] = await _context.FlightClassss.ToListAsync();
                    return View(flightvm);
                }
                else if(flightvm.Flight.DepartingAirportId == flightvm.Flight.ArrivingAirportId)
                {
                    ModelState.AddModelError("Flight.ArrivingAirportId", "You must choose differnt airport than the Departing Airport");
                    ViewData["AirlineCode"] = new SelectList(_context.Airlines, "Code", "Name", flightvm.Flight.AirlineCode);
                    ViewData["ArrivingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.ArrivingAirportId);
                    ViewData["DepartingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.DepartingAirportId);
                    ViewData["Classes"] = await _context.FlightClassss.ToListAsync();
                    return View(flightvm);
                }
                else
                try
                {
                    _context.Update(flightvm.Flight);
                    List<FlightsClasses> flightsClasses=await _context.FlightsClasses.Where(e=>e.FlightId==id).ToListAsync();
                    List<int> founds=new List<int>();
                        foreach(FlightsClasses item in flightsClasses)
                        {
                            int found = flightvm.Classes.FirstOrDefault(e=>e==item.FlightClassId);
                            if (found == 0)
                            {
                                _context.FlightsClasses.Remove(item);
                            }
                            else
                            {
                                item.Price = flightvm.Prices[flightvm.Classes.IndexOf(found)];
                                _context.FlightsClasses.Update(item);
                                founds.Add(found);
                            }
                        }
                        foreach(int item in flightvm.Classes)
                        {
                            if (!founds.Contains(item))
                            {
                                FlightsClasses clas=new FlightsClasses()
                                {
                                    FlightId=id,
                                    FlightClassId=item,
                                    Price= flightvm.Prices[flightvm.Classes.IndexOf(item)]
                                };
                               await _context.AddAsync(clas);
                            }
                        }

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flightvm.Flight.Id))
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
            if(flightvm.Prices.Count==0)
                flightvm.Prices = null;
            ViewData["AirlineCode"] = new SelectList(_context.Airlines, "Code", "Code", flightvm.Flight.AirlineCode);
            ViewData["ArrivingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.ArrivingAirportId);
            ViewData["DepartingAirportId"] = new SelectList(_context.Airports, "Id", "Name", flightvm.Flight.DepartingAirportId);
            ViewData["Classes"] = await _context.FlightClassss.ToListAsync();

            return View(flightvm);
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.Airline)
                .Include(f => f.ArrivingAirport)
                .Include(f => f.DepartingAirport)
                .Include(f=>f.FlightClasses)
                .ThenInclude(e=>e.FlightClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            List<FlightsClasses> flightsClasses=await _context.FlightsClasses.Where(e=>e.FlightId==id).ToListAsync();
            foreach(FlightsClasses classes in flightsClasses)
            {
                _context.FlightsClasses.Remove(classes);
            }
            await _context.SaveChangesAsync();
            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(e => e.Id == id);
        }
    }
}
