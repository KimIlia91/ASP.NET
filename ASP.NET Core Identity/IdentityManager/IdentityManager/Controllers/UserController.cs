using IdentityManager.Data;
using IdentityManager.Models;
using IdentityManager.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityManager.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string roleFilter, string nameFilet)
        {
            var users = await _context.ApplicationUsers.AsNoTracking().ToListAsync();
            var userRoles = await _context.UserRoles.AsNoTracking().ToListAsync();
            var roles = await _context.Roles.AsNoTracking().ToListAsync();
            foreach (var user in users)
            {
                var role = userRoles.FirstOrDefault(u => u.UserId == user.Id);
                if (role is null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId)!.Name!;
                }
            }
            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.Role!.ToLower().Contains(roleFilter.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(nameFilet))
            {
                users = users.Where(u => u.Name.ToLower().Contains(nameFilet.ToLower())).ToList();
            }
            return View(users);
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                return NotFound();
            }
            var roles = await _context.Roles.AsNoTracking().ToListAsync();
            var userRoles = await _context.UserRoles.AsNoTracking().ToListAsync();
            var roleOfUser = userRoles.FirstOrDefault(u => u.UserId == user.Id);
            if (roleOfUser is not null)
            {
                user.RoleId = roles.FirstOrDefault(u => u.Id == roleOfUser.RoleId)!.Id;
            }
            user.RoleList = _context.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var userFromDb = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (user is null)
                {
                    return NotFound();
                }
                var userRoles = await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == userFromDb.Id);
                if (userRoles is not null)
                {
                    var previousRoleName = await _context.Roles.Where(u => u.Id == userRoles.RoleId)
                                                               .Select(r => r.Name)
                                                               .FirstOrDefaultAsync();
                    //Remove old role
                    await _userManager.RemoveFromRoleAsync(userFromDb!, previousRoleName!);
                }
                //Add new role
                var newRole = _context.Roles.FirstOrDefault(u => u.Id == user.RoleId)!.Name;
                await _userManager.AddToRoleAsync(userFromDb!, newRole!);
                userFromDb!.Name = user.Name;
                await _context.SaveChangesAsync();
                TempData[SD.Success] = "User has been edited successfully.";
                return RedirectToAction(nameof(Index), "User");
            }
            user.RoleList = _context.Roles.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUnlock(string userId)
        {
            var userFromDb = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if(userFromDb is null)
            {
                return NotFound();
            }
            if (userFromDb.LockoutEnd is not null && userFromDb.LockoutEnd > DateTime.Now)
            {
                //user is locked
                //click to unlock user
                userFromDb.LockoutEnd = DateTime.Now;
                TempData[SD.Success] = "User unlocked successfully.";
            }
            else
            {
                //user is not locked 
                //click to locked user
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
                TempData[SD.Success] = "User locked successfully.";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            var userFromDb = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (userFromDb is null)
            {
                TempData[SD.Error] = "User not found.";
                return RedirectToAction(nameof(Index), "Roles");
            }
            _context.ApplicationUsers.Remove(userFromDb);
            await _context.SaveChangesAsync();
            TempData[SD.Success] = "User remove successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
            {
                return NotFound();
            }
            //Получем вск права которые есть у пользователя 
            var existingsUserClaims = await _userManager.GetClaimsAsync(user);
            //Создаем модель представления и добаляем ID пользователя
            var model = new UserClaimsViewModel()
            {
                UserId = userId,
            };
            //Создаем список прав для пользователя
            foreach (var claim in ClaimStore.claimList)
            {
                var userClaim = new UserCalim
                {
                    ClaimType = claim.Type,
                };
                //Ставим флаг на права кторые уже есть у пользователя
                if (existingsUserClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
            {
                return NotFound();
            }
            //Получаем все права которые есть у пользователя
            var claimsOfUser = await _userManager.GetClaimsAsync(user);
            //Удаляем все существующие права у пользователя
            var result = await _userManager.RemoveClaimsAsync(user, claimsOfUser);
            // Проверяем успешно ли прошло удаление
            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while removing claims";
                return View(model);
            }
            //Получаем выбранные новые вабранные права для пользователя
            var claimsIsSelected = model.Claims.Where(c => c.IsSelected)
                                               .Select(c => new Claim(c.ClaimType, c.IsSelected.ToString()));
            //Добавляем в таблицу новые выбранные права для пользователя
            result = await _userManager.AddClaimsAsync(user, claimsIsSelected);
            //Прверка прошло ли удачно добавление новых прав
            if (!result.Succeeded)
            {
                TempData[SD.Error] = "Error while adding claims";
                return View(model);
            }
            TempData[SD.Success] = "Claims update successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
