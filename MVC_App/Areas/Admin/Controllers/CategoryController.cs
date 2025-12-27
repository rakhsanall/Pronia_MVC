using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using MVC_App.Models;
using MVC_App.ViewModels.Category;
using MVC_App.ViewModels.Product;
using MVC_App.ViewModels.Tag;

namespace MVC_App.Areas.Admin.Controllers
{
    [Area("Admin")]
   

    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<CategoryGetVM> categoryVMs = await _context.Categories
                .Select(category=> new CategoryGetVM(){
                    Id = category.Id,
                    Name = category.Name

            }).ToListAsync();
            return View(categoryVMs);

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Category category = new()
            {
                Name = model.Name
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            Category foundCategory = await _context.Categories.FindAsync(id);
            if (foundCategory is not { })
            {
                return NotFound();
            }

            CategoryUpdateVM categroyUpdateVM = new()
            {
                Name = foundCategory.Name
            };
            return View(categroyUpdateVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(CategoryUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var foundCategory = await _context.Categories.FindAsync(model.Id);
            if (foundCategory is null)
            {
                return NotFound();
            }
            foundCategory.Name = model.Name;
          
            _context.Categories.Update(foundCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
