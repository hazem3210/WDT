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
    public class AirlinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public AirlinesController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        // GET: Airlines
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search,string hCountry, string? type, string? order, string? ordersort, int pageSize, int pageNumber)
        {
            List<Airline> airlines;
            List<string> types = new List<string>() { "name", "country" };

            ViewBag.search = search;

            if (string.IsNullOrEmpty(type))
                ViewBag.types = new SelectList(types);
            else
                ViewBag.types = new SelectList(types, type);

            if (!string.IsNullOrEmpty(order))
                ViewBag.order = order;
           
            if (string.IsNullOrEmpty(search))
                airlines = await _context.Airlines.Include(c => c.Country).ToListAsync();
            else if (!string.IsNullOrEmpty(search) && type == "name")
                airlines = await _context.Airlines.Where(f => f.Name.ToLower().Contains(search.Trim().ToLower())).Include(c => c.Country).ToListAsync();
            else 
                airlines = await _context.Airlines.Where(f => f.Country.Name.ToLower().Contains(search.Trim().ToLower())).Include(c => c.Country).ToListAsync();
           
            if (order == "name" && ordersort == "desc")
                airlines = airlines.OrderBy(f => f.Name).ToList();
            else if (order == "country" && ordersort == "desc")
                airlines = airlines.OrderBy(f => f.Country.Name).ToList();
            else if (order == "name")
                airlines = airlines.OrderByDescending(f => f.Name).ToList();
            else if (order == "country")
                airlines = airlines.OrderByDescending(f => f.Country.Name).ToList();
           
            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";

            if (pageSize > 0 && pageNumber > 0)
            {
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;

                airlines = airlines.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }

            return View(airlines);
        }

        // GET: Airlines/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines
                .FirstOrDefaultAsync(m => m.Code == id);
            if (airline == null)
            {
                return NotFound();
            }
            ViewBag.flights=await _context.Flights.Where(f=>f.AirlineCode==id).Include(f=>f.Airline)
                .Include(f=>f.FlightClasses).ThenInclude(e=>e.FlightClass)
                .Include(f=>f.ArrivingAirport).ThenInclude(e=>e.Country)
                .Include(f => f.DepartingAirport).ThenInclude(e => e.Country)
                .ToListAsync();

            return View(airline);
        }

        // GET: Airlines/Create
        public IActionResult Create()
        {
            ViewBag.Countries = new SelectList(_context.Countries.ToList(), "ID", "Name");
            return View();
        }

        // POST: Airlines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Airline airline, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                CreateFiles(airline, image);

                _context.Add(airline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Countries = new SelectList(_context.Countries.ToList(), "ID", "Name");
            return View(airline);
        }
        protected void CreateFiles(Airline airline, IFormFile image = null)
        {
            if (image != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(_webHost.WebRootPath, @"img\airlines");
                var extension = Path.GetExtension(image.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    image.CopyTo(fileStreams);
                }

                airline.ImageUrl = @"\img\airlines\" + fileName + extension;
            }
        }
        // GET: Airlines/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines.FindAsync(id);
            if (airline == null)
            {
                return NotFound();
            }
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "ID", "Name", airline.CountryId);
            return View(airline);
        }

        // POST: Airlines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Airline airline, IFormFile image)
        {
            if (id != airline.Code)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null)
                    {
                        if (airline.ImageUrl != null)
                        {
                            var oldPath = Path.Combine(_webHost.WebRootPath, airline.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        CreateFiles(image: image, airline: airline);
                    }

                    _context.Update(airline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AirlineExists(airline.Code))
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
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "ID", "Name", airline.CountryId);
            return View(airline);
        }

        // GET: Airlines/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var airline = await _context.Airlines
                .FirstOrDefaultAsync(m => m.Code == id);
            if (airline == null)
            {
                return NotFound();
            }

            return View(airline);
        }

        // POST: Airlines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {

            var airline = await _context.Airlines.FindAsync(id);

            if (airline.ImageUrl != null)
            {
                var oldPath = Path.Combine(_webHost.WebRootPath, airline.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            _context.Airlines.Remove(airline);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool AirlineExists(string id)
        {
            return _context.Airlines.Any(e => e.Code == id);
        }
    }
}
