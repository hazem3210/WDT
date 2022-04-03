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
    public class HotelBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelBookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HotelBookings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HotelBookings.Include(h => h.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HotelBookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotelBooking = await _context.HotelBookings
                .Include(h => h.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotelBooking == null)
            {
                return NotFound();
            }

            return View(hotelBooking);
        }

        // GET: HotelBookings/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CitizenshipCountry");
            return View();
        }

        // POST: HotelBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,NumberOfRooms,Date,NumberOfDays,TotalPrice,HotelRoomlId,CustomerId")] HotelBooking hotelBooking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hotelBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CitizenshipCountry", hotelBooking.CustomerId);
            return View(hotelBooking);
        }

        // GET: HotelBookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotelBooking = await _context.HotelBookings.FindAsync(id);
            if (hotelBooking == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CitizenshipCountry", hotelBooking.CustomerId);
            return View(hotelBooking);
        }

        // POST: HotelBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,NumberOfRooms,Date,NumberOfDays,TotalPrice,HotelRoomlId,CustomerId")] HotelBooking hotelBooking)
        {
            if (id != hotelBooking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotelBooking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelBookingExists(hotelBooking.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "CitizenshipCountry", hotelBooking.CustomerId);
            return View(hotelBooking);
        }

        // GET: HotelBookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotelBooking = await _context.HotelBookings
                .Include(h => h.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotelBooking == null)
            {
                return NotFound();
            }

            return View(hotelBooking);
        }

        // POST: HotelBookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotelBooking = await _context.HotelBookings.FindAsync(id);
            _context.HotelBookings.Remove(hotelBooking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelBookingExists(int id)
        {
            return _context.HotelBookings.Any(e => e.Id == id);
        }
    }
}
