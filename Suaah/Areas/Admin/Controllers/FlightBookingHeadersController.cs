using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Suaah.Data;
using Suaah.Models;

namespace Suaah.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class FlightBookingHeadersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightBookingHeadersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/FlightBookingHeaders
        public async Task<IActionResult> Index(string? search, string? type, string? order, string? ordersort, DateTime? timef, DateTime? timet, double? prif, double? prit,string? stats, string? pstats)
        {
            List<FlightBookingHeader> canceld = await _context.FlightBookingHeader.Where(f => f.PaymentDueDate < DateTime.Now && f.PaymentStatus == SD.Payment_Pending).ToListAsync();
            foreach(FlightBookingHeader canceldItem in canceld)
            {
                FlightBookingHeader Item = cancel(canceldItem);
                _context.FlightBookingHeader.Update(Item);
                
            }
            await _context.SaveChangesAsync();
            ViewBag.search = search;
            ViewBag.type = type;
            ViewBag.order = order;
            if(timef != null)
            ViewBag.timef=timef.GetValueOrDefault().ToString("yyyy-MM-ddThh:mm");
            if (timet != null)
                ViewBag.timet = timet.GetValueOrDefault().ToString("yyyy-MM-ddThh:mm");
            ViewBag.prif = prif;
            ViewBag.prit = prit;
            List<FlightBookingHeader> applicationDbContext=null;
            List<string> stat = new List<string>() {"All", SD.Status_Approved, SD.Status_Cancelled, SD.Status_Done, SD.Status_Pending };
            List<string> pstat = new List<string>() { "All", SD.Payment_Approved, SD.Payment_Pending, SD.Payment_Cancelled, SD.Payment_Refunded };
            List<string> types = new List<string>() { "ID","Customer" };
            if (!string.IsNullOrEmpty(stats))
                ViewBag.stats = new SelectList(stat, stats);
            else
                ViewBag.stats = new SelectList(stat);
            if (!string.IsNullOrEmpty(pstats))
                ViewBag.pstats = new SelectList(pstat, pstats);
            else
                ViewBag.pstats = new SelectList(pstat);
            if (!string.IsNullOrEmpty(type))
                ViewBag.type = new SelectList(types, type);
            else
                ViewBag.type = new SelectList(types);
            if (!string.IsNullOrEmpty(search))
            {
                if (!User.IsInRole(SD.Role_Customer))
                {
                    if (type == "ID")
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.ID.ToString().Contains(search.Trim())).Include(f => f.Customer).ToListAsync();
                    }
                    else if (type == "Customer")
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Include(f => f.Customer).Where(f => f.Customer.Name.Contains(search.Trim().ToLower())).ToListAsync();
                    }
                }
                else
                {
                    var claimsId = (ClaimsIdentity)User.Identity;
                    var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                    if (type == "ID")
                    {
                        
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.ID.ToString().Contains(search.Trim()) && f.CustomerID==claim.Value).Include(f => f.Customer).ToListAsync();
                    }
                    else if (type == "Customer")
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Include(f => f.Customer).Where(f => f.Customer.Name.Contains(search.Trim().ToLower()) && f.CustomerID == claim.Value ).ToListAsync();
                    }
                }
            }
            if(applicationDbContext == null)
            {
                if(!User.IsInRole(SD.Role_Customer))
                {
                    if(timef.HasValue&& timet.HasValue)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.OrderDate>=timef && f.OrderDate<timet).Include(f => f.Customer).ToListAsync();
                    }
                    else if (!timef.HasValue && timet.HasValue)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.OrderDate < timet).Include(f => f.Customer).ToListAsync();
                    }
                    else if (timef.HasValue && !timet.HasValue)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.OrderDate >= timef).Include(f => f.Customer).ToListAsync();
                    }
                    else if (prif!=null && prit!=null)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.OrderTotal>=prif && f.OrderTotal<prit).Include(f => f.Customer).ToListAsync();
                    }
                    else if (prif != null && prit == null)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.OrderTotal >= prif).Include(f => f.Customer).ToListAsync();
                    }
                    else if (prif == null && prit != null)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.OrderTotal < prit).Include(f => f.Customer).ToListAsync();
                    }
                    else if (stats!="All" && pstats!="All")
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.OrderStatus.Contains(stats) && f.PaymentStatus.Contains(pstats)).Include(f => f.Customer).ToListAsync();
                    }
                    else if (stats == "All" && pstats != "All")
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.PaymentStatus.Contains(pstats)).Include(f => f.Customer).ToListAsync();
                    }
                    else if (stats != "All" && pstats == "All")
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.OrderStatus.Contains(stats)).Include(f => f.Customer).ToListAsync();
                    }
                }
                else
                {
                    var claimsId = (ClaimsIdentity)User.Identity;
                    var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                    if (timef.HasValue && timet.HasValue)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.CustomerID == claim.Value && f.OrderDate >= timef && f.OrderDate < timet).Include(f => f.Customer).ToListAsync();
                    }
                    else if (!timef.HasValue && timet.HasValue)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.CustomerID == claim.Value && f.OrderDate < timet).Include(f => f.Customer).ToListAsync();
                    }
                    else if (timef.HasValue && !timet.HasValue)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.CustomerID == claim.Value && f.OrderDate >= timef).Include(f => f.Customer).ToListAsync();
                    }
                    else if (prif != null && prit != null)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.CustomerID == claim.Value && f.OrderTotal >= prif && f.OrderTotal < prit).Include(f => f.Customer).ToListAsync();
                    }
                    else if (prif != null && prit == null)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.CustomerID == claim.Value && f.OrderTotal >= prif).Include(f => f.Customer).ToListAsync();
                    }
                    else if (prif == null && prit != null)
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.CustomerID == claim.Value && f.OrderTotal < prit).Include(f => f.Customer).ToListAsync();
                    }
                    else if (!string.IsNullOrEmpty(stats) && !string.IsNullOrEmpty(pstats))
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.CustomerID == claim.Value && f.OrderStatus.Contains(stats) && f.PaymentStatus.Contains(pstats)).Include(f => f.Customer).ToListAsync();
                    }
                    else if (string.IsNullOrEmpty(stats) && !string.IsNullOrEmpty(pstats))
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.CustomerID == claim.Value && f.PaymentStatus.Contains(pstats)).Include(f => f.Customer).ToListAsync();
                    }
                    else if (!string.IsNullOrEmpty(stats) && string.IsNullOrEmpty(pstats))
                    {
                        applicationDbContext = await _context.FlightBookingHeader.Where(f => f.CustomerID == claim.Value && f.OrderStatus.Contains(stats)).Include(f => f.Customer).ToListAsync();
                    }
                }
            }
            if(applicationDbContext != null)
            {
                if (timef.HasValue && timet.HasValue)
                {
                    applicationDbContext = applicationDbContext.Where(f => f.OrderDate >= timef && f.OrderDate < timet).ToList();
                }
                else if (!timef.HasValue && timet.HasValue)
                {
                    applicationDbContext = applicationDbContext.Where(f => f.OrderDate < timet).ToList();
                }
                else if (timef.HasValue && !timet.HasValue)
                {
                    applicationDbContext = applicationDbContext.Where(f => f.OrderDate >= timef).ToList();
                }
                 if (prif != null && prit != null)
                {
                    applicationDbContext = applicationDbContext.Where(f => f.OrderTotal >= prif && f.OrderTotal < prit).ToList();
                }
                else if (prif != null && prit == null)
                {
                    applicationDbContext = applicationDbContext.Where(f => f.OrderTotal >= prif).ToList();
                }
                else if (prif == null && prit != null)
                {
                    applicationDbContext = applicationDbContext.Where(f => f.OrderTotal < prit).ToList();
                }
                 if (!string.IsNullOrEmpty(stats) && !string.IsNullOrEmpty(pstats))
                {
                    applicationDbContext = applicationDbContext.Where(f => f.OrderStatus.Contains(stats) && f.PaymentStatus.Contains(pstats)).ToList();
                }
                else if (string.IsNullOrEmpty(stats) && !string.IsNullOrEmpty(pstats))
                {
                    applicationDbContext = applicationDbContext.Where(f => f.PaymentStatus.Contains(pstats)).ToList();
                }
                else if (!string.IsNullOrEmpty(stats) && string.IsNullOrEmpty(pstats))
                {
                    applicationDbContext = applicationDbContext.Where(f => f.OrderStatus.Contains(stats)).ToList();
                }
            }
            if(applicationDbContext == null)
            {
                if(!User.IsInRole(SD.Role_Customer))
                {
                    applicationDbContext = await _context.FlightBookingHeader.ToListAsync();
                }
                else
                {
                    var claimsId = (ClaimsIdentity)User.Identity;
                    var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                    applicationDbContext = await _context.FlightBookingHeader.Where(f=>f.CustomerID==claim.Value).ToListAsync();
                }

            }
            if (order == "ID" && ordersort == "desc")
                applicationDbContext = applicationDbContext.OrderBy(f => f.ID).ToList();
            else if (order == "Customer" && ordersort == "desc")
                applicationDbContext = applicationDbContext.OrderBy(f => f.Customer.Name).ToList();
            else if (order == "OrderDate" && ordersort == "desc")
                applicationDbContext = applicationDbContext.OrderBy(f => f.OrderDate).ToList();
            else if (order == "OrderTotal" && ordersort == "desc")
                applicationDbContext = applicationDbContext.OrderBy(f => f.OrderTotal).ToList();
            else if (order == "OrderStatus" && ordersort == "desc")
                applicationDbContext = applicationDbContext.OrderBy(f => f.OrderStatus).ToList();
            else if (order == "PaymentStatus" && ordersort == "desc")
                applicationDbContext = applicationDbContext.OrderBy(f => f.PaymentStatus).ToList();
            else if (order == "ID" )
                applicationDbContext = applicationDbContext.OrderByDescending(f => f.ID).ToList();
            else if (order == "Customer" )
                applicationDbContext = applicationDbContext.OrderByDescending(f => f.Customer.Name).ToList();
            else if (order == "OrderDate" )
                applicationDbContext = applicationDbContext.OrderByDescending(f => f.OrderDate).ToList();
            else if (order == "OrderTotal" )
                applicationDbContext = applicationDbContext.OrderByDescending(f => f.OrderTotal).ToList();
            else if (order == "OrderStatus" )
                applicationDbContext = applicationDbContext.OrderByDescending(f => f.OrderStatus).ToList();
            else if (order == "PaymentStatus")
                applicationDbContext = applicationDbContext.OrderByDescending(f => f.PaymentStatus).ToList();
            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";
            return View(applicationDbContext);
        }

        // GET: Admin/FlightBookingHeaders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FlightBookingHeader == null)
            {
                return NotFound();
            }

            var flightBookingHeader = await _context.FlightBookingHeader
                .Include(f => f.Customer).ThenInclude(g=>g.IdentityUser)
                .FirstOrDefaultAsync(m => m.ID == id);
            var flights=await _context.FlightBookingDetails.Include(g => g.Flight).ThenInclude(h => h.Airline)
                .Include(g => g.Flight).ThenInclude(h => h.FlightClasses)
                .Include(g => g.Flight).ThenInclude(h => h.ArrivingAirport).ThenInclude(i => i.Country)
                .Include(g => g.Flight).ThenInclude(h => h.DepartingAirport).ThenInclude(i => i.Country)
                .Include(g => g.FlightClass).Where(f=>f.OrderID==id).ToListAsync();
            if (flightBookingHeader == null)
            {
                return NotFound();
            }
            flightBookingHeader.FlightBookings = flights;

            return View(flightBookingHeader);
        }

        // GET: Admin/FlightBookingHeaders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FlightBookingHeader == null)
            {
                return NotFound();
            }

            var flightBookingHeader = await _context.FlightBookingHeader
                .Include(f => f.Customer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (flightBookingHeader == null)
            {
                return NotFound();
            }

            return View(flightBookingHeader);
        }

        // POST: Admin/FlightBookingHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FlightBookingHeader == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FlightBookingHeader'  is null.");
            }
            var flightBookingHeader = await _context.FlightBookingHeader.FindAsync(id);
            if (flightBookingHeader != null)
            {
                _context.FlightBookingHeader.Remove(flightBookingHeader);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightBookingHeaderExists(int id)
        {
          return (_context.FlightBookingHeader?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Done(int id)
        {
            FlightBookingHeader? bookingHeader = await _context.FlightBookingHeader.FirstOrDefaultAsync(f => f.ID == id);


            if (bookingHeader == null)
            {
                return NotFound();
            }

            bookingHeader.OrderStatus = SD.Status_Done;
            _context.FlightBookingHeader.Update(bookingHeader);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            FlightBookingHeader? bookingHeader =await _context.FlightBookingHeader.FirstOrDefaultAsync(f=>f.ID==id);

            if (bookingHeader == null)
            {
                return NotFound();
            }
            bookingHeader = cancel(bookingHeader);
            
            _context.FlightBookingHeader.Update(bookingHeader);
              await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }

        private FlightBookingHeader cancel(FlightBookingHeader bookingHeader)
        {
            if (bookingHeader.PaymentStatus == SD.Payment_Approved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = bookingHeader.PaymentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                bookingHeader.OrderStatus = SD.Status_Cancelled;
                bookingHeader.PaymentStatus = SD.Payment_Refunded;
            }
            else
            {
                bookingHeader.OrderStatus = SD.Status_Cancelled;
                bookingHeader.PaymentStatus = SD.Payment_Cancelled;
            }
            return bookingHeader;
        }
        public async Task<IActionResult> Pay(int id)
        {
         
                FlightBookingHeader header = await _context.FlightBookingHeader
                .Include(f => f.Customer).ThenInclude(g => g.IdentityUser)
                .FirstOrDefaultAsync(m => m.ID == id);
            var flights = await _context.FlightBookingDetails.Include(g => g.Flight).ThenInclude(h => h.Airline)
                .Include(g => g.Flight).ThenInclude(h => h.FlightClasses)
                .Include(g => g.Flight).ThenInclude(h => h.ArrivingAirport).ThenInclude(i => i.Country)
                .Include(g => g.Flight).ThenInclude(h => h.DepartingAirport).ThenInclude(i => i.Country)
                .Include(g => g.FlightClass).Where(f => f.OrderID == id).ToListAsync();
            header.FlightBookings = flights;
            if (header.PaymentStatus == SD.Payment_Pending && header.PaymentDueDate > DateTime.Now) 
            {
                string domain = "https://localhost:44310/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                    LineItems = new List<SessionLineItemOptions>()
                    ,
                    Mode = "payment",
                    SuccessUrl = domain + $"Admin/FlightBookingHeaders/OrderConfirm?id={header.ID}",
                    CancelUrl = domain + $"Admin/FlightBookingHeaders/index",
                };
                foreach (var order in header.FlightBookings)
                {

                    var SessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long?)((order.TotalPrice / order.NumberOfSeats) * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Flight to " + order.Flight.ArrivingAirport.City + " ," + order.Flight.ArrivingAirport.Country.Name,
                                Description = "Class: " + order.FlightClass.Class
                            },

                        },
                        Quantity = order.NumberOfSeats,
                    };
                    options.LineItems.Add(SessionLineItem);

                }

                var service = new SessionService();
                Session session = service.Create(options);
                header.SessionId = session.Id;
                header.PaymentId = session.PaymentIntentId;
                _context.FlightBookingHeader.Update(header);
                await _context.SaveChangesAsync();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction("Details", id);
        }
        public async Task<IActionResult> OrderConfirm(int id)
        {
            FlightBookingHeader header = await _context.FlightBookingHeader.FirstOrDefaultAsync(f => f.ID == id);
            var service = new SessionService();
            var session = await service.GetAsync(header.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                header.OrderStatus = SD.Status_Approved;
                header.PaymentStatus = SD.Payment_Approved;
                header.PaymentDate = DateTime.Now;
                _context.FlightBookingHeader.Update(header);
                await _context.SaveChangesAsync();
            }
            return View(id);
        }
    }
}
