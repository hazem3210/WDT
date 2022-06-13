using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Suaah.Data;
using Suaah.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Suaah.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            HomeData homeData = new HomeData()
            {
                Airlines = _context.Airlines.Take(4),
                Countries = _context.Countries.Take(4),
                flightsCountries=_context.Countries.OrderBy( e=> ( _context.Airports.Where(c=>c.CountryId==e.ID).ToList().Count)).Take(4)
            };
            
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
           
            if (claim != null && User.IsInRole(SD.Role_Customer))
            {
                HttpContext.Session.SetInt32(SD.Session_HotelBooking, _context.HotelBookings.Where(u => u.CustomerId == claim.Value).ToList().Count);
                HttpContext.Session.SetInt32(SD.Session_FlightBooking, _context.FlightBookings.Where(u => u.CustomerId == claim.Value).ToList().Count);
            }
            else if(claim != null && (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Manager)) )
            {
                HttpContext.Session.SetInt32(SD.Session_HotelBooking, _context.HotelBookings.Where(u => u.NotCustomerId == claim.Value).ToList().Count);
            }
            else
            {
                HttpContext.Session.Clear();
            }

            return View(homeData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}