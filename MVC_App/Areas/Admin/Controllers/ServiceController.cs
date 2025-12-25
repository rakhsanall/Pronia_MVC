using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using MVC_App.Models;

namespace MVC_App.Areas.Admin.Controllers
{
    [Area("Admin")]
  
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Service> services = await _context.Services.ToListAsync();
            return View(services);
           
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Service service)
        {
            if (!ModelState.IsValid) {
                return View();
            }
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if ( service is null)
            {
                return NotFound(); 
            }
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            Service foundService =await _context.Services.FindAsync(id);
            if(foundService is not { })
            {
                return NotFound();
            }
            return View(foundService);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Service service)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var foundService =await _context.Services.FindAsync(service.Id);
            if (foundService is null) { 
            return NotFound();
            }
            foundService.Title= service.Title;
            foundService.Description= service.Description;
            foundService.PhotoUrl= service.PhotoUrl;
            _context.Services.Update(foundService);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        
        }
    }
}
