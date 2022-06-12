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
        public IActionResult Index(int? id, DateTime? bookingfrom, DateTime? bookingto, DateTime? paymentfrom, DateTime? paymentto, string name, double? price, string stat, string pay, int pageSize, int pageNumber, string order, string ordersort)
        {
            IQueryable<HotelBookingHeader> headers = _context.HotelBookingHeader;

            ViewBag.id = id;
            ViewBag.name = name;
            ViewBag.price = price;
            ViewBag.bookingfrom = bookingfrom;
            ViewBag.bookingto = bookingto;
            ViewBag.paymentfrom = paymentfrom;
            ViewBag.paymentto = paymentto;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageNumber;

            ViewBag.order = order;
            ViewBag.ordersort = ordersort;

            List<string> status = new() { "All", SD.Status_Approved, SD.Status_Cancelled, SD.Status_Done, SD.Status_Pending };
            List<string> payment = new() { "All", SD.Payment_Approved, SD.Payment_Pending, SD.Payment_Cancelled, SD.Payment_Refunded };

            if (!string.IsNullOrEmpty(stat))
                ViewBag.status = new SelectList(status, stat);
            else
                ViewBag.status = new SelectList(status);

            if (!string.IsNullOrEmpty(pay))
                ViewBag.payment = new SelectList(payment, pay);
            else
                ViewBag.payment = new SelectList(payment);


            if (id != null)
            {
                headers = headers.Where(h => h.Id == id) ;
            }

            if (!String.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                headers = headers.Where(h => h.Name.Contains(name)) ;
            }

            if (price != null)
            {
                headers = headers.Where(h => h.TotalPrice == price) ;
            }

            if (bookingfrom != null && bookingto != null)
            {
                headers = headers.Where(h => h.BookingDate.Date >= bookingfrom && h.BookingDate.Date <= bookingto) ;
            }
            else if (bookingfrom != null && bookingto == null)
            {
                headers = headers.Where(h => h.BookingDate.Date == bookingfrom) ;
            }
            else if (bookingfrom == null && bookingto != null)
            {
                headers = headers.Where(h => h.BookingDate.Date == bookingto) ;
            }

            if (paymentfrom != null && paymentto != null)
            {
                headers = headers.Where(h => h.PaymentDate.Date >= paymentfrom && h.PaymentDate.Date <= paymentto);
            }
            else if (paymentfrom != null && paymentto == null)
            {
                headers = headers.Where(h => h.PaymentDate.Date == paymentfrom);
            }
            else if (paymentfrom == null && paymentto != null)
            {
                headers = headers.Where(h => h.PaymentDate.Date == paymentto);
            }

            if (!String.IsNullOrWhiteSpace(stat) && stat != "All")
            {
                headers = headers.Where(h => h.Status == stat);
            } 
            
            if (!String.IsNullOrWhiteSpace(pay) && pay != "All")
            {
                headers = headers.Where(h => h.Payment == pay);
            }

            if (order == "id" && ordersort == "desc")
                headers = headers.OrderBy(h => h.Id);
            else if (order == "name" && ordersort == "desc")
                headers = headers.OrderBy(h => h.Name);
            else if (order == "bdate" && ordersort == "desc")
                headers = headers.OrderBy(h => h.BookingDate); 
            else if (order == "pdate" && ordersort == "desc")
                headers = headers.OrderBy(h => h.PaymentDate); 
            else if (order == "price" && ordersort == "desc")
                headers = headers.OrderBy(h => h.TotalPrice); 
            else if (order == "stat" && ordersort == "desc")
                headers = headers.OrderBy(h => h.Status); 
            else if (order == "pay" && ordersort == "desc")
                headers = headers.OrderBy(h => h.Payment);

            else if (order == "id")
                headers = headers.OrderByDescending(h => h.Id);
            else if (order == "name" )
                headers = headers.OrderByDescending(h => h.Name);
            else if (order == "bdate")
                headers = headers.OrderByDescending(h => h.BookingDate);
            else if (order == "pdate" )
                headers = headers.OrderByDescending(h => h.PaymentDate);
            else if (order == "price" )
                headers = headers.OrderByDescending(h => h.TotalPrice);
            else if (order == "stat")
                headers = headers.OrderByDescending(h => h.Status);
            else if (order == "pay")
                headers = headers.OrderByDescending(h => h.Payment);
          


            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";

            if (pageSize > 0 && pageNumber > 0)
            {
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;

                headers = headers.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            return View(headers.ToList());
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status, DateTime? bookingd)
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

            if (bookingd != null)
            {
                hotelBookingHeaders = _context.HotelBookingHeader.Where(h => h.BookingDate == bookingd).Include(c => c.Customer); ;
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
