using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using MVC_App.Models;

namespace MVC_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateAntiForgeryToken]

    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context=context;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products =await _context.Products.Include(x=>x.Category).ToListAsync();
            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await SendCategoriesWithViewbagAsync();
            return View();
        }

        private async Task SendCategoriesWithViewbagAsync()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                await SendCategoriesWithViewbagAsync();
                return View();
            }
            var foundCategory = await _context.Categories.FindAsync(product.CategoryId);
            if(foundCategory is null)
            {
                await SendCategoriesWithViewbagAsync();

                ModelState.AddModelError("CategoryId", "bele category yoxdur");
                return View();
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await SendCategoriesWithViewbagAsync();

            Product foundProduct = await _context.Products.FindAsync(id);
            if (foundProduct is not { })
            {
                return NotFound();
            }
            return View(foundProduct);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            if (!ModelState.IsValid)
            {
                await SendCategoriesWithViewbagAsync();

                return View();
            }
            var foundCategory = await _context.Categories.FindAsync(product.CategoryId);

            if (foundCategory is null)
            {
                await SendCategoriesWithViewbagAsync();

                ModelState.AddModelError("CategoryId", "bele category yoxdur");
                return View();
            }
            var foundProduct = await _context.Products.FindAsync(product.Id);
            if (foundProduct is null)
            {
                return NotFound();
            }
            foundProduct.Name = product.Name;
            foundProduct.Price = product.Price;
            foundProduct.Description = product.Description;
            foundProduct.PhotoUrl = product.PhotoUrl;
            foundProduct.CategoryId = product.CategoryId;
            _context.Products.Update(foundProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }




    }
}
