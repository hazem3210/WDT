﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Suaah.Data;
using Suaah.Models;

namespace Suaah.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly  IWebHostEnvironment _hostEnvi;


        public CountriesController(ApplicationDbContext context,IWebHostEnvironment hostEnvi)
        {
            _context = context;
            _hostEnvi = hostEnvi;
        }

        // GET: Admin/Countries
        public async Task<IActionResult> Index(string? name,string? order, int pageSize, int pageNumber)
        {
           
            List<Country> countries;
            if (string.IsNullOrEmpty(name))
                countries = await _context.Countries.ToListAsync();
            else
                countries = await _context.Countries.Where(x => x.Name.ToLower().Contains(name.Trim().ToLower())).ToListAsync();
                ViewBag.name = name;
            if (order == "desc")
            {
                countries = countries.OrderByDescending(f => f.Name).ToList();
                ViewBag.order = "asc";
            }
            else
            {
                countries = countries.OrderBy(f => f.Name).ToList();
                ViewBag.order = "desc";
            }

            if (pageSize > 0 && pageNumber > 0)
            {
                ViewBag.PageSize = pageSize;
                ViewBag.PageNumber = pageNumber;

                countries = countries.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }

            return View(countries);
        }

        // GET: Admin/Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.ID == id);
            if (country == null)
            {
                return NotFound();
            }
            ViewBag.airports = await _context.Airports.Where(f => f.CountryId == id).Include(f => f.Country).ToListAsync();
            return View(country);
        }

        // GET: Admin/Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Country country)
        {
            if (ModelState.IsValid)
            {
                if (country.Photo == null)
                {
                    ModelState.AddModelError("Photo","You have to Enter a Photo");
                    return View(country);
                }
                string webpath = _hostEnvi.WebRootPath;
                Guid guid= Guid.NewGuid();
                string exe=Path.GetExtension(country.Photo.FileName);
                string filename=guid+exe;
                FileStream stream = System.IO.File.Create(webpath+"\\img\\Country\\"+filename);
                country.Photo.CopyTo(stream);
                await stream.DisposeAsync();
                country.PhotoPath = "\\img\\Country\\" + filename;

                await _context.AddAsync(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Admin/Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Admin/Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,PhotoPath")] Country country)
        {
            if (id != country.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (country.Photo != null)
                {
                    
                    string webpath = _hostEnvi.WebRootPath;
                    System.IO.File.Delete(webpath+ country.PhotoPath);
                    Guid guid = Guid.NewGuid();
                    string exe = Path.GetExtension(country.Photo.FileName);
                    string filename = guid + exe;
                    FileStream stream = System.IO.File.Create(webpath + "\\img\\Country\\" + filename);
                    country.Photo.CopyTo(stream);
                    await stream.DisposeAsync();
                    country.PhotoPath = "\\img\\Country\\" + filename;

                }
                else
                {
                    Country country1 = await _context.Countries.FindAsync(country.ID);
                    country.PhotoPath=country1.PhotoPath;
                }
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.ID))
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
            return View(country);
        }

        // GET: Admin/Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.ID == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Admin/Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            string webpath = _hostEnvi.WebRootPath;
            System.IO.File.Delete(webpath + country.PhotoPath);
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.ID == id);
        }
    }
}
