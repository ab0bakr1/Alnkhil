using alnakhil.Data;
using alnakhil.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace alnakhil.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly alnakhilContext _context;

        public CategoriesController(alnakhilContext context)
        {
            _context = context;
        }

        // عرض التصنيفات
        public IActionResult Index()
        {
            return View(_context.Categories.ToList());
        }

        // صفحة الإضافة
        public IActionResult Create()
        {
            return View();
        }

        // حفظ التصنيف
        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
