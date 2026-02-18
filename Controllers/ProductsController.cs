using alnakhil.Data;
using alnakhil.Models;
using alnakhil.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using alnakhil.Services;
using System.Threading.Tasks;
using System.Linq;

namespace alnakhil.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly alnakhilContext _context;
        private readonly InventoryService _inventory;
        

        public ProductsController(alnakhilContext context, InventoryService inventory)
        {
            _context = context;
            _inventory = inventory;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(products);
        }
        

        // ================= EDIT (GET) =================
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var vm = new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Barcode = product.Barcode,
                Quantity = product.Quantity,
                PurchasePrice = product.PurchasePrice,
                SalePrice = product.SalePrice,
                ManualSalePrice = product.ManualSalePrice,
                IsManualPrice = product.IsManualPrice,
                ExpirationDate = product.ExpirationDate,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name
            };

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(vm);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
                return View(vm);
            }

            var product = await _context.Products.FindAsync(vm.Id);
            if (product == null) return NotFound();

            product.Name = vm.Name;
            product.Barcode = vm.Barcode;
            product.Quantity = vm.Quantity;
            product.PurchasePrice = vm.PurchasePrice;
            product.SalePrice = vm.IsManualPrice ? vm.ManualSalePrice ?? product.SalePrice : vm.SalePrice ?? product.SalePrice;
            product.ManualSalePrice = vm.IsManualPrice ? vm.ManualSalePrice : null;
            product.IsManualPrice = vm.IsManualPrice;
            product.ExpirationDate = vm.ExpirationDate;
            product.CategoryId = vm.CategoryId;
            

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE (GET) =================
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();
            return View(product);
        }

        // ================= DELETE (POST) =================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
