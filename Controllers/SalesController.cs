using alnakhil.Data;
using alnakhil.Models;
using alnakhil.Services;
using alnakhil.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alnakhil.Controllers
{
    public class SalesController : Controller
    {
        private readonly alnakhilContext _context;
        private readonly InventoryService _inventory;

        public SalesController(
            alnakhilContext context,
            InventoryService inventory)
        {
            _context = context;
            _inventory = inventory;
        }

        // ================= INDEX =================
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            return View(sales);
        }

        // ================= DETAILS =================
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Details(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                return NotFound();

            return View(sale);
        }

        // ================= CREATE (GET) =================
        [Authorize(Roles = "Admin,Cashier")]
        public IActionResult Create()
        {
            return View(new SaleVM());
        }

        // ================= CREATE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Create(SaleVM vm)
        {
            if (!ModelState.IsValid)
            {
                // هذا السطر سيجمع كل الأخطاء ويرسلها لك لتراها في المتصفح
                var errors = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(errors); 
            }

            var pricing = await _context.PricingSetting.FirstOrDefaultAsync()
                          ?? new PricingSetting { TaxPercentage = 0 };

            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                InvoiceNumber = GenerateInvoiceNumber("S"),
                CustomerName = vm.CustomerName,
                PaymentStatus = vm.PaymentStatus,
                DueDate = vm.DueDate,
                DiscountAmount = vm.DiscountAmount,
                Items = new List<SaleItem>()
            };

            foreach (var item in vm.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                {
                    ModelState.AddModelError("", "منتج غير موجود");
                    return View(vm);
                }

                if (product.Quantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"الكمية غير كافية للمنتج {product.Name}");
                    return View(vm);
                }

                // ✅ السعر النهائي (يدوي ← تلقائي)
                var finalPrice = product.ManualSalePrice ?? product.SalePrice;

                sale.Items.Add(new SaleItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = finalPrice
                });
            }

            // الحسابات
            sale.Subtotal = sale.Items.Sum(i => i.Quantity * i.UnitPrice);
            sale.TaxAmount = sale.Subtotal * (pricing.TaxPercentage / 100);
            sale.TotalAmount = sale.Subtotal + sale.TaxAmount - sale.DiscountAmount;

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // خصم المخزون + تسجيل الحركة
            await _inventory.ApplySaleAsync(
                sale,
                User.Identity?.Name ?? "Cashier"
            );

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // ================= Suspend =================
        [HttpPost]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Suspend([FromBody] SaleVM vm)
        {
            if (vm == null)
                return BadRequest("VM IS NULL");

            if (vm.Items == null)
                return BadRequest("ITEMS IS NULL");

            // 🔥 تنظيف العناصر null
            vm.Items = vm.Items.Where(i => i != null).ToList();

            if (!vm.Items.Any())
                return BadRequest("NO VALID ITEMS");

            if (vm.Items.Any(i => i.ProductId == 0))
                return BadRequest("INVALID PRODUCT ID");

            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                InvoiceNumber = GenerateInvoiceNumber("H"),
                CustomerName = vm.CustomerName,
                PaymentStatus = PaymentStatus.Suspended,
                IsSuspended = true,
                DiscountAmount = vm.DiscountAmount,
                Items = vm.Items.Select(i => new SaleItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }


        // ================= SEARCH PRODUCTS =================
        [Authorize(Roles = "Admin,Cashier")]
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new List<object>());

            var products = await _context.Products
                .Where(p => p.Name.Contains(query) || p.Barcode.Contains(query))
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    barcode = p.Barcode,
                    salePrice = (double)(p.ManualSalePrice ?? p.SalePrice),
                    quantity = p.Quantity
                })

                .Take(10)
                .ToListAsync();

            return Json(products);
        }

        // ================= EDIT (GET) =================
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sales
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            var vm = new SaleVM
            {
                Id = sale.Id,
                CustomerName = sale.CustomerName,
                PaymentStatus = sale.PaymentStatus,
                DiscountAmount = sale.DiscountAmount,
                Items = sale.Items.Select(i => new SaleItemVM
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    ProductBarcode = i.Product.Barcode
                }).ToList()
            };

            ViewBag.Products = _context.Products.ToList();
            return View(vm);
        }

        // ================= EDIT (POST) =================
        [Authorize(Roles = "Admin,Cashier")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SaleVM vm)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == vm.Id);

            if (sale == null)
                return NotFound();

            // 🟡 إذا كانت الفاتورة معلقة سابقًا وتحولت لفاتورة عادية
            if (sale.IsSuspended && vm.PaymentStatus != PaymentStatus.Suspended)
            {
                sale.IsSuspended = false;

                // توليد رقم فاتورة بيع عادي
                sale.InvoiceNumber = GenerateInvoiceNumber("S");

                // تحديث تاريخ البيع
                sale.SaleDate = DateTime.Now;
            }

            // 🔴 الحالة القديمة
            var oldStatus = sale.PaymentStatus;

            // تحديث البيانات
            sale.CustomerName = vm.CustomerName;
            sale.PaymentStatus = vm.PaymentStatus;
            sale.DiscountAmount = vm.DiscountAmount;

            // 🧠 خريطة العناصر القادمة من الفيو
            var vmItems = vm.Items.ToDictionary(i => i.ProductId);

            // 🧠 خريطة العناصر الحالية في الفاتورة
            var dbItems = sale.Items.ToDictionary(i => i.ProductId);


            // ================== ❌ حذف منتجات ==================

            var removedItems = sale.Items
                .Where(i => !vmItems.ContainsKey(i.ProductId))
                .ToList();

            foreach (var removed in removedItems)
            {
                // لو كانت الفاتورة مدفوعة → رجّع المخزون
                if (oldStatus == PaymentStatus.Paid)
                {
                    removed.Product.Quantity += removed.Quantity;
                }

                _context.SaleItems.Remove(removed);
            }


            // ================== 🔄 تحديث الكميات ==================
            foreach (var dbItem in sale.Items)
            {
                if (vmItems.TryGetValue(dbItem.ProductId, out var vmItem))
                {
                    var diff = vmItem.Quantity - dbItem.Quantity;

                    // لو مدفوعة → تحقق من المخزون
                    if (sale.PaymentStatus == PaymentStatus.Paid && diff > 0)
                    {
                        if (dbItem.Product.Quantity < diff)
                        {
                            ModelState.AddModelError("",
                                $"المخزون غير كافٍ للمنتج {dbItem.Product.Name}");
                            return View(vm);
                        }

                        dbItem.Product.Quantity -= diff;
                    }

                    // لو كانت مدفوعة سابقًا وتخفّضت الكمية
                    if (oldStatus == PaymentStatus.Paid && diff < 0)
                    {
                        dbItem.Product.Quantity += Math.Abs(diff);
                    }

                    dbItem.Quantity = vmItem.Quantity;
                }
            }


            // ================== ➕ إضافة منتجات جديدة ==================
            var newItems = vm.Items
                .Where(i => !dbItems.ContainsKey(i.ProductId))
                .ToList();

            foreach (var newItem in newItems)
            {
                var product = await _context.Products.FindAsync(newItem.ProductId);
                if (product == null)
                    continue;

                if (sale.PaymentStatus == PaymentStatus.Paid)
                {
                    if (product.Quantity < newItem.Quantity)
                    {
                        ModelState.AddModelError("",
                            $"المخزون غير كافٍ للمنتج {product.Name}");
                        return View(vm);
                    }

                    product.Quantity -= newItem.Quantity;
                }

                sale.Items.Add(new SaleItem
                {
                    ProductId = product.Id,
                    Quantity = newItem.Quantity,
                    UnitPrice = newItem.UnitPrice
                });
            }

            foreach (var item in vm.Items)
            {
                item.ProductName = _context.Products
                    .Where(p => p.Id == item.ProductId)
                    .Select(p => p.Name)
                    .FirstOrDefault();
            }


            // 6️⃣ إعادة الحساب
            var settings = await _context.PricingSetting.FirstOrDefaultAsync()
                        ?? new PricingSetting { TaxPercentage = 15 };

            sale.Subtotal = sale.Items.Sum(i => i.Quantity * i.UnitPrice);
            sale.TaxAmount = sale.Subtotal * (settings.TaxPercentage / 100);
            sale.TotalAmount = sale.Subtotal + sale.TaxAmount - sale.DiscountAmount;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE =================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                return NotFound();

            // إعادة المخزون
            foreach (var item in sale.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                    product.Quantity += item.Quantity;
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= INVOICE NUMBER =================
        private string GenerateInvoiceNumber(string prefix)
        {
            var last = _context.Sales
                .Where(s => s.InvoiceNumber.StartsWith(prefix))
                .OrderByDescending(s => s.Id)
                .FirstOrDefault();

            int next = 1;

            if (last != null)
            {
                var number = last.InvoiceNumber.Substring(prefix.Length);
                if (int.TryParse(number, out int n))
                    next = n + 1;
            }

            return $"{prefix}{next:D6}";
        }
    }
}
