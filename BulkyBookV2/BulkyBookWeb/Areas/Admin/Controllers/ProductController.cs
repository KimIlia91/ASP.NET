using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        //GET
        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id is null || id is 0)
            {
                //Create product
                return View(productVM);
            }
            else
            {
                //Update  product
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
                return View(productVM);
            }
            
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken] // Protect from attack 
        public IActionResult Upsert(ProductVM productToEdit, IFormFile? formFile)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (productToEdit.CategoryList == null)
                productToEdit.CategoryList = productVM.CategoryList;
            if (productToEdit.CoverTypeList == null)
                productToEdit.CoverTypeList = productVM.CoverTypeList;
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (formFile != null)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(formFile.FileName);
                    if (productToEdit.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productToEdit.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        formFile.CopyTo(fileStreams);
                    }
                    productToEdit.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                if (productToEdit.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productToEdit.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productToEdit.Product);
                    TempData["success"] = "Product update successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index), "Product");
            }
            return View(productToEdit);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperty: "Category,CoverType");
            return Json(new {data = productList});
        }

        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDel = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
            if (productToDel is null)
                return Json(new { success = false, message = "Error while deleting." });
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, productToDel.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
            _unitOfWork.Product.Remove(productToDel);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful." });
        }

        #endregion
    }
}
