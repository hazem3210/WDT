#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Suaah.Data;
using Suaah.Models;

namespace Suaah.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class HotelBookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public HotelBookingsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: HotelBookings
        [AllowAnonymous]
        public IActionResult Index(string rdesc, float? rprice, string rhotel, string rservice)
        {
            ViewData["rdesc"] = rdesc;
            ViewData["rprice"] = rprice;
            ViewData["rhotel"] = rhotel;
            ViewData["rservice"] = rservice;

            IQueryable<HotelRoom> hotelsRooms = _context.HotelRooms
              .Include(h => h.Hotel)
              .Include(h => h.Services)
              .ThenInclude(s => s.Services); 

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (User.IsInRole(SD.Role_Customer))
            {
                HttpContext.Session.SetInt32(SD.Session_HotelBooking, _context.HotelBookings.Where(u => u.CustomerId == claim.Value).ToList().Count);
            }
            else if ((User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Manager)))
            {
                HttpContext.Session.SetInt32(SD.Session_HotelBooking, _context.HotelBookings.Where(u => u.NotCustomerId == claim.Value).ToList().Count);
            }

            if (!String.IsNullOrWhiteSpace(rdesc))
            {
                rdesc = rdesc.Trim();

                hotelsRooms = hotelsRooms.Where(r => r.Description.Contains(rdesc));
            }
            
            if (!String.IsNullOrWhiteSpace(rhotel))
            {
                rhotel = rhotel.Trim();

                hotelsRooms = hotelsRooms.Where(r => r.Hotel.Name.Contains(rhotel));
            } 
            
            if (!String.IsNullOrWhiteSpace(rservice))
            {
                rservice = rservice.Trim();

                var temp = hotelsRooms;
                List<HotelRoom> temp2 = new(); 

                foreach (var item in temp)
                {
                    foreach (var t in item.Services)
                    {
                        if (t.Services.Name.Contains(rservice))
                        {
                            temp2.Add(item);
                        }
                    }
                }

                hotelsRooms = temp2.AsQueryable(); ;
            }
            
            if(rprice != null)
            {
                hotelsRooms = hotelsRooms.Where(r => r.Price == rprice);
            }

            return View(hotelsRooms.ToList());
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
        public IActionResult Create(int roomId)
        {
        s:
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Manager))
            {
                try
                {
                    if (HttpContext.Session.GetInt32(SD.Session_HotelBooking).Value != 0)
                    {
                        var claimIdentity = (ClaimsIdentity)User.Identity;
                        var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                        var customer = _context.HotelBookings.FirstOrDefault(h => h.NotCustomerId == claim.Value).CustomerId;

                        ViewData["customerName"] = _context.Customers.FirstOrDefault(h => h.Id == customer).Name;
                    }
                }
                catch (Exception)
                {

                    goto s;
                }
            }
            ViewData["roomId"] = new SelectList(_context.HotelRooms.Where(r => r.Id == roomId), "Id", "Description");
            ViewData["customerId"] = new SelectList(_context.Customers, "Id", "Name");
            return View();
        }

        // POST: HotelBookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelBooking hotelBooking)
        {
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if (User.IsInRole(SD.Role_Customer))
                {
                    hotelBooking.CustomerId = claim.Value;
                }

                if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Manager))
                {
                    hotelBooking.NotCustomerId = claim.Value;
                s:
                    try
                    {

                        if (HttpContext.Session.GetInt32(SD.Session_HotelBooking).Value != 0)
                        {
                            hotelBooking.CustomerId = _context.HotelBookings.FirstOrDefault(h => h.NotCustomerId == claim.Value).CustomerId;
                        }
                    }
                    catch (Exception)
                    {

                        goto s;
                    }

                }

                hotelBooking.HoteRoom = _context.HotelRooms.FirstOrDefault(r => r.Id == hotelBooking.HotelRoomId);
                hotelBooking.TotalPrice = hotelBooking.NumberOfDays * hotelBooking.HoteRoom.Price * hotelBooking.NumberOfRooms;

                _context.Add(hotelBooking);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetInt32(SD.Session_HotelBooking, _context.HotelBookings.Where(u => u.CustomerId == hotelBooking.CustomerId).ToList().Count);

                return RedirectToAction(nameof(Index));
            }

            ViewData["roomId"] = new SelectList(_context.HotelRooms.Where(r => r.Id == hotelBooking.HotelRoomId), "Id", "Description");
            ViewData["customerId"] = new SelectList(_context.Customers, "Id", "Name");
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
            ViewData["customerId"] = new SelectList(_context.Customers, "Id", "Name");

            return View(hotelBooking);
        }

        // POST: HotelBookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HotelBooking hotelBooking)
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
            ViewData["customerId"] = new SelectList(_context.Customers, "Id", "Name");

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

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            HttpContext.Session.SetInt32(SD.Session_HotelBooking, _context.HotelBookings.Where(u => u.CustomerId == claim.Value).ToList().Count);

            if (hotelBooking.Flag != 0)
            {
                var HotelBookingDetails = _context.HotelBookingDetails.Where(r => r.HotelRoomId == hotelBooking.HotelRoomId);
                var HBD = HotelBookingDetails.Where(f => f.HotelBookingHeaderId == hotelBooking.Flag).Single();
                var t = HBD.HotelBookingHeaderId;
                _context.HotelBookingDetails.Remove(HBD);
                await _context.SaveChangesAsync();

                var c = _context.HotelBookingDetails.FirstOrDefault(r => r.HotelBookingHeaderId == t);
                if (c == null)
                {
                    _context.HotelBookingHeader.Remove(_context.HotelBookingHeader.Find(t));
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HotelBookingExists(int id)
        {
            return _context.HotelBookings.Any(e => e.Id == id);
        }

        public async Task<IActionResult> PendingReservation()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IQueryable<HotelBooking> applicationDbContext = default;

            if (User.IsInRole(SD.Role_Manager) || User.IsInRole(SD.Role_Admin))
            {
                applicationDbContext = _context.HotelBookings
                .Include(c => c.Customer)
                .Include(h => h.HoteRoom)
                .ThenInclude(h => h.Hotel)
                .Include(h => h.HoteRoom)
                .ThenInclude(s => s.Services)
                .ThenInclude(s => s.Services)
                .Where(id => id.NotCustomerId == claim.Value);
            }
            else if (User.IsInRole(SD.Role_Customer))
            {
                applicationDbContext = _context.HotelBookings
                .Include(c => c.Customer)
                .Include(h => h.HoteRoom)
                .ThenInclude(hh => hh.Hotel)
                .Include(h => h.HoteRoom)
                .ThenInclude(s => s.Services)
                .ThenInclude(s => s.Services)
                .Where(id => id.CustomerId == claim.Value);
            }

            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult Summary()
        {
            HotelBookingHeaderAndDetails HBHAD = new();
            HBHAD = SummaryAid(HBHAD);

            return View(HBHAD);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var HotelBookings = await _context.HotelBookings
                .Include(r => r.HoteRoom)
                  .ThenInclude(h => h.Hotel)
                  .Include(r => r.HoteRoom)
                  .ThenInclude(h => h.Services)
                  .ThenInclude(h => h.Services)
                .Where(id => id.CustomerId == claim.Value).ToListAsync();

            HotelBookingHeaderAndDetails HBHAD = new();

            if (HotelBookings[0].Flag == 0)
            {
                HBHAD = SummaryAid(HBHAD);
                _context.HotelBookingHeader.Add(HBHAD.HotelBookingHeader);
                _context.SaveChanges();

                HotelBookings[0].Flag = HBHAD.HotelBookingHeader.Id;

                for (int i = 0; i < HBHAD.HotelBookingDetails.Count; i++)
                {
                    HBHAD.HotelBookingDetails[i].HotelBookingHeaderId = HBHAD.HotelBookingHeader.Id;
                    _context.HotelBookingDetails.Add(HBHAD.HotelBookingDetails[i]);

                    HotelBookings[i].Flag = HBHAD.HotelBookingHeader.Id;

                    _context.SaveChanges();
                }

            }
            else
            {
                HBHAD.HotelBookingHeader = _context.HotelBookingHeader.FirstOrDefault(h => h.Id == HotelBookings[0].Flag);
                HBHAD.HotelBookingDetails = _context.HotelBookingDetails.Where(d => d.HotelBookingHeaderId == HotelBookings[0].Flag).ToList();
            }

            var domain = "https://localhost:7033/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Customer/HotelBookings/BookingConfirmation?id={HBHAD.HotelBookingHeader.Id}",
                CancelUrl = domain + $"Customer/HotelBookings/index",
            };

            foreach (var item in HotelBookings)
            {
                var sessionItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.HoteRoom.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = /*item.HoteRoom.Type*/item.HoteRoom.Description + ", " + item.HoteRoom.Hotel.Name,
                        },

                    },
                    Quantity = item.NumberOfDays,
                };

                options.LineItems.Add(sessionItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            HBHAD.HotelBookingHeader.SessionId = session.Id;
            HBHAD.HotelBookingHeader.PaymentIntentId = session.PaymentIntentId;
            _context.SaveChanges();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }

        public HotelBookingHeaderAndDetails SummaryAid(HotelBookingHeaderAndDetails HBHAD)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IQueryable<HotelBooking> HotelBookings = default;
            if ((User.IsInRole(SD.Role_Manager) || User.IsInRole(SD.Role_Admin)) && HttpContext.Session.GetInt32(SD.Session_HotelBooking).Value != 0)
            {
                HotelBookings = _context.HotelBookings
                .Include(c => c.Customer)
                .Include(h => h.HoteRoom)
                .ThenInclude(hh => hh.Hotel)
                .Include(h => h.HoteRoom)
                .ThenInclude(hh => hh.Services)
                .ThenInclude(hh => hh.Services)
                .Where(id => id.NotCustomerId == claim.Value);

                var list = HotelBookings.ToList();
                HBHAD.HotelBookingHeader.CustomerId = list[0].CustomerId;
                HBHAD.HotelBookingHeader.Name = _context.Customers.FirstOrDefault(id => id.Id == HBHAD.HotelBookingHeader.CustomerId).Name;
            }
            else if (User.IsInRole(SD.Role_Customer))
            {
                HotelBookings = _context.HotelBookings
                .Include(c => c.Customer)
                .Include(h => h.HoteRoom)
                .ThenInclude(hh => hh.Hotel)
                .Include(h => h.HoteRoom)
                .ThenInclude(hh => hh.Services)
                .ThenInclude(hh => hh.Services)
                .Where(id => id.CustomerId == claim.Value);

                HBHAD.HotelBookingHeader.CustomerId = claim.Value;
                HBHAD.HotelBookingHeader.Name = _context.Customers.FirstOrDefault(id => id.Id == claim.Value).Name;
            }

            HBHAD.HotelBookingHeader.TotalPrice = 0;
            int minCancelTime = 0;

            foreach (var item in HotelBookings)
            {
                HotelBookingDetails hotelBookingDetails = new();

                hotelBookingDetails.HotelRoomId = item.HotelRoomId;
                hotelBookingDetails.CheckInDate = item.Date;
                hotelBookingDetails.NumberOfDays = item.NumberOfDays;
                hotelBookingDetails.PriceForDay = item.HoteRoom.Price;
                hotelBookingDetails.HoteRoom = item.HoteRoom;
                hotelBookingDetails.HoteRoom.Hotel = item.HoteRoom.Hotel;

                HBHAD.HotelBookingHeader.TotalPrice += (hotelBookingDetails.NumberOfDays * hotelBookingDetails.PriceForDay * hotelBookingDetails.NumberOfRooms);

                HBHAD.HotelBookingDetails.Add(hotelBookingDetails);

                if (minCancelTime > 0 && item.HoteRoom.CancelBeforeHours < minCancelTime)
                {
                    minCancelTime = item.HoteRoom.CancelBeforeHours;
                }
                else
                {
                    minCancelTime = item.HoteRoom.CancelBeforeHours;
                }
            }

            HBHAD.HotelBookingHeader.BookingDate = DateTime.Now;
            HBHAD.HotelBookingHeader.PaymentDate = default;

            HBHAD.HotelBookingHeader.Status = SD.Status_Pending;
            HBHAD.HotelBookingHeader.Payment = SD.Payment_Pending;
            HBHAD.HotelBookingHeader.SessionId = "";
            HBHAD.HotelBookingHeader.PaymentIntentId = "";
            HBHAD.HotelBookingHeader.CancelBeforeHours = minCancelTime;

            return HBHAD;
        }

        public IActionResult CustomerBooking(DateTime? bookingd, DateTime? payment, int? hid, float? hprice)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            HotelBookingHeaderAndDetails HBHAD = new();

            HBHAD.HotelBookingHeaders
                 .AddRange(
                  _context.HotelBookingHeader.Where(id => id.CustomerId == claim.Value)
                  .Include(d => d.HotelBookingDetails)
                    .ThenInclude(r => r.HoteRoom)
                        .ThenInclude(r => r.Hotel));

            ViewData["hid"] = hid;
            ViewData["hprice"] = hprice;

            if (bookingd != null)
            {
                HBHAD.HotelBookingHeaders = HBHAD.HotelBookingHeaders.Where(d => d.BookingDate.Date == bookingd).ToList();
            } 
            
            if (payment != null)
            {
                HBHAD.HotelBookingHeaders = HBHAD.HotelBookingHeaders.Where(d => d.PaymentDate.Date == payment).ToList();
            } 
            
            if (hid != null)
            {
                HBHAD.HotelBookingHeaders = HBHAD.HotelBookingHeaders.Where(d => d.Id == hid).ToList();
            }
            
            if (hprice != null)
            {
                HBHAD.HotelBookingHeaders = HBHAD.HotelBookingHeaders.Where(d => d.TotalPrice == hprice).ToList();
            }

            return View(HBHAD);
        }

        public IActionResult BookingConfirmation(int id)
        {
            HotelBookingHeader hotelBookingHeader = _context.HotelBookingHeader
                .Include(c => c.Customer)
                    .ThenInclude(u => u.IdentityUser)
                .FirstOrDefault(u => u.Id == id);

            var service = new SessionService();
            Session session = service.Get(hotelBookingHeader.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                hotelBookingHeader.Status = SD.Status_Approved;
                hotelBookingHeader.Payment = SD.Payment_Approved;
                hotelBookingHeader.PaymentDate = DateTime.Now;
                _context.SaveChanges();
            }

            _emailSender.SendEmailAsync(hotelBookingHeader.Customer.IdentityUser.Email, "New Booking - Suaah", "<p>New Booking Created</p>");
            var HotelBookings = _context.HotelBookings
               .Where(id => id.CustomerId == hotelBookingHeader.Customer.Id);
            _context.HotelBookings.RemoveRange(HotelBookings);
            _context.SaveChanges();

            HttpContext.Session.Clear();
            return View(id);
        }

        public IActionResult EndBooking()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var hotelBooking = _context.HotelBookings.Where(u => u.NotCustomerId == claim.Value);

            foreach (var item in hotelBooking)
            {
                item.NotCustomerId = string.Empty;
            }

            _context.HotelBookings.UpdateRange(hotelBooking);
            _context.SaveChanges();
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "HotelBookings");
        }
    }
}
