using Microsoft.AspNetCore.Mvc;
using MVC_App.Contexts;
using MVC_App.Models;

namespace MVC_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Service> services=_context.Services.ToList();
            return View(services);
        }
    }
}
