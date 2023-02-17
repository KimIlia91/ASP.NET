using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManager.Controllers
{
    [Authorize]
    public class AccessCheckerController : Controller
    {
        //Доступ для всех, даже для тех кто не авторизован
        [AllowAnonymous]
        public IActionResult AllAccess()
        {
            return View();
        }

        //Доступ авторизованным пользователям 
        public IActionResult AuthorizedAccess()
        {
            return View();
        }

        //Доступ пользователям с ролью пользователь
        [Authorize(Roles = "User")]
        public IActionResult UserAccess()
        {
            return View();
        }

        //Доступ пользователям с ролью пользователь или админа
        [Authorize(Roles = "User,Admin")]
        public IActionResult UserOrAdminAccess()
        {
            return View();
        }

        //Доступ пользователям с ролью пользователь и админа
        [Authorize(Policy = "UserAndAdmin")]
        public IActionResult UserAndAdminAccess()
        {
            return View();
        }

        //Доступ пользователям с ролью админ
        [Authorize(Policy = "Admin")]
        public IActionResult AdminAccess()
        {
            return View();
        }

        //Доступ пользователям с ролью админ кторый имеет права только на создание на создание
        [Authorize(Policy = "AdminCreateAccess")]
        public IActionResult AdminCreateAccess()
        {
            return View();
        }

        //Доступ пользователям с ролью админ кторый имеет права на создание, изменения и удаления
        [Authorize(Policy = "AdminCreateEditDeleteAccess")]
        public IActionResult AdminCreateEditDeleteAccess()
        {
            return View();
        }

        //Доступ пользователям с ролью супер админ или админа кторый имеет все права
        [Authorize(Policy = "AdminCreateEditDeleteOrSuperAdminAccess")]
        public IActionResult AdminCreateEditDeleteOrSuperAdminAccess()
        {
            return View();
        }

        [Authorize(Policy = "AdminWhenMore1000Days")]
        public IActionResult DateTime()
        {
            return View();
        }

        [Authorize(Policy = "FirstNameAuth")]
        public IActionResult FirstNameAuth()
        {
            return View();
        }
    }
}
