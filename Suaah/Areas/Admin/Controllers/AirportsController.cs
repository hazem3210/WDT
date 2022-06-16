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
        private readonly IWebHostEnvironment _webHost;

        public AirportsController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        // GET: Airports
        [AllowAnonymous]
        public async Task<IActionResult> Index(string search,string type,string order,string ordersort, int pageSize, int pageNumber)
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

             if (order == "name" && ordersort== "asc")
                airports = airports.OrderBy(f => f.Name).ToList();
            else if (order == "country" && ordersort == "asc")
                airports = airports.OrderBy(f => f.Country.Name).ToList();
            else if(order=="city" && ordersort == "asc")
                airports = airports.OrderBy(f => f.City).ToList();
             else if(order == "name")
                airports = airports.OrderByDescending(f => f.Name).ToList();
            else if (order == "country")
                airports = airports.OrderByDescending(f => f.Country.Name).ToList();
            else if (order == "city")
                airports = airports.OrderByDescending(f => f.City).ToList();
            
            if (ordersort == "asc")
                ViewBag.ordersort = "desc";
            else
                ViewBag.ordersort = "asc";


            if (pageSize > 0 && pageNumber > 0)
            {
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;

                airports = airports.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }

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
            ViewBag.flights = await _context.Flights.Where(f => f.ArrivingAirportId == id || f.DepartingAirportId==id).Include(f => f.Airline)
                .Include(f => f.FlightClasses).ThenInclude(e => e.FlightClass)
                .Include(f => f.ArrivingAirport).ThenInclude(e => e.Country)
                .Include(f => f.DepartingAirport).ThenInclude(e => e.Country)
                .ToListAsync();
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
        public async Task<IActionResult> Create( Airport airport, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                CreateFiles(airport, image);

                await _context.AddAsync(airport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "ID", "Name",airport.CountryId);
            return View(airport);
        }
        protected void CreateFiles(Airport airport, IFormFile image = null)
        {
            if (image != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(_webHost.WebRootPath, @"img\airport");
                var extension = Path.GetExtension(image.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    image.CopyTo(fileStreams);
                }

                airport.ImageUrl = @"\img\airport\" + fileName + extension;
            }
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
        public async Task<IActionResult> Edit(int id, Airport airport, IFormFile image)
        {
            if (id != airport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null)
                    {
                        if (airport.ImageUrl != null)
                        {
                            var oldPath = Path.Combine(_webHost.WebRootPath, airport.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        CreateFiles(airport, image);
                    }

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

            if (airport.ImageUrl != null)
            {
                var oldPath = Path.Combine(_webHost.WebRootPath, airport.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

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
