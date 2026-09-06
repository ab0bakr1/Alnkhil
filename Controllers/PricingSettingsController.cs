using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using alnakhil.Data;
using alnakhil.Models;


[Authorize(Roles = "Admin")]
public class PricingSettingsController : Controller
{
    private readonly alnakhilContext _context;

    public PricingSettingsController(alnakhilContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _context.PricingSetting.FirstOrDefaultAsync()
            ?? new PricingSetting();

        return View(settings);
    }

    [HttpPost]
    public async Task<IActionResult> Update(decimal profitPercentage, decimal taxPercentage)
    {
        var settings = await _context.PricingSetting.FirstOrDefaultAsync();

        if (settings == null)
        {
            settings = new PricingSetting
            {
                ProfitPercentage = profitPercentage,
                TaxPercentage = taxPercentage
            };
            _context.PricingSetting.Add(settings);
        }
        else
        {
            settings.ProfitPercentage = profitPercentage;
            settings.TaxPercentage = taxPercentage;
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "تم حفظ إعدادات التسعير بنجاح";
        return RedirectToAction(nameof(Index));
    }
}