using alnakhil.Data;
using alnakhil.Models;
using alnakhil.Services;
using alnakhil.ViewModels;
using alnakhil.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alnakhil.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly alnakhilContext _context;
        private readonly InventoryService _inventory;

        public PurchasesController(alnakhilContext context , InventoryService inventory)
        {
            _context = context;
            _inventory = inventory;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Items)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return View(purchases);
        }
        // ================= CREATE (GET) =================
        public async Task<IActionResult> Create()
        {
            var vm = new PurchaseVM
            {
                PurchaseDate = DateTime.Today
            };

            ViewBag.Products = await _context.Products.ToListAsync();
            return View(vm);
        }


        // ================= CREATE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseVM vm)
        {
            // التحقق من صحة البيانات الإضافية
            if (vm.Items.Any(i => i.Quantity <= 0))
            {
                ModelState.AddModelError("Items", "يجب إدخال كمية صحيحة");
            }
            if (vm.Items.Any(i => i.PurchasePrice <= 0))
            {
                ModelState.AddModelError("Items", "يجب إدخال سعر شراء صحيح");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _context.Products.ToListAsync();
                return View(vm);
            }

            // تجهيز عناصر الشراء
            var purchaseItems = new List<PurchaseItem>();

            foreach (var item in vm.Items)
            {
                Product product;

                // 🆕 منتج جديد
                if (!item.ProductId.HasValue)
                {
                    if (string.IsNullOrWhiteSpace(item.ProductName))
                        throw new Exception("اسم المنتج مطلوب");

                    product = new Product
                    {
                        Name = item.ProductName,
                        Barcode = item.Barcode,
                        CategoryId = item.CategoryId,
                        Quantity = 0,
                        PurchasePrice = item.PurchasePrice,
                        ExpirationDate = item.ExpiryDate
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    product = await _context.Products.FindAsync(item.ProductId.Value);
                    if (product == null)
                        throw new Exception("المنتج غير موجود");
                }


                purchaseItems.Add(new PurchaseItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    PurchasePrice = item.PurchasePrice,
                    ExpiryDate = item.ExpiryDate
                });
            }


            var purchase = new Purchase
            {
                PurchaseDate = vm.PurchaseDate,
                SupplierName = vm.SupplierName,
                PaymentStatus = vm.PaymentStatus,
                DueDate = vm.DueDate,
                Subtotal = vm.Subtotal,
                TaxPercentage = vm.TaxPercentage,
                TaxAmount = vm.TaxAmount,
                DiscountPercentage = vm.DiscountPercentage,
                DiscountAmount = vm.DiscountAmount,
                TotalAmount = vm.TotalAmount,
                Items = purchaseItems
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            purchase.InvoiceNumber = $"PUR-{purchase.Id:00000}";

            await _context.SaveChangesAsync(); // حفظ رقم الفاتورة


            // ✅ هنا فقط يتم تحديث المخزون
            await _inventory.ApplyPurchaseAsync(purchase.Id, User.Identity?.Name ?? "Admin");

            return RedirectToAction("Index", "Products");
        }

        
        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            return View(purchase);
        }
        
        // ================= EDIT (GET) =================
        public async Task<IActionResult> Edit(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            var vm = new PurchaseVM
            {
                Id = purchase.Id,
                SupplierName = purchase.SupplierName,
                PurchaseDate = purchase.PurchaseDate,
                PaymentStatus = purchase.PaymentStatus,
                DueDate = purchase.DueDate,
                TaxPercentage = purchase.TaxPercentage,
                DiscountPercentage = purchase.DiscountPercentage,
                Items = purchase.Items.Select(i => new PurchaseItemVM
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "غير معروف",
                    Quantity = i.Quantity,
                    PurchasePrice = i.PurchasePrice,
                    ExpiryDate = i.ExpiryDate
                }).ToList()
            };

            ViewBag.Products = await _context.Products.ToListAsync();
            return View(vm);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseVM vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            var purchase = await _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            if (purchase.PaymentStatus == PaymentStatus.Paid)
            {
                ModelState.AddModelError("", "لا يمكن تعديل فاتورة مدفوعة");
                return View(vm);
            }

            purchase.SupplierName = vm.SupplierName;
            purchase.PurchaseDate = vm.PurchaseDate;
            purchase.TaxPercentage = vm.TaxPercentage;
            purchase.DiscountPercentage = vm.DiscountPercentage;
            purchase.PaymentStatus = vm.PaymentStatus;
            purchase.DueDate = vm.DueDate;

            // حذف الأصناف القديمة
            _context.PurchaseItems.RemoveRange(purchase.Items);

            foreach (var item in vm.Items)
            {
                if (item.Quantity <= 0 || !item.ProductId.HasValue)
                    continue;

                var purchaseItem = new PurchaseItem
                {
                    ProductId = item.ProductId.Value,
                    Quantity = item.Quantity,
                    PurchasePrice = item.PurchasePrice,
                    ExpiryDate = item.ExpiryDate
                };

                purchase.Items.Add(purchaseItem);

                var product = await _context.Products.FindAsync(item.ProductId.Value);
                if (product != null)
                {
                    product.Quantity += item.Quantity;

                    _context.InventoryTransactions.Add(new InventoryTransaction
                    {
                        ProductId = product.Id,
                        QuantityChanged = item.Quantity,
                        Type = InventoryTransactionType.Purchase,
                        Date = DateTime.Now,
                        UserName = User.Identity!.Name!
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // ================= DELETE =================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null) return NotFound();

            try
            {
                await _inventory.ReversePurchaseAsync(
                    purchase,
                    User.Identity?.Name ?? "Admin"
                );

                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // البحث
        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            var products = await _context.Products
                .Where(p =>
                    p.Name.Contains(q) ||
                    p.Barcode.Contains(q))
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    barcode = p.Barcode,
                    purchasePrice = p.PurchasePrice, // أو LastPurchasePrice
                    category = p.Category.Name
                })
                .Take(10)
                .ToListAsync();

            return Json(products);
        }

    }
}
