using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using MVC_App.Models;
using MVC_App.ViewModels.Product;
using NuGet.Packaging;


namespace MVC_App.Areas.Admin.Controllers
{
    [Area("Admin")]


    public class ProductController : Controller 
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductController(AppDbContext context,IWebHostEnvironment environment)
        {
            _context=context;
            _environment=environment;

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
        public async Task<IActionResult> Create(ProductCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                await SendCategoriesWithViewbagAsync();
                return View();
            }

            var foundCategory = await _context.Categories.FindAsync(model.CategoryId);
            if (foundCategory is null)
            {
                await SendCategoriesWithViewbagAsync();

                ModelState.AddModelError("CategoryId", "bele category yoxdur");
                return View();
            }

            if (!model.MainImage.ContentType.Contains("Image"))
            {
                ModelState.AddModelError("MainImage", "Sadece Image tipinde daxil etmek lazimdir.");
            }
            if (model.MainImage.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("MainImage", "Size limit 2 mbdir.");

            }
            if (!model.HoverImage.ContentType.Contains("Image")){
                ModelState.AddModelError("HoverImage", "Sadece Image tipinde daxil etmek lazimdir.");
            }
            if (model.HoverImage.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("HoverImage", "Size limit 2 mbdir.");

            }


            string uniqueMainImageName = Guid.NewGuid().ToString() + model.MainImage.FileName;
            string mainImagepath=Path.Combine(_environment.WebRootPath, "assets", "images", "website-images",uniqueMainImageName);

            using FileStream mainStream = new FileStream(mainImagepath, FileMode.Create);
            await model.MainImage.CopyToAsync(mainStream);

            string uniqueHoverImageName = Guid.NewGuid().ToString() + model.HoverImage.FileName;
            string hoverImagepath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images",uniqueHoverImageName);

            using FileStream hoverStream = new FileStream(hoverImagepath, FileMode.Create);
            await model.HoverImage.CopyToAsync(hoverStream);


        
            Product product = new()
            {
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Price = model.Price,
                Rate = model.Rate,
                HoverImagePath=uniqueHoverImageName,
                MainImagePath=uniqueMainImageName

            };

           


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
           
            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");
            string mainImagePath=Path.Combine(folderPath,product.MainImagePath);
            string hoverImagePath = Path.Combine(folderPath, product.HoverImagePath);

            if(System.IO.File.Exists(mainImagePath))
                System.IO.File.Delete(mainImagePath);

            if (System.IO.File.Exists(hoverImagePath))
                System.IO.File.Delete(hoverImagePath);

            System.IO.File.Delete(mainImagePath);
            System.IO.File.Delete(hoverImagePath);



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
            //foundProduct.PhotoUrl = product.PhotoUrl;
            foundProduct.CategoryId = product.CategoryId;
            _context.Products.Update(foundProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }




    }
}
