#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Suaah.Data;
using Suaah.Models;

namespace Suaah.Controllers
{
    public class FlightClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FlightClasses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FlightClassss.Include(f => f.Flight);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FlightClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightClass = await _context.FlightClassss
                .Include(f => f.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
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
                _context.Add(flightClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlightId"] = new SelectList(_context.Flights, "Id", "ArriveGate", flightClass.FlightId);
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
            ViewData["FlightId"] = new SelectList(_context.Flights, "Id", "ArriveGate", flightClass.FlightId);
            return View(flightClass);
        }

        // POST: FlightClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Class,Price,FlightId")] FlightClass flightClass)
        {
            if (id != flightClass.Id)
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
                    if (!FlightClassExists(flightClass.Id))
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
            ViewData["FlightId"] = new SelectList(_context.Flights, "Id", "ArriveGate", flightClass.FlightId);
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
                .Include(f => f.Flight)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            _context.FlightClassss.Remove(flightClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightClassExists(int id)
        {
            return _context.FlightClassss.Any(e => e.Id == id);
        }
    }
}
