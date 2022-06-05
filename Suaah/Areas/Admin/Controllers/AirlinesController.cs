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

        public AirlinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Airlines
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, string? type, string? order, string? ordersort)
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
                airlines = await _context.Airlines.ToListAsync();
            else if (!string.IsNullOrEmpty(search) && type == "name")
                airlines = await _context.Airlines.Where(f => f.Name.ToLower().Contains(search.Trim().ToLower())).ToListAsync();
            else 
                airlines = await _context.Airlines.Where(f => f.Country.ToLower().Contains(search.Trim().ToLower())).ToListAsync();
           
            if (order == "name" && ordersort == "desc")
                airlines = airlines.OrderBy(f => f.Name).ToList();
            else if (order == "country" && ordersort == "desc")
                airlines = airlines.OrderBy(f => f.Country).ToList();
            else if (order == "name")
                airlines = airlines.OrderByDescending(f => f.Name).ToList();
            else if (order == "country")
                airlines = airlines.OrderByDescending(f => f.Country).ToList();
           
            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";
            
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

            return View(airline);
        }

        // GET: Airlines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Airlines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Name,Country,Description")] Airline airline)
        {
            if (ModelState.IsValid)
            {
                _context.Add(airline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(airline);
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
            return View(airline);
        }

        // POST: Airlines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Code,Name,Country,Description")] Airline airline)
        {
            if (id != airline.Code)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
