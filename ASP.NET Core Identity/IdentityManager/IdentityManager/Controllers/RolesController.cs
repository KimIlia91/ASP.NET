using IdentityManager.Data;
using IdentityManager.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityManager.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.AsNoTracking().ToListAsync();
            return View(roles);
        }

        [HttpGet]
        [Authorize(Policy = "OnlySuperAdminChacker")]
        public async Task<IActionResult> Upsert(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            else
            {
                var objFromDb = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                var identityRoleVM = new IdentityRoleViewModel()
                {
                    Id = objFromDb!.Id,
                    Name = objFromDb.Name!
                };
                return View(identityRoleVM);
            }
        }

        [HttpPost]
        [Authorize(Policy = "OnlySuperAdminChacker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(IdentityRoleViewModel identityRole)
        {
            if (ModelState.IsValid)
            {
                if (await _roleManager.RoleExistsAsync(identityRole.Name))
                {
                    //error
                    TempData[SD.Error] = "Role alredy exists.";
                    return View(identityRole);
                }
                if (string.IsNullOrEmpty(identityRole.Id))
                {
                    //create
                    await _roleManager.CreateAsync(new IdentityRole { Name = identityRole.Name });
                    TempData[SD.Success] = "Role created successfully.";
                }
                else
                {
                    //update
                    var roleFromDb = await _context.Roles.FirstOrDefaultAsync(u => u.Id == identityRole.Id);
                    if (roleFromDb == null)
                    {
                        TempData[SD.Error] = "Role not found.";
                        return View(identityRole);
                    }
                    roleFromDb.Name = identityRole.Name;
                    roleFromDb.NormalizedName = identityRole.Name.ToUpper();
                    var result = await _roleManager.UpdateAsync(roleFromDb);
                    TempData[SD.Success] = "Role updated successfully.";
                }
                return RedirectToAction(nameof(Index), "Roles");
            }
            return View(identityRole);
        }

        [HttpPost]
        [Authorize(Policy = "OnlySuperAdminChacker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var objFromDb = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (objFromDb == null)
            {
                TempData[SD.Error] = "Role not found.";
                return RedirectToAction(nameof(Index), "Roles");
            }
            var userRole = _context.UserRoles.Where(u => u.RoleId == id).Count();
            if (userRole > 0) 
            {
                TempData[SD.Error] = "Cannot delete this role, users assingned to this role.";
                return RedirectToAction(nameof(Index), "Roles");
            }
            await _roleManager.DeleteAsync(objFromDb);
            TempData[SD.Success] = "Role deleted successfully!";
            return RedirectToAction(nameof(Index), "Roles");
        }
    }
}
