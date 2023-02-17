using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //GET
        public IActionResult Index()
        {
            IEnumerable<CoverType> categoryList = _unitOfWork.CoverType.GetAll();
            return View(categoryList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken] // Protect from attack 
        public IActionResult Create(CoverType coverType)
        {
            //Server side Validation
            if (ModelState.IsValid)
            {
                var coverToCheck = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Name == coverType.Name);
                if (coverToCheck != null)
                {
                    if (coverType.Name != coverToCheck.Name)
                    {
                        _unitOfWork.CoverType.Add(coverType);
                        _unitOfWork.Save();
                        TempData["success"] = "Cover type created successfully";
                        return RedirectToAction(nameof(Index), "CoverType");
                    }
                    TempData["error"] = "The type of cover with this name has already existed!";
                    return View(coverType);
                }
                _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.Save();
                TempData["success"] = "Cover type created successfully";
                return RedirectToAction(nameof(Index), "CoverType");
            }
            return View(coverType);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id is null || id is 0)
                return NotFound();
            var coverToEdit = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (coverToEdit is null)
                return NotFound();
            return View(coverToEdit);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken] // Protect from attack 
        public IActionResult Edit(CoverType coverToEdit)
        {
            //Server side Validation
            if (ModelState.IsValid)
            {
                var coverToCheck = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Name.ToLower() == coverToEdit.Name.ToLower());
                if(coverToCheck is null)
                {
                    _unitOfWork.CoverType.Update(coverToEdit);
                    _unitOfWork.Save();
                    TempData["success"] = "Cover type updated successfully";
                    return RedirectToAction(nameof(Index), "CoverType");
                }
                TempData["error"] = "The type of cover with this name has already existed!";
            }
            return View(coverToEdit);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id is null || id is 0)
                return NotFound();
            var coverToDelete = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (coverToDelete is null)
                return NotFound();
            return View(coverToDelete);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Protect from attack 
        public IActionResult DeletePost(int? id)
        {
            if (id is null || id is 0)
                return NotFound();
            var coverToDelete = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (coverToDelete is null)
                return NotFound();
            _unitOfWork.CoverType.Remove(coverToDelete);
            _unitOfWork.Save();
            TempData["success"] = "Cover type deleted successfully";
            return RedirectToAction(nameof(Index), "CoverType");
        }
    }
}
