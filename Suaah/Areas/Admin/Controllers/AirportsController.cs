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
    public class AirportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AirportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Airports
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search,string? type,string? order,string? ordersort)
        {
            List<Airport> airports;
            List<string> types=new List<string>() { "name","country","city"};
            ViewBag.search = search;
            if(string.IsNullOrEmpty(type))
            ViewBag.types=new SelectList(types);
            else
             ViewBag.types = new SelectList(types,type);
            if(!string.IsNullOrEmpty(order))
                ViewBag.order=order;
            if (string.IsNullOrEmpty(search))
                airports = await _context.Airports.Include(e => e.Country).ToListAsync();
            else if(!string.IsNullOrEmpty(search) && type =="name")
                airports = await _context.Airports.Where(f=>f.Name.ToLower().Contains(search.Trim().ToLower())).Include(e => e.Country).ToListAsync();
            else if(!string.IsNullOrEmpty(search) && type == "country")
                airports = await _context.Airports.Include(e => e.Country).Where(f => f.Country.Name.ToLower().Contains(search.Trim().ToLower())).ToListAsync();
            else
                airports = await _context.Airports.Where(f => f.City.ToLower().Contains(search.Trim().ToLower())).Include(e => e.Country).ToListAsync();
             if (order == "name" && ordersort=="desc")
                airports = airports.OrderBy(f => f.Name).ToList();
            else if (order == "country" && ordersort == "desc")
                airports = airports.OrderBy(f => f.Country.Name).ToList();
            else if(order=="city" && ordersort == "desc")
                airports = airports.OrderBy(f => f.City).ToList();
             else if(order == "name")
                airports = airports.OrderByDescending(f => f.Name).ToList();
            else if (order == "country")
                airports = airports.OrderByDescending(f => f.Country.Name).ToList();
            else if (order == "city")
                airports = airports.OrderByDescending(f => f.City).ToList();
            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";
            return View(airports);
        }

        // GET: Airports/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports
                .Include(e => e.Country).FirstOrDefaultAsync(m => m.Id == id);
            if (airport == null)
            {
                return NotFound();
            }

            return View(airport);
        }

        // GET: Airports/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Countries =new SelectList( await _context.Countries.ToListAsync(),"ID","Name");
            return View();
        }

        // POST: Airports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Airport airport)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(airport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "ID", "Name",airport.CountryId);
            return View(airport);
        }

        // GET: Airports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports.FindAsync(id);
            if (airport == null)
            {
                return NotFound();
            }
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "ID", "Name",airport.CountryId);
            return View(airport);
        }

        // POST: Airports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Country,Governorate,City,Description")] Airport airport)
        {
            if (id != airport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(airport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirportExists(airport.Id))
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
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "ID", "Name", airport.CountryId);
            return View(airport);
        }

        // GET: Airports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airport = await _context.Airports
                .Include(e => e.Country).FirstOrDefaultAsync(m => m.Id == id);
            if (airport == null)
            {
                return NotFound();
            }

            return View(airport);
        }

        // POST: Airports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var airport = await _context.Airports.FindAsync(id);
            List<Flight> flights = _context.Flights.Where(m => m.DepartingAirportId == id || m.ArrivingAirportId==id).ToList();
            foreach (Flight flight in flights)
            {
                List<FlightsClasses> flightsClasses = _context.FlightsClasses.Where(m => m.FlightId == flight.Id).ToList();
                foreach (FlightsClasses classes in flightsClasses)
                {
                    _context.FlightsClasses.Remove(classes);
                }
                await _context.SaveChangesAsync();
                _context.Flights.Remove(flight);
            }
            await _context.SaveChangesAsync();
            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AirportExists(int id)
        {
            return _context.Airports.Any(e => e.Id == id);
        }
    }
}
