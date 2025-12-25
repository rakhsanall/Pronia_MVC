using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using MVC_App.Models;

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
    }
}
