using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using MVC_App.Models;

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
            List<Category> categories = await _context.Categories.ToListAsync();
            return View(categories);

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
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
            return View(foundCategory);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var foundCategory = await _context.Categories.FindAsync(category.Id);
            if (foundCategory is null)
            {
                return NotFound();
            }
            foundCategory.Name = category.Name;
          
            _context.Categories.Update(foundCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}
