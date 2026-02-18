using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using alnakhil.Models;
using alnakhil.Data; // تأكد من استدعاء الـ Namespace الخاص بالـ DbContext
using alnakhil.ViewModels; // تأكد من استدعاء الـ Namespace الخاص بالـ ViewModel
using Microsoft.EntityFrameworkCore; // لإضافة التحسينات في الاستعلامات

namespace alnakhil.Controllers;

public class HomeController : Controller
{
    private readonly alnakhilContext _context; // استبدل باسم الـ DbContext الخاص بك
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger, alnakhilContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // جلب البيانات من قاعدة البيانات (أمثلة)
        var today = DateTime.Today;
        var twoMonthsFromNow = today.AddMonths(2);

        var model = new DashboardVM
        {
            // جلب مبيعات اليوم
            TodaySales = await _context.Sales
                .Where(s => s.SaleDate.Date == today)
                .SumAsync(s => s.TotalAmount),

            // جلب عدد المنتجات
            ProductsCount = await _context.Products.CountAsync(),

            // جلب المنتجات منخفضة المخزن (أقل من 5 قطع مثلاً)
            LowStockCount = await _context.Products.CountAsync(p => p.Quantity < 50),

            // جلب آخر 5 عمليات بيع (للجزء الاحترافي الذي طلبته)
            RecentSales = await _context.Sales
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .ToListAsync(),
            
            // تاريخ بعد شهرين من الآن
            ExpiringSoonCount = await _context.Products
                .CountAsync(p => p.ExpirationDate != null && 
                p.ExpirationDate >= today && 
                p.ExpirationDate <= twoMonthsFromNow),


            ReceivableDebts = await _context.Sales
                    .Where(s => s.PaymentStatus != PaymentStatus.Paid)
                    .SumAsync(s => (decimal?)(s.TotalAmount - s.AmountPaid)) ?? 0,

            PayableDebts = await _context.Purchases
                .Where(p => p.PaymentStatus != PaymentStatus.Paid)
                .SumAsync(p => (decimal?)(p.TotalAmount - p.AmountPaid)) ?? 0
        };

        // إرسال الموديل إلى الـ View
        return View(model);
    
    }


    public IActionResult System()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
