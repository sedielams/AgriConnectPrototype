using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriConnectPrototype.Data;
using AgriConnectPrototype.Models;

namespace AgriConnectPrototype.Controllers
{
    // Handles farmer management operations
    // Allows users to create, edit, view and delete farmers

    public class FarmersController : Controller
    {
        // Database connection object
        private readonly AppDbContext _context;

        // Inject database context
        public FarmersController(AppDbContext context)
        {
            _context = context;
        }

        // Display all farmers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Farmers.ToListAsync());
        }

        // Display selected farmer details
        public async Task<IActionResult> Details(int? id)
        {
            // Check if ID exists
            if (id == null)
            {
                return NotFound();
            }

            // Search for farmer by ID
            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(m => m.FarmerId == id);

            // Farmer not found
            if (farmer == null)
            {
                return NotFound();
            }

            return View(farmer);
        }

        // Display Create Farmer page
        public IActionResult Create()
        {
            return View();
        }

        // Save new farmer to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FarmerId,FullName,ContactNumber,Email,FarmName,Region")]
            Farmer farmer)
        {
            // Validate user input
            if (ModelState.IsValid)
            {
                _context.Add(farmer);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(farmer);
        }

        // Display Edit page
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers.FindAsync(id);

            if (farmer == null)
            {
                return NotFound();
            }

            return View(farmer);
        }

        // Update farmer information
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("FarmerId,FullName,ContactNumber,Email,FarmName,Region")]
            Farmer farmer)
        {
            // Verify IDs match
            if (id != farmer.FarmerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(farmer);

                    await _context.SaveChangesAsync();
                }

                // Handle update conflicts
                catch (DbUpdateConcurrencyException)
                {
                    if (!FarmerExists(farmer.FarmerId))
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

            return View(farmer);
        }

        // Display Delete confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var farmer = await _context.Farmers
                .FirstOrDefaultAsync(m => m.FarmerId == id);

            if (farmer == null)
            {
                return NotFound();
            }

            return View(farmer);
        }

        // Remove farmer from database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);

            if (farmer != null)
            {
                _context.Farmers.Remove(farmer);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Check whether farmer exists
        private bool FarmerExists(int id)
        {
            return _context.Farmers.Any(e => e.FarmerId == id);
        }
    }
}