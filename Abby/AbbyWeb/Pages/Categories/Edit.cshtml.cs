using AbbyWeb.Data;
using AbbyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbbyWeb.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public Category Category { get; set; }

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet(int id)
        {
            Category = _context.Categories.Find(id);
        }

        public async Task<IActionResult> OnPost()
        {
            if(Category.Name == Category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("DisplayOrder", "The Display order cannot be exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _context.Categories.Update(Category);
                await _context.SaveChangesAsync();
                TempData["success"] = "Category update successfully!";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
