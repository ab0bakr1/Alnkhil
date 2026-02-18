using alnakhil.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using alnakhil.ViewModels;

namespace alnakhil.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ================= LIST =================
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var list = new List<UserWithRoleVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                list.Add(new UserWithRoleVM
                {
                    Id = user.Id,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "—"
                });
            }

            return View(list);
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            return View(user);
        }

        // ================= EDIT =================
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string fullName, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = fullName;
            await _userManager.UpdateAsync(user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction(nameof(Index));
        }

        // ================= CREATE CASHIER =================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCashierVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                FullName = model.FullName,      // ✅ الاسم الحقيقي
                UserName = model.Email,   
                Email = model.Email,
                PhoneNumber = model.Phone,
                EmailConfirmed = true
            };


            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // تأكد أن Role موجود
                if (!await _roleManager.RoleExistsAsync("Cashier"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Cashier"));
                }

                await _userManager.AddToRoleAsync(user, "Cashier");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        // ================= DELETE =================
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                return Forbid(); // ممنوع حذف الأدمن
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

    }
}
