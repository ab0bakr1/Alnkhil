using alnakhil.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alnakhil.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InventoryController : Controller
    {
        private readonly alnakhilContext _context;

        public InventoryController(alnakhilContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var history = await _context.InventoryTransactions
                .Include(x => x.Product)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return View(history);
        }
    }
}
