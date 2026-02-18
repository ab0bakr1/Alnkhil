using alnakhil.Data;
using alnakhil.ViewModels;
using alnakhil.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alnakhil.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly alnakhilContext _context;

        public DashboardController(alnakhilContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var vm = new DashboardVM
            {
                ProductsCount = await _context.Products.CountAsync(),
                LowStockCount = await _context.Products
                    .CountAsync(p => p.Quantity <= 5),

                TodaySales = await _context.Sales
                    .Where(s => s.SaleDate.Date == today)
                    .SumAsync(s => (decimal?)s.TotalAmount) ?? 0,

                TodayPurchases = await _context.Purchases
                    .Where(p => p.PurchaseDate.Date == today)
                    .SumAsync(p => (decimal?)p.TotalAmount) ?? 0,

                ReceivableDebts = await _context.Sales
                    .Where(s => s.PaymentStatus != PaymentStatus.Paid)
                    .SumAsync(s => (decimal?)(s.TotalAmount - s.AmountPaid)) ?? 0,

                PayableDebts = await _context.Purchases
                    .Where(p => p.PaymentStatus != PaymentStatus.Paid)
                    .SumAsync(p => (decimal?)(p.TotalAmount - p.AmountPaid)) ?? 0
            };

            // ===== آخر 6 أشهر =====
            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                vm.Months.Add(month.ToString("MM-yyyy"));

                vm.SalesByMonth.Add(
                    await _context.Sales
                        .Where(s => s.SaleDate.Month == month.Month &&
                                    s.SaleDate.Year == month.Year)
                        .SumAsync(s => (decimal?)s.TotalAmount) ?? 0
                );

                vm.PurchasesByMonth.Add(
                    await _context.Purchases
                        .Where(p => p.PurchaseDate.Month == month.Month &&
                                    p.PurchaseDate.Year == month.Year)
                        .SumAsync(p => (decimal?)p.TotalAmount) ?? 0
                );
            }

            return View(vm);
        }

        // احصائيات اليوم
        public async Task<IActionResult> Today()
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            var salesToday = await _context.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .Where(s => s.SaleDate.Date == today)
                .ToListAsync();

            var salesYesterday = await _context.Sales
                .Where(s => s.SaleDate.Date == yesterday)
                .ToListAsync();

            // ===== رسم بياني حسب الساعة =====
            var hourly = salesToday
                .GroupBy(s => s.SaleDate.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    Total = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Hour)
                .ToList();

            // ===== أكثر 5 منتجات مبيعاً =====
            var topProducts = salesToday
                .SelectMany(s => s.Items)
                .GroupBy(i => i.Product.Name)
                .Select(g => new TopProductVM
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    Total = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToList();

            var vm = new TodayStatsVM
            {
                TotalSales = salesToday.Sum(s => s.TotalAmount),
                SalesCount = salesToday.Count,

                YesterdaySales = salesYesterday.Sum(s => s.TotalAmount),

                Hours = hourly.Select(h => $"{h.Hour}:00").ToList(),
                HourlySales = hourly.Select(h => h.Total).ToList(),

                TopProducts = topProducts
            };

            return View(vm);
        }

        // احصائيات الشهر 
        public async Task<IActionResult> Monthly(int? year, int? month)
        {
            var now = DateTime.Now;

            int selectedYear = year ?? now.Year;
            int selectedMonth = month ?? now.Month;

            var startDate = new DateTime(selectedYear, selectedMonth, 1);
            var endDate = startDate.AddMonths(1);

            var lastMonthStart = startDate.AddMonths(-1);
            var lastMonthEnd = startDate;

            // ===== مبيعات الشهر الحالي =====
            var salesThisMonth = await _context.Sales
                .Include(s => s.Items)
                    .ThenInclude(i => i.Product)
                .Where(s => s.SaleDate >= startDate && s.SaleDate < endDate)
                .ToListAsync();

            // ===== مبيعات الشهر السابق =====
            var salesLastMonth = await _context.Sales
                .Where(s => s.SaleDate >= lastMonthStart && s.SaleDate < lastMonthEnd)
                .ToListAsync();

            // ===== المبيعات حسب اليوم =====
            var dailySales = salesThisMonth
                .GroupBy(s => s.SaleDate.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    Total = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Day)
                .ToList();

            // ===== أكثر 5 منتجات مبيعاً =====
            var topProducts = salesThisMonth
                .SelectMany(s => s.Items)
                .GroupBy(i => i.Product.Name)
                .Select(g => new TopProductVM
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    Total = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToList();



            var purchasesThisMonth = await _context.Purchases
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate < endDate)
                .ToListAsync();

            var salesDebts = salesThisMonth
                .Where(s => s.PaymentStatus != PaymentStatus.Paid)
                .Sum(s => s.TotalAmount);

            var purchaseDebts = purchasesThisMonth
                .Where(p => p.PaymentStatus != PaymentStatus.Paid)
                .Sum(p => p.TotalAmount);


            var vm = new MonthlyStatsVM
                {
                    TotalSales = salesThisMonth.Sum(s => s.TotalAmount),
                    SalesCount = salesThisMonth.Count,

                    TotalPurchases = purchasesThisMonth.Sum(p => p.TotalAmount),

                    SalesDebts = salesDebts,
                    PurchaseDebts = purchaseDebts,

                    LastMonthSales = salesLastMonth.Sum(s => s.TotalAmount),

                    Days = dailySales.Select(d => $"يوم {d.Day}").ToList(),
                    DailySales = dailySales.Select(d => d.Total).ToList(),

                    TopProducts = topProducts
                };

            ViewBag.Month = startDate.ToString("MMMM yyyy", new System.Globalization.CultureInfo("ar-YE"));

            return View(vm);
        }


    }
}
