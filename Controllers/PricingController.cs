using alnakhil.Data;
using alnakhil.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alnakhil.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PricingController : Controller
    {
        private readonly alnakhilContext _context;

        public PricingController(alnakhilContext context)
        {
            _context = context;
        }

        // ================= PRICING PAGE =================
        public async Task<IActionResult> Index()
        {
            var data = await _context.Products
                .Select(p => new PricingVM
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    LastPurchasePrice = _context.PurchaseItems
                        .Where(pi => pi.ProductId == p.Id && pi.Quantity > 0)
                        .OrderByDescending(pi => pi.Id)
                        .Select(pi => (decimal?)(pi.PurchasePrice / pi.Quantity))
                        .FirstOrDefault() ?? p.PurchasePrice,
                    SalePrice = p.SalePrice,
                    Quantity = p.Quantity
                })
                .ToListAsync();

            return View(data);
        }


        // ================= UPDATE SALE PRICE =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int productId, decimal salePrice)
        {
            if (salePrice <= 0) return BadRequest();

            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            product.SalePrice = salePrice;
            product.IsManualPrice = true;
            product.ManualSalePrice = salePrice;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
