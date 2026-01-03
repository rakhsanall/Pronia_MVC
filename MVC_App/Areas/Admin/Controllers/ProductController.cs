using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using MVC_App.Contexts;
using MVC_App.Helpers;
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
            List<ProductGetVM> productVMs = await _context.Products
                .Select(product =>new ProductGetVM()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    CategoryName = product.Category.Name,
                    Price= product.Price,
                    MainImagePath = product.MainImagePath,
                    HoverImagePath = product.HoverImagePath,
                    Rate = product.Rate
                }).ToListAsync();





            //List<ProductGetVM> models = new();
            //foreach(var product in products)
            //{
            //    ProductGetVM model = new()
            //    {
            //        Id = product.Id,
            //        Name = product.Name,
            //        Description = product.Description,
            //        CategoryName = product.Category.Name,
            //        MainImagePath = product.MainImagePath,
            //Price = product.Price,
            //        HoverImagePath = product.HoverImagePath,
            //        Rate=product.Rate
            //    };
            //    models.Add(model);
            //}


            return View(productVMs);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await SendItemsWithViewbagAsync();
            return View();
        }

        private async Task SendItemsWithViewbagAsync()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            var tags = await _context.Tags.ToListAsync();
            ViewBag.Tags = tags;
        }

        //private async Task SendGenericItemsWithViewBag<T>() where T : class
        //{
        //    var items=await _context.Set<T>().ToListAsync();
        //    ViewBag.Items = items;
        //}

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                await SendItemsWithViewbagAsync();
                return View();
            }

            var foundCategory = await _context.Categories.FindAsync(model.CategoryId);
            if (foundCategory is null)
            {
                await SendItemsWithViewbagAsync();

                ModelState.AddModelError("CategoryId", "bele category yoxdur");
                return View();
            }
            foreach(var tagId in model.TagIds)
            {
                var existTag = await _context.Tags.AnyAsync(x => x.Id == tagId);
                if (!existTag)
                {
                    ModelState.AddModelError("TagIds", "bele tag yoxdur");
                    return View(model);

                }
            }


            if (!model.MainImage.CheckType())
            {
                ModelState.AddModelError("MainImage", "Sadece Image tipinde daxil etmek lazimdir.");
                return View(model);

            }
            if (!model.MainImage.CheckSize(2))
            {
                ModelState.AddModelError("MainImage", "Size limit 2 mbdir.");
                return View(model);


            }
            if (!model.HoverImage.CheckType())
            {
                ModelState.AddModelError("HoverImage", "Sadece Image tipinde daxil etmek lazimdir.");
                return View(model);

            }
            if (!model.HoverImage.CheckSize(2))
            {
                ModelState.AddModelError("HoverImage", "Size limit 2 mbdir.");
                return View(model);
            }

            foreach(var image in model.Images)
            {
                if (!image.CheckType())
                {
                    ModelState.AddModelError("Images", "Sadece Image tipinde daxil etmek lazimdir.");
                    return View(model);

                }
                if (!image.CheckSize(2))
                {
                    ModelState.AddModelError("Images", "Size limit 2 mbdir.");
                    return View(model);
                }
            }

            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

            //string uniqueMainImageName = Guid.NewGuid().ToString() + model.MainImage.FileName;
            //string mainImagepath=Path.Combine(_environment.WebRootPath, "assets", "images", "website-images",uniqueMainImageName);

            //using FileStream mainStream = new FileStream(mainImagepath, FileMode.Create);
            //await model.MainImage.CopyToAsync(mainStream);

            //string uniqueHoverImageName = Guid.NewGuid().ToString() + model.HoverImage.FileName;
            //string hoverImagepath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images",uniqueHoverImageName);

            //using FileStream hoverStream = new FileStream(hoverImagepath, FileMode.Create);
            //await model.HoverImage.CopyToAsync(hoverStream);

            string uniqueMainImageName =await model.MainImage.SaveFileAsync(folderPath);
            string uniqueHoverImageName= await model.HoverImage.SaveFileAsync(folderPath);

           

            Product product = new()
            {
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Price = model.Price,
                Rate = model.Rate,
                HoverImagePath=uniqueHoverImageName,
                MainImagePath=uniqueMainImageName,
                ProductTags = [],
                ProductImages = []

            };

            foreach (var image in model.Images)
            {
                string uniqueFilePath = await image.SaveFileAsync(folderPath);
                ProductImage productImage = new()
                {
                    ImagePath = uniqueFilePath,
                    Product = product
                };
                product.ProductImages.Add(productImage);
            }

            foreach (var tagId in model.TagIds)
            {
                ProductTag productTag = new()
                {
                    TagId = tagId,
                    Product = product
                };
                product.ProductTags.Add(productTag);
            }



            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id==id);
            if (product is null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
           
            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");
         
            string mainImagePath=Path.Combine(folderPath,product.MainImagePath);
            string hoverImagePath = Path.Combine(folderPath, product.HoverImagePath);

          
            ExtensionMethods.DeleteFile(mainImagePath);
            ExtensionMethods.DeleteFile(hoverImagePath);

            foreach(var productImage in product.ProductImages)
            {
                string imagePath = Path.Combine(folderPath, productImage.ImagePath);
                ExtensionMethods.DeleteFile(imagePath);

            }
            



            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await SendItemsWithViewbagAsync();

            var foundProduct = await _context.Products.Include(x=>x.ProductTags).Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id==id);
            if (foundProduct is not { })
            {
                return NotFound();
            }
            ProductUpdateVM model=new ProductUpdateVM()
            {
                Id = foundProduct.Id,
                Name= foundProduct.Name,
                Description= foundProduct.Description,
                Price= foundProduct.Price,
                CategoryId= foundProduct.CategoryId,
                Rate=foundProduct.Rate,
                MainImagePath=foundProduct.MainImagePath,
                HoverImagePath=foundProduct.HoverImagePath,
                TagIds=foundProduct.ProductTags.Select(x=>x.TagId).ToList(),
                AdditionalImagePaths=foundProduct.ProductImages.Select(x=>x.ImagePath).ToList(),
                AdditionalImageIds=foundProduct.ProductImages.Select(x=>x.Id).ToList(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                await SendItemsWithViewbagAsync();
                return View(model);
            }

            foreach (var tagId in model.TagIds)
            {
                var existTag = await _context.Tags.AnyAsync(x => x.Id == tagId);
                if (!existTag)
                {
                    ModelState.AddModelError("TagIds", "bele tag yoxdur");
                    return View(model);

                }
            }

            var foundProduct = await _context.Products.Include(x=>x.ProductTags).Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id== model.Id);
            if (foundProduct is null)
            {
                return NotFound();
            }

            var foundCategory = await _context.Categories.FindAsync(model.CategoryId);

            if (foundCategory is null)
            {
                await SendItemsWithViewbagAsync();

                ModelState.AddModelError("CategoryId", "bele category yoxdur");
                return View(model);
            }

           

            if (!model.MainImage?.CheckType() ?? false)
            {
                ModelState.AddModelError("MainImage", "Sadece Image tipinde daxil etmek lazimdir.");
            }
            if (!model.MainImage?.CheckSize(2) ?? false)
            {
                ModelState.AddModelError("MainImage", "Size limit 2 mbdir.");

            }
            if (!model.HoverImage?.CheckType() ?? false)
            {
                ModelState.AddModelError("HoverImage", "Sadece Image tipinde daxil etmek lazimdir.");
            }
            if (!model.HoverImage?.CheckSize(2) ?? false)
            {
                ModelState.AddModelError("HoverImage", "Size limit 2 mbdir.");

            }

            foreach (var image in model.Images?? [])
            {
                if (!image.CheckType())
                {
                    ModelState.AddModelError("Images", "Sadece Image tipinde daxil etmek lazimdir.");
                    return View(model);

                }
                if (!image.CheckSize(2))
                {
                    ModelState.AddModelError("Images", "Size limit 2 mbdir.");
                    return View(model);
                }
            }







            foundProduct.Name = model.Name;
            foundProduct.Description = model.Description;
            foundProduct.Price = model.Price;
            foundProduct.Rate=model.Rate;
            foundProduct.CategoryId = model.CategoryId;

            foundProduct.ProductTags = [];
            foreach(var tagId in model.TagIds)
            {
                ProductTag productTag = new()
                {
                    TagId = tagId,
                    ProductId = foundProduct.Id
                };
                foundProduct.ProductTags.Add(productTag);
            }



            string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

            if (model.MainImage is { })
            {
                string newMainImagePath = await model.MainImage.SaveFileAsync(folderPath);
                string existMainImagePath = Path.Combine(folderPath, foundProduct.MainImagePath);
                ExtensionMethods.DeleteFile(existMainImagePath);
                foundProduct.MainImagePath = newMainImagePath;

            }
            if (model.HoverImage is { })
            {
                string newHoverImagePath = await model.HoverImage.SaveFileAsync(folderPath);
                string existHoverImagePath = Path.Combine(folderPath, foundProduct.HoverImagePath);
                ExtensionMethods.DeleteFile(existHoverImagePath);
                foundProduct.HoverImagePath = newHoverImagePath;

            }
            var existImages=foundProduct.ProductImages.ToList();


            foreach(var image in existImages)
            {
                var isExistImageId=model.AdditionalImageIds?.Any(x=>x==image.Id)?? false;
                if (!isExistImageId)
                {
                    string deletedPath = Path.Combine(folderPath, image.ImagePath);
                    ExtensionMethods.DeleteFile(deletedPath); //remove from server
                    foundProduct.ProductImages.Remove(image); //remove from db

                }
            }
            foreach (var image in model.Images)
            {
                string uniqueFilePath = await image.SaveFileAsync(folderPath);
                ProductImage productImage = new()
                {
                    ImagePath = uniqueFilePath,
                    ProductId = foundProduct.Id
                };
                foundProduct.ProductImages.Add(productImage);
            }

            _context.Products.Update(foundProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products.Include(x => x.Category)
                
                .Select(product=> new ProductGetVM()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    CategoryName = product.Category.Name,
                    Price = product.Price,
                    MainImagePath = product.MainImagePath,
                    HoverImagePath = product.HoverImagePath,
                    Rate = product.Rate,
                    TagNames=product.ProductTags.Select(x=>x.Tag.Name).ToList(),
                    AdditionalImagePaths=product.ProductImages.Select(x=>x.ImagePath).ToList()
                }).FirstOrDefaultAsync(x => x.Id==id);
            if (product is null)
            {
                return NotFound();
            }
            return View(product);

        } 




    }
}
