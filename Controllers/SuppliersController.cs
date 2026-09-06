using alnakhil.Data;
using alnakhil.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alnakhil.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SuppliersController : Controller
    {
        private readonly alnakhilContext _context;

        public SuppliersController(alnakhilContext context)
        {
            _context = context;
        }

        // ================== INDEX ==================
        public async Task<IActionResult> Index()
        {
            var suppliers = await _context.Suppliers
                .Include(s => s.Purchases)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(suppliers);
        }

        // ================== CREATE ==================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            var exists = await _context.Suppliers
                .AnyAsync(s => s.Name == supplier.Name);

            if (exists)
            {
                ModelState.AddModelError(nameof(Supplier.Name), "يوجد مورد بنفس الاسم بالفعل");
                return View(supplier);
            }

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================== EDIT ==================
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(supplier);
            }

            var duplicate = await _context.Suppliers
                .AnyAsync(s => s.Name == supplier.Name && s.Id != id);

            if (duplicate)
            {
                ModelState.AddModelError(nameof(Supplier.Name), "يوجد مورد بنفس الاسم بالفعل");
                return View(supplier);
            }

            var existing = await _context.Suppliers.FindAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = supplier.Name;
            existing.Phone = supplier.Phone;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}