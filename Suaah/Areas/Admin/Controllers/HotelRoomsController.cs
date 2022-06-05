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
    public class HotelRoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelRoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HotelRooms
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HotelRooms.Include(h => h.Hotel);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HotelRooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotelRoom = await _context.HotelRooms
                .Include(h => h.Hotel)
                .Include(h => h.Services)
                  .ThenInclude(h=>h.Services)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hotelRoom == null)
            {
                return NotFound();
            }

            return View(hotelRoom);
        }

        // GET: HotelRooms/Create
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name");

            var hotelRoom = new HotelRoom
            {
                Services = new List<HotelRoomServices>()
            };

            PopulateAssignedServiceData(hotelRoom);

            return View();
        }

        // POST: HotelRooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HotelRoom hotelRoom, string[] selectedServices)
        {
            if (selectedServices != null)
            {
                hotelRoom.Services = new List<HotelRoomServices>();
                foreach (var service in selectedServices)
                {
                    var serviceToAdd = new HotelRoomServices { HotelRoomId = hotelRoom.Id, ServicesId = int.Parse(service) };

                    hotelRoom.Services.Add(serviceToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(hotelRoom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", hotelRoom.HotelId);

            PopulateAssignedServiceData(hotelRoom);
            return View(hotelRoom);
        }

        private void PopulateAssignedServiceData(HotelRoom hotelRoom)
        {
            var allServices = _context.Services;
            var RoomServices = new HashSet<int>(hotelRoom.Services.Select(c => c.ServicesId));
            var viewModel = new List<AssignedServiceData>();

            foreach (var service in allServices)
            {
                viewModel.Add(new AssignedServiceData
                {
                    ServiceID = service.Id,
                    Title = service.Name,
                    Assigned = RoomServices.Contains(service.Id)
                });
            }
            ViewData["Services"] = viewModel;
        }

        // GET: HotelRooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotelRoom = await _context.HotelRooms
                .Include(c => c.Hotel)
                .Include(s => s.Services)
                    .ThenInclude(ss => ss.Services)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotelRoom == null)
            {
                return NotFound();
            }

            PopulateAssignedServiceData(hotelRoom);
            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", hotelRoom.HotelId);
            return View(hotelRoom);
        }

        // POST: HotelRooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedServices)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotelRoom = await _context.HotelRooms
                .Include(i => i.Hotel)
                .Include(i => i.Services).ThenInclude(i => i.Services)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (await TryUpdateModelAsync<HotelRoom>(
                hotelRoom,
                "",
                /*i => i.Type,*/ i => i.Price, i => i.Description, i => i.CancelBeforeHours))
            {

                UpdateHotelRoomServices(selectedServices, hotelRoom);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["HotelId"] = new SelectList(_context.Hotels, "Id", "Name", hotelRoom.HotelId);
            UpdateHotelRoomServices(selectedServices, hotelRoom);
            PopulateAssignedServiceData(hotelRoom);
            return View(hotelRoom);
        }

        private void UpdateHotelRoomServices(string[] selectedServices, HotelRoom hotelRoom)
        {
            if (selectedServices == null)
            {
                hotelRoom.Services = new List<HotelRoomServices>();
                return;
            }

            var selectedServicesHS = new HashSet<string>(selectedServices);
            var roomServices = new HashSet<int>
                (hotelRoom.Services.Select(c => c.Services.Id));

            foreach (var service in _context.Services)
            {
                if (selectedServicesHS.Contains(service.Id.ToString()))
                {
                    if (!roomServices.Contains(service.Id))
                    {
                        hotelRoom.Services.Add(new HotelRoomServices { HotelRoomId = hotelRoom.Id, ServicesId = service.Id });
                    }
                }
                else
                {
                    if (roomServices.Contains(service.Id))
                    {
                        HotelRoomServices serviceToRemove = hotelRoom.Services.FirstOrDefault(i => i.ServicesId == service.Id);
                        _context.Remove(serviceToRemove);
                    }
                }
            }
        }

        // GET: HotelRooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotelRoom = await _context.HotelRooms
                .Include(h => h.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotelRoom == null)
            {
                return NotFound();
            }

            return View(hotelRoom);
        }

        // POST: HotelRooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotelRoom = await _context.HotelRooms.FindAsync(id);
            _context.HotelRooms.Remove(hotelRoom);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelRoomExists(int id)
        {
            return _context.HotelRooms.Any(e => e.Id == id);
        }
    }
}
