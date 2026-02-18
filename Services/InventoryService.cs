using alnakhil.Data;
using alnakhil.Helpers;
using alnakhil.Models;
using Microsoft.EntityFrameworkCore;

namespace alnakhil.Services
{
    public class InventoryService
    {
        private readonly alnakhilContext _context;

        public InventoryService(alnakhilContext context)
        {
            _context = context;
        }

        // ================= PURCHASE =================
        public async Task ApplyPurchaseAsync(int purchaseId, string userName = "Admin")
        {
            if (purchaseId <= 0)
                throw new Exception("فاتورة الشراء غير صحيحة");

            var purchase = await _context.Purchases
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == purchaseId);

            if (purchase == null)
                throw new Exception("فاتورة الشراء غير موجودة");

            var pricing = await _context.PricingSetting.FirstOrDefaultAsync();
            var profitPercentage = pricing?.ProfitPercentage ?? 20;

            foreach (var item in purchase.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                    throw new Exception($"المنتج غير موجود: {item.ProductId}");

                var before = product.Quantity;

                // ✅ تحديث الكمية
                product.Quantity += item.Quantity;

                // ✅ سعر الوحدة
                var unitPrice = item.PurchasePrice / item.Quantity;

                // ✅ تحديث سعر الشراء
                product.PurchasePrice = item.PurchasePrice;

                // ✅ تحديث تاريخ الانتهاء
                product.ExpirationDate = item.ExpiryDate;

                // ✅ تحديث سعر البيع فقط إذا لم يكن يدوي
                if (!product.IsManualPrice)
                {
                    product.SalePrice = PricingHelper.CalculateSalePrice(
                        unitPrice,
                        profitPercentage
                    );
                }

                // ✅ تسجيل حركة المخزون
                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductId = product.Id,
                    QuantityBefore = before,
                    QuantityChanged = item.Quantity,
                    QuantityAfter = product.Quantity,
                    Type = InventoryTransactionType.Purchase,
                    UserName = userName,
                    Date = DateTime.Now,
                    Note = $"شراء - فاتورة #{purchase.Id}"
                });
            }
            
            await _context.SaveChangesAsync(); // 🔥 هذا هو المفتاح
        }

        
        // ================= SALE =================
        public async Task ApplySaleAsync(Sale sale, string userName)
        {
            foreach (var item in sale.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                    throw new Exception("المنتج غير موجود");

                if (product.Quantity < item.Quantity)
                    throw new Exception($"الكمية غير كافية: {product.Name}");

                var before = product.Quantity;

                product.Quantity -= item.Quantity;

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductId = product.Id,
                    QuantityBefore = before,
                    QuantityChanged = -item.Quantity,
                    QuantityAfter = product.Quantity,
                    Type = InventoryTransactionType.Sale,
                    UserName = userName,
                    Date = DateTime.Now,
                    Note = $"بيع - فاتورة #{sale.Id}"
                });
            }

            await _context.SaveChangesAsync();
        }

        // ================= REVERSE PURCHASE =================
        public async Task ReversePurchaseAsync(Purchase purchase, string userName)
        {
            foreach (var item in purchase.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                    throw new Exception("المنتج غير موجود");

                var before = product.Quantity;

                product.Quantity -= item.Quantity;

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductId = product.Id,
                    QuantityBefore = before,
                    QuantityChanged = -item.Quantity,
                    QuantityAfter = product.Quantity,
                    Type = InventoryTransactionType.Adjustment,
                    UserName = userName,
                    Date = DateTime.Now,
                    Note = $"إلغاء شراء - فاتورة #{purchase.Id}"
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
