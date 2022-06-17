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
    public class FlightClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FlightClasses
        public async Task<IActionResult> Index(string? name, string? order, int pageSize, int pageNumber)
        {
            ViewBag.Names = await _context.FlightClassss.Select(f => f.Class).ToListAsync();

            List<FlightClass> flightClasses;
            if (string.IsNullOrEmpty(name))
            {
                flightClasses = await _context.FlightClassss.ToListAsync();
                ViewBag.name = ViewBag.name;
            }
            else
            {
                flightClasses = await _context.FlightClassss.Where(x => x.Class.ToLower().Contains(name.ToLower())).ToListAsync();
                ViewBag.name = name;
            }
            if (order == "desc")
            {
                flightClasses = flightClasses.OrderByDescending(f => f.Class).ToList();
                ViewBag.order = "asc";
            }
            else
            {
                flightClasses = flightClasses.OrderBy(f => f.Class).ToList();
                ViewBag.order = "desc";
            }

            if (pageSize > 0 && pageNumber > 0)
            {
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;

                flightClasses = flightClasses.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }

            return View(flightClasses);
        }

        // GET: FlightClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightClass = await _context.FlightClassss
                .FirstOrDefaultAsync(m => m.ID == id);
            if (flightClass == null)
            {
                return NotFound();
            }

            return View(flightClass);
        }

        // GET: FlightClasses/Create
        public IActionResult Create()
        {
            ViewData["FlightId"] = new SelectList(_context.Flights, "Id", "ArriveGate");
            return View();
        }

        // POST: FlightClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Class,Price,FlightId")] FlightClass flightClass)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(flightClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(flightClass);
        }

        // GET: FlightClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightClass = await _context.FlightClassss.FindAsync(id);
            if (flightClass == null)
            {
                return NotFound();
            }
            return View(flightClass);
        }

        // POST: FlightClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Class,Price,FlightId")] FlightClass flightClass)
        {
            if (id != flightClass.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flightClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightClassExists(flightClass.ID))
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
            return View(flightClass);
        }

        // GET: FlightClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightClass = await _context.FlightClassss
                .FirstOrDefaultAsync(m => m.ID == id);
            if (flightClass == null)
            {
                return NotFound();
            }

            return View(flightClass);
        }

        // POST: FlightClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flightClass = await _context.FlightClassss.FindAsync(id);
            List<FlightsClasses> flightsClasses=_context.FlightsClasses.Where(m=> m.FlightClassId == id).ToList();
            foreach(FlightsClasses classes in flightsClasses)
            {
                _context.FlightsClasses.Remove(classes);
            }
            await _context.SaveChangesAsync();
            _context.FlightClassss.Remove(flightClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightClassExists(int id)
        {
            return _context.FlightClassss.Any(e => e.ID == id);
        }
    }
}
