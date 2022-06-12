#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Suaah.Data;
using Suaah.Models;

namespace Suaah.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Customers
        public async Task<IActionResult> Index(string? search, string? type, string? order, string? ordersort, int pageSize, int pageNumber)
        {
            List<Models.Customer> customers;
            List<string> types = new List<string>() { "Name", "Email","Phone","Passport Number" };
            ViewBag.search = search;
            if (string.IsNullOrEmpty(type))
                ViewBag.types = new SelectList(types);
            else
                ViewBag.types = new SelectList(types, type);
            if (!string.IsNullOrEmpty(order))
                ViewBag.order = order;
            if (string.IsNullOrEmpty(search))
                customers = await _context.Customers.Include(f => f.IdentityUser).ToListAsync();
            else if (!string.IsNullOrEmpty(search) && type == "Name")
                customers = await _context.Customers.Include(f => f.IdentityUser).Where(f => f.Name.ToLower().Contains(search.ToLower())).ToListAsync();
            else if (!string.IsNullOrEmpty(search) && type == "Email")
                customers = await _context.Customers.Include(f=>f.IdentityUser).Where(f => f.IdentityUser.Email.ToLower().Contains(search.ToLower())).ToListAsync();
            else if (!string.IsNullOrEmpty(search) && type == "Phone")
                customers = await _context.Customers.Include(f => f.IdentityUser).Where(f => f.IdentityUser.PhoneNumber.ToLower().Contains(search.ToLower())).ToListAsync();
            else
                customers = await _context.Customers.Include(f => f.IdentityUser).Where(f => f.PassportNumber.ToLower().Contains(search.ToLower())).ToListAsync();
            if (order == "name" && ordersort == "desc")
                customers = customers.OrderBy(f => f.Name).ToList();
            else if (order == "email" && ordersort == "desc")
                customers = customers.OrderBy(f => f.IdentityUser.Email).ToList();
            else if (order == "phone" && ordersort == "desc")
                customers = customers.OrderBy(f => f.IdentityUser.PhoneNumber).ToList();
            else if (order == "passport" && ordersort == "desc")
                customers = customers.OrderBy(f => f.PassportNumber).ToList();
            else if (order == "name")
                customers = customers.OrderByDescending(f => f.Name).ToList();
            else if (order == "email")
                customers = customers.OrderByDescending(f => f.IdentityUser.Email).ToList();
            else if (order == "phone")
                customers = customers.OrderByDescending(f => f.IdentityUser.PhoneNumber).ToList();
            else if (order == "passport")
                customers = customers.OrderByDescending(f => f.PassportNumber).ToList();
            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";

            if (pageSize > 0 && pageNumber > 0)
            {
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;

                customers = customers.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }

            return View(customers);
        }

        // GET: Customer/Customers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewBag.flights = await _context.FlightBookingDetails.Where(f => f.CustomerId == id)
                .Include(f => f.Customer).ThenInclude(e => e.IdentityUser)
                .Include(f => f.Flight).ThenInclude(e => e.FlightClasses)
                .Include(f => f.FlightClass).ThenInclude(e => e.Flights)
                .Include(f => f.Flight).ThenInclude(e => e.Airline)
                .Include(f => f.Flight).ThenInclude(e => e.ArrivingAirport).ThenInclude(d => d.Country)
                .Include(f => f.Flight).ThenInclude(e => e.DepartingAirport).ThenInclude(d => d.Country)
                .ToListAsync();
            return View(customer);
        }

        // GET: Customer/Customers/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: Customer/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Create(Models.Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
            }
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Name", customer.Id);
        }

        // GET: Customer/Customers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer =await  _context.Customers
                .FirstOrDefaultAsync(c=>c.Id ==id);

            if (customer == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Name", customer.Id);
            return View(customer);
        }

        // POST: Customer/Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Models.Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(customer.IdentityUser);
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            ViewData["Id"] = new SelectList(_context.Users, "Id", "Name", customer.Id);
            return View(customer);
        }

        // GET: Customer/Customers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await _context.Customers.Include(c => c.IdentityUser).FirstOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(customer.IdentityUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Users(string? search, string? type, string? order, string? ordersort,string? role, int pageSize, int pageNumber)
        {
            List<IdentityUser> users;
            List<string> types = new List<string>() { "User Name", "Email", "Phone"};
            List<string> roles = new List<string>() { "Customer", "Admin", "Manager" };
            ViewBag.search = search;
            if (string.IsNullOrEmpty(type))
                ViewBag.types = new SelectList(types);
            else
                ViewBag.types = new SelectList(types, type);
            if (string.IsNullOrEmpty(role))
                ViewBag.roles = new SelectList(roles);
            else
                ViewBag.roles = new SelectList(roles, role);
            if (!string.IsNullOrEmpty(order))
                ViewBag.order = order;
            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(role))
                users = await _context.Users.ToListAsync();
            else if (type == "User Name" && string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(search))
                users = await _context.Users.Where(f => f.UserName.ToLower().Contains(search.Trim().ToLower())).ToListAsync();
            else if (type == "Email" && string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(search))
                users = await _context.Users.Where(f => f.Email.ToLower().Contains(search.Trim().ToLower())).ToListAsync();
            else if (type == "Phone" && string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(search))
                users = await _context.Users.Where(f => f.PhoneNumber.ToLower().Contains(search.Trim().ToLower())).ToListAsync();
            else if (type == "User Name" && !string.IsNullOrEmpty(search))
            {
                var urole = await _context.Roles.FirstOrDefaultAsync(f => f.Name == role);
                var userrole = await _context.UserRoles.Where(f=>f.RoleId==urole.Id).Select(f=>f.UserId).ToListAsync();
                users = await _context.Users.Where(f => f.UserName.ToLower().Contains(search.Trim().ToLower())&&userrole.Contains(f.Id)).ToListAsync();
                
            }
            else if (type == "Email" && !string.IsNullOrEmpty(search))
            {
                var urole = await _context.Roles.FirstOrDefaultAsync(f => f.Name == role);
                var userrole = await _context.UserRoles.Where(f => f.RoleId == urole.Id).Select(f => f.UserId).ToListAsync();
                users = await _context.Users.Where(f => f.Email.ToLower().Contains(search.Trim().ToLower()) && userrole.Contains(f.Id)).ToListAsync();
            }
            else if (type == "Phone" && !string.IsNullOrEmpty(search))
            {
                var urole = await _context.Roles.FirstOrDefaultAsync(f => f.Name == role);
                var userrole = await _context.UserRoles.Where(f => f.RoleId == urole.Id).Select(f => f.UserId).ToListAsync();
                users = await _context.Users.Where(f => f.PhoneNumber.ToLower().Contains(search.Trim().ToLower()) && userrole.Contains(f.Id)).ToListAsync();
            }
            else if(!string.IsNullOrEmpty(role))
            {
                var urole = await _context.Roles.FirstOrDefaultAsync(f => f.Name == role);
                var userrole = await _context.UserRoles.Where(f => f.RoleId == urole.Id).Select(f => f.UserId).ToListAsync();
                users = await _context.Users.Where(f => userrole.Contains(f.Id)).ToListAsync();
            }
            else
            {
                users = await _context.Users.ToListAsync();
            }

            if (order == "name" && ordersort == "desc")
                users = users.OrderBy(f => f.UserName).ToList();
            else if (order == "email" && ordersort == "desc")
                users = users.OrderBy(f => f.Email).ToList();
            else if (order == "phone" && ordersort == "desc")
                users = users.OrderBy(f => f.PhoneNumber).ToList();
            else if (order == "role" && ordersort == "desc")
            {
                var userrole = await _context.UserRoles.OrderBy(f=>f.RoleId).Select(f=>f.UserId).ToListAsync();
                
                users = users.OrderBy(f=>userrole.IndexOf(f.Id)).ToList();
            }
            else if (order == "name")
                users = users.OrderByDescending(f => f.UserName).ToList();
            else if (order == "email")
                users = users.OrderByDescending(f => f.Email).ToList();
            else if (order == "phone")
                users = users.OrderByDescending(f => f.PhoneNumber).ToList();
            else if (order == "role")
            {
                var userrole = await _context.UserRoles.OrderByDescending(f => f.RoleId).Select(f => f.UserId).ToListAsync();

                users = users.OrderBy(f => userrole.IndexOf(f.Id)).ToList();
            }
            if (ordersort == "desc")
                ViewBag.ordersort = "asc";
            else
                ViewBag.ordersort = "desc";

            if (pageSize > 0 && pageNumber > 0)
            {
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;

                users = users.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }

            return View(users);
        } 
        
        public IActionResult DeleteAdmin(string id)
        {            
            var user = _context.Users.Find(id);
           
            return View(user);
        }

        // POST: Customer/Customers/Delete/5
        [HttpPost, ActionName("DeleteAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdminConfirmed(string id)
        {
            var User = await _context.Users.FindAsync(id);
            _context.Users.Remove(User);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
