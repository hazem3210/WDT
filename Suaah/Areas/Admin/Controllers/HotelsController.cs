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
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public HotelsController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        // GET: Hotels
        [AllowAnonymous]
        public IActionResult Index(string hName,string hCountry, string hAddress, string hEmail, string hPhoneNumber,string hStars, string order, string ordersort, int pageSize, int pageNumber)
        {
            ViewData["hName"] = hName;
            ViewData["hAddress"] = hAddress;
            ViewData["hEmail"] = hEmail;
            ViewData["hPhoneNumber"] = hPhoneNumber;
            ViewData["hStars"] = hStars;
            ViewData["hCountry"] = hCountry;
            ViewData["pageSize"] = pageSize;
            ViewData["pageNumber"] = pageNumber;
 

            IQueryable<Hotel> hotels = _context.Hotels.Include(c=>c.Country);    

            if (!String.IsNullOrWhiteSpace(hName))
            {
                hName = hName.Trim();
                hotels = hotels.Where(h => h.Name.Contains(hName));
            }
            
           if(!String.IsNullOrWhiteSpace(hAddress))
            {
                hAddress = hAddress.Trim();
                hotels = hotels.Where(h => h.Address.Contains(hAddress));
            }
            
            if(!String.IsNullOrWhiteSpace(hCountry))
            {
                hCountry = hCountry.Trim();
                hotels = hotels.Where(h => h.Country.Name.Contains(hCountry));
            }

            if (!String.IsNullOrWhiteSpace(hEmail))
            {
                hEmail = hEmail.Trim();
                hotels = hotels.Where(h => h.Email.Contains(hEmail));
            }
            
            if (!String.IsNullOrWhiteSpace(hPhoneNumber))
            {
                hPhoneNumber = hPhoneNumber.Trim();
                hotels = hotels.Where(h => h.PhoneNumber.Contains(hPhoneNumber));
            }
          
            if (!String.IsNullOrWhiteSpace(hStars))
            {
                hStars = hStars.Trim();
                hotels = hotels.Where(h => h.Stars == hStars);
            }


            if (order == "name" && ordersort == "desc")
                hotels = hotels.OrderBy(h => h.Name);

            else if (order == "add" && ordersort == "desc")
                hotels = hotels.OrderBy(h => h.Address); 
            else if (order == "stars" && ordersort == "desc")
                hotels = hotels.OrderBy(h => h.Stars);
            else if (order == "country" && ordersort == "desc")
                hotels = hotels.OrderBy(h => h.Country.Name);

            else if (order == "name")
                hotels = hotels.OrderByDescending(h => h.Name);

            else if (order == "add")
                hotels = hotels.OrderByDescending(h => h.Address); 
            else if (order == "stars")
                hotels = hotels.OrderByDescending(h => h.Stars);
            else if (order == "country")
                hotels = hotels.OrderByDescending(h => h.Country.Name);

            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";

            if (pageSize > 0 && pageNumber > 0)
            {
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;

                hotels = hotels.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            return View(hotels.ToList());
        }

        // GET: Hotels/Details/5
        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel =  _context.Hotels
                .Where(m => m.Id == id)
                .Include(c=>c.Country)
                .Include(r=>r.HotelRooms).ThenInclude(s=>s.Services).ThenInclude(ss=>ss.Services).Single();

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // GET: Hotels/Create
        public IActionResult Create()
        {
            ViewBag.Countries = new SelectList( _context.Countries.ToList(), "ID", "Name");
            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel hotel, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                CreateFiles(hotel, image);

                _context.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Countries = new SelectList( _context.Countries.ToList(), "ID", "Name");
            return View(hotel);
        }

        protected void CreateFiles(Hotel hotel, IFormFile image = null)
        {
            if (image != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(_webHost.WebRootPath, @"img\hotel");
                var extension = Path.GetExtension(image.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    image.CopyTo(fileStreams);
                }

                hotel.ImageUrl = @"\img\hotel\" + fileName + extension;
            }
        }
        // GET: Hotels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "ID", "Name", hotel.CountryId);
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hotel hotel, IFormFile image)
        {
            if (id != hotel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null)
                    {
                        if (hotel.ImageUrl != null)
                        {
                            var oldPath = Path.Combine(_webHost.WebRootPath, hotel.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        CreateFiles(hotel, image);
                    }

                    _context.Update(hotel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.Id))
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
            ViewBag.Countries = new SelectList(await _context.Countries.ToListAsync(), "ID", "Name", hotel.CountryId);
            return View(hotel);
        }

        // GET: Hotels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);

            if (hotel.ImageUrl != null)
            {
                var oldPath = Path.Combine(_webHost.WebRootPath, hotel.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.Id == id);
        }
    }
}
