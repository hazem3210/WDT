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
    public class FlightBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightBookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FlightBookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FlightBookings.Include(f => f.Customer).Include(f => f.FlightClass);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FlightBookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightBooking = await _context.FlightBookings
                .Include(f => f.Customer)
                .Include(f => f.FlightClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flightBooking == null)
            {
                return NotFound();
            }

            return View(flightBooking);
        }

        // GET: FlightBookings/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CitizenshipCountry");
            ViewData["FlightClassId"] = new SelectList(_context.FlightClassss, "Id", "Id");
            return View();
        }

        // POST: FlightBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,NumberOfAdults,NumberOfChildren,NumberOfInfants,NumberOfSeats,TotalPrice,FlightClassId,CustomerId")] FlightBooking flightBooking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flightBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CitizenshipCountry", flightBooking.CustomerId);
            ViewData["FlightClassId"] = new SelectList(_context.FlightClassss, "Id", "Id", flightBooking.FlightClassId);
            return View(flightBooking);
        }

        // GET: FlightBookings/Edit/5
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CitizenshipCountry", flightBooking.CustomerId);
            ViewData["FlightClassId"] = new SelectList(_context.FlightClassss, "Id", "Id", flightBooking.FlightClassId);
            return View(flightBooking);
        }

        // POST: FlightBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,NumberOfAdults,NumberOfChildren,NumberOfInfants,NumberOfSeats,TotalPrice,FlightClassId,CustomerId")] FlightBooking flightBooking)
        {
            if (id != flightBooking.Id)
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
                    if (!FlightBookingExists(flightBooking.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CitizenshipCountry", flightBooking.CustomerId);
            ViewData["FlightClassId"] = new SelectList(_context.FlightClassss, "Id", "Id", flightBooking.FlightClassId);
            return View(flightBooking);
        }

        // GET: FlightBookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flightBooking = await _context.FlightBookings
                .Include(f => f.Customer)
                .Include(f => f.FlightClass)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (flightBooking == null)
            {
                return NotFound();
            }

            return View(flightBooking);
        }

        // POST: FlightBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var flightBooking = await _context.FlightBookings.FindAsync(id);
            _context.FlightBookings.Remove(flightBooking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightBookingExists(int id)
        {
            return _context.FlightBookings.Any(e => e.Id == id);
        }
    }
}
