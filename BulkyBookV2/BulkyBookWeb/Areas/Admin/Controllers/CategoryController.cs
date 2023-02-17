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
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //GET
        public IActionResult Index()
        {
            IEnumerable<Category> categoryList = _unitOfWork.Category.GetAll();
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
        public IActionResult Create(Category category)
        {
            //Server side Validation
            if (ModelState.IsValid)
            {
                if (category.DisplayOrder > 0)
                {
                    _unitOfWork.Category.Add(category);
                    _unitOfWork.Save();
                    TempData["success"] = "Category created successfully";
                    return RedirectToAction(nameof(Index), "Category");
                }
                ModelState.AddModelError("DisplayOrder", "The Display order cannot be 0 or less than 0!");
            }
            return View(category);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id is null || id is 0)
                return NotFound();
            var categoryToEdit = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (categoryToEdit is null)
                return NotFound();
            return View(categoryToEdit);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken] // Protect from attack 
        public IActionResult Edit(Category category)
        {
            //Server side Validation
            if (ModelState.IsValid)
            {
                if (category.DisplayOrder > 0)
                {
                    _unitOfWork.Category.Update(category);
                    _unitOfWork.Save();
                    TempData["success"] = "Category updated successfully";
                    return RedirectToAction(nameof(Index), "Category");
                }
                ModelState.AddModelError("DisplayOrder", "The Display order cannot be 0 or less than 0!");
            }
            return View(category);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id is null || id is 0)
                return NotFound();
            var categoryToEdit = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (categoryToEdit is null)
                return NotFound();
            return View(categoryToEdit);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Protect from attack 
        public IActionResult DeletePost(int? id)
        {
            if (id is null || id is 0)
                return NotFound();
            var categoryToDelete = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (categoryToDelete is null)
                return NotFound();
            _unitOfWork.Category.Remove(categoryToDelete);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index), "Category");
        }
    }
}
