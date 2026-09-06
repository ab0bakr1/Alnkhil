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
        [Authorize(Roles = "Admin,Cashier")]
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
                PurchaseDate = DateTime.Today,
                PaymentStatus = PaymentStatus.Paid
            };

            ViewBag.Products = await _context.Products.ToListAsync();
            ViewBag.Suppliers = await _context.Suppliers.OrderBy(s => s.Name).ToListAsync();
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            return View(vm);
        }

        // ربط الفاتورة بمورد فعلي: يستخدم SupplierId لو موجود،
        // أو يبحث بالاسم، أو ينشئ مورد جديد لو الاسم جديد (زي منطق المنتج الجديد بالضبط)
        private async Task<Supplier?> ResolveSupplierAsync(int? supplierId, string? supplierName)
        {
            if (supplierId.HasValue)
            {
                var supplier = await _context.Suppliers.FindAsync(supplierId.Value);
                if (supplier != null)
                    return supplier;
            }

            if (string.IsNullOrWhiteSpace(supplierName))
                return null;

            var existing = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Name == supplierName);

            if (existing != null)
                return existing;

            var newSupplier = new Supplier { Name = supplierName };
            _context.Suppliers.Add(newSupplier);
            await _context.SaveChangesAsync();
            return newSupplier;
        }


        // ================= CREATE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseVM vm)
        {
            // التحقق من صحة البيانات الإضافية
            if (vm.Items == null || !vm.Items.Any())
            {
                ModelState.AddModelError("Items", "يجب إضافة صنف واحد على الأقل");
            }
            else
            {
                if (vm.Items.Any(i => i.Quantity <= 0))
                {
                    ModelState.AddModelError("Items", "يجب إدخال كمية صحيحة");
                }
                if (vm.Items.Any(i => i.PurchasePrice <= 0))
                {
                    ModelState.AddModelError("Items", "يجب إدخال سعر شراء صحيح");
                }
            }

            // التحقق من تاريخ الاستحقاق إذا كانت الفاتورة دين آجل
            if (vm.PaymentStatus == PaymentStatus.Deferred && !vm.DueDate.HasValue)
            {
                ModelState.AddModelError("DueDate", "يجب كتابة تاريخ الاستحقاق إذا كانت الفاتورة دين آجل");
            }

            if (vm.PaymentStatus == PaymentStatus.Paid)
            {
                vm.DueDate = null;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _context.Products.ToListAsync();
                ViewBag.Suppliers = await _context.Suppliers.OrderBy(s => s.Name).ToListAsync();
                ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                return View(vm);
            }

            // تجهيز عناصر الشراء
            var purchaseItems = new List<PurchaseItem>();

            foreach (var item in vm.Items)
            {
                Product product;

                // 🔍 البحث عن منتج موجود أولاً (بالـ ID أو بالاسم لمنع التكرار)
                if (item.ProductId.HasValue)
                {
                    product = await _context.Products.FindAsync(item.ProductId.Value);
                    if (product == null)
                        throw new Exception("المنتج غير موجود");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(item.ProductName))
                        throw new Exception("اسم المنتج مطلوب");

                    // ✅ تحقق أولاً: هل يوجد منتج بنفس الاسم؟
                    product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Name.ToLower() == item.ProductName.ToLower().Trim());

                    if (product == null)
                    {
                        // 🆕 منتج جديد فعلاً
                        product = new Product
                        {
                            Name = item.ProductName.Trim(),
                            Barcode = item.Barcode,
                            CategoryId = item.CategoryId,
                            Quantity = 0,
                            PurchasePrice = item.PurchasePrice,
                            ExpirationDate = item.ExpiryDate
                        };

                        _context.Products.Add(product);
                        await _context.SaveChangesAsync();
                    }
                }


                purchaseItems.Add(new PurchaseItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    PurchasePrice = item.PurchasePrice,
                    ExpiryDate = item.ExpiryDate
                });
            }


            var supplier = await ResolveSupplierAsync(vm.SupplierId, vm.SupplierName);

            var purchase = new Purchase
            {
                PurchaseDate = vm.PurchaseDate,
                SupplierName = supplier?.Name ?? vm.SupplierName,
                SupplierId = supplier?.Id,
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
        [Authorize(Roles = "Admin")]
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
                SupplierId = purchase.SupplierId,
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
            ViewBag.Suppliers = await _context.Suppliers.OrderBy(s => s.Name).ToListAsync();
            return View(vm);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseVM vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (vm.PaymentStatus == PaymentStatus.Deferred && !vm.DueDate.HasValue)
            {
                ModelState.AddModelError("DueDate", "يجب كتابة تاريخ الاستحقاق إذا كانت الفاتورة دين آجل");
            }

            if (vm.PaymentStatus == PaymentStatus.Paid)
            {
                vm.DueDate = null;
            }

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

            var supplier = await ResolveSupplierAsync(vm.SupplierId, vm.SupplierName);
            purchase.SupplierId = supplier?.Id;
            purchase.SupplierName = supplier?.Name ?? vm.SupplierName;
            purchase.PurchaseDate = vm.PurchaseDate;
            purchase.TaxPercentage = vm.TaxPercentage;
            purchase.DiscountPercentage = vm.DiscountPercentage;
            purchase.PaymentStatus = vm.PaymentStatus;
            purchase.DueDate = vm.DueDate;

            // 🔴 لازم نعكس تأثير الأصناف القديمة على المخزون قبل ما نحذفها،
            // وإلا الكميات القديمة تفضل مُضافة للمخزون بشكل دائم وتتضخم مع كل تعديل
            foreach (var oldItem in purchase.Items)
            {
                var oldProduct = await _context.Products.FindAsync(oldItem.ProductId);
                if (oldProduct != null)
                {
                    oldProduct.Quantity -= oldItem.Quantity;

                    _context.InventoryTransactions.Add(new InventoryTransaction
                    {
                        ProductId = oldProduct.Id,
                        QuantityChanged = -oldItem.Quantity,
                        Type = InventoryTransactionType.Purchase,
                        Date = DateTime.Now,
                        UserName = User.Identity!.Name!
                    });
                }
            }

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
        [Authorize(Roles = "Admin")]
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
            if (string.IsNullOrWhiteSpace(q))
                return Json(new object[0]);

            var products = await _context.Products
                .Where(p =>
                    p.Name.Contains(q) ||
                    (p.Barcode != null && p.Barcode.Contains(q)))
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    barcode = p.Barcode ?? "",
                    purchasePrice = p.PurchasePrice,
                    categoryId = (int?)p.CategoryId,
                    category = p.Category != null ? p.Category.Name : ""
                })
                .Take(10)
                .ToListAsync();

            return Json(products);
        }

    }
}