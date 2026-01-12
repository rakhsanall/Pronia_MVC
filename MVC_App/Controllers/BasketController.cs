using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using System.Security.Claims;

namespace MVC_App.Controllers
{
    public class BasketController(AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var basketItems = await _context.BasketItems.Include(x => x.Product).ToListAsync();
            return View(basketItems);
        }

        public async Task<IActionResult> EmptyBasket()
        {
            var basketItems = await _context.BasketItems.ToListAsync();
            foreach (var basketItem in basketItems)
            {
                _context.BasketItems.Remove(basketItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        
        public async Task<IActionResult> RemoveFromBasket(int productId)
        {
            var isexistProduct = await _context.Products.AnyAsync(x => x.Id == productId);
            if (isexistProduct == false)
            {
                return NotFound();
            }
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var isExistUser = await _context.Users.AnyAsync(x => x.Id == userId);

            if (!isExistUser)
            {
                return BadRequest();
            }
            var basketItem = await _context.BasketItems.FirstOrDefaultAsync(x => x.AppUserId == userId && x.ProductId == productId);
            if(basketItem is null)
            {
                return NotFound();
            }
            _context.BasketItems.Remove(basketItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
