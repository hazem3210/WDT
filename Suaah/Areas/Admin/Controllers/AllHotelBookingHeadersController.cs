#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Suaah.Data;
using Suaah.Models;

namespace Suaah.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Manager + "," + SD.Role_Admin)]
    public class AllHotelBookingHeadersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AllHotelBookingHeadersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HotelBookingHeaders
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<HotelBookingHeader> hotelBookingHeaders;

            if (status != "all")
            {
                hotelBookingHeaders = _context.HotelBookingHeader.Where(h => h.Status.ToLower() == status.ToLower()).Include(c => c.Customer);
            }
            else
            {
                hotelBookingHeaders = _context.HotelBookingHeader.Include(c => c.Customer);
            }

            return Json(new { data = hotelBookingHeaders });
        }
        #endregion

        // GET: Admin/HotelBookingHeaders/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HotelBookingHeaderAndDetails HBHAD = new();

            HBHAD.HotelBookingHeader = _context.HotelBookingHeader
                .Include(c => c.Customer)
                  .ThenInclude(i => i.IdentityUser)
                .Include(d => d.HotelBookingDetails)
                   .ThenInclude(r => r.HoteRoom)
                        .ThenInclude(h => h.Hotel)
                .Include(d => d.HotelBookingDetails)
                   .ThenInclude(r => r.HoteRoom)
                        .ThenInclude(h => h.Services)
                        .ThenInclude(h => h.Services)
                .FirstOrDefault(m => m.Id == id);

            if (HBHAD.HotelBookingHeader == null)
            {
                return NotFound();
            }


            return View(HBHAD);
        }

        

        // GET: Admin/HotelBookingHeaders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotelBookingHeader = await _context.HotelBookingHeader
                .Include(h => h.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotelBookingHeader == null)
            {
                return NotFound();
            }

            return View(hotelBookingHeader);
        }

        // POST: Admin/HotelBookingHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotelBookingHeader = await _context.HotelBookingHeader.FindAsync(id);
            _context.HotelBookingHeader.Remove(hotelBookingHeader);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelBookingHeaderExists(int id)
        {
            return _context.HotelBookingHeader.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Done(int id)
        {
            var hotelBookingHeader = _context.HotelBookingHeader.FirstOrDefault(m => m.Id == id);

            if (hotelBookingHeader == null)
            {
                return NotFound();
            }

            hotelBookingHeader.Status = SD.Status_Done;
            _context.HotelBookingHeader.Update(hotelBookingHeader);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelBooking(int id)
        {
            var hotelBookingHeader = _context.HotelBookingHeader.FirstOrDefault(m => m.Id == id);

            if (hotelBookingHeader == null)
            {
                return NotFound();
            }

            if (hotelBookingHeader.Payment == SD.Payment_Approved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = hotelBookingHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                hotelBookingHeader.Status = SD.Status_Cancelled;
                hotelBookingHeader.Payment = SD.Payment_Refunded;
            }
            else
            {
                hotelBookingHeader.Status = SD.Status_Cancelled;
                hotelBookingHeader.Payment = SD.Payment_Cancelled;
            }
            _context.HotelBookingHeader.Update(hotelBookingHeader);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
