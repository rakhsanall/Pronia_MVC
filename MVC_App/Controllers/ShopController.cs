using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_App.Abstractions;
using MVC_App.Contexts;
using MVC_App.Models;
using MVC_App.ViewModels.Product;
using System.Security.Claims;

namespace MVC_App.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public ShopController(AppDbContext context,IEmailService emailService)
        {
            _context=context;
            _emailService = emailService;

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

        //public async Task<IActionResult> Test()
        //{
        //    await _emailService.SendEmailAsync("reksaneallahverdiyeva4@gmail.com","mpa","email done");
        //    return Ok("Ok");
        //}

        public async Task<IActionResult> AddToBasket(int productId)
        {
            var isExistProduct = await _context.Products.AnyAsync(x => x.Id == productId);
            if (isExistProduct == false)
            {
                return NotFound();
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);

            if (!isExistUser)
            {
                return BadRequest();
            }
            var existBasketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);
            if (existBasketItem is { })
            {
                existBasketItem.Count++;
                _context.BasketItems.Update(existBasketItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                BasketItem basketItem = new()
                {
                    ProductId = productId,
                    AppUserId = userId,
                    Count = 1
                };
                await _context.BasketItems.AddAsync(basketItem);

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
