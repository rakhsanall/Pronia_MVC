using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using MVC_App.Models;
using MVC_App.ViewModels.Product;

namespace MVC_App.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context=context;
        }
        public async Task <IActionResult> Index()
        {
            List<Product> products=await _context.Products.ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var product=await _context.Products.Select(x=>new ProductGetVM()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                CategoryName=x.Category.Name,
                Rate = x.Rate,
                AdditionalImagePaths=x.ProductImages.Select(x=>x.ImagePath).ToList(),
                HoverImagePath=x.HoverImagePath,
                MainImagePath=x.MainImagePath,
                TagNames=x.ProductTags.Select(x=>x.Tag.Name).ToList(),

            }).FirstOrDefaultAsync(x=>x.Id==id);

            if(product is null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
