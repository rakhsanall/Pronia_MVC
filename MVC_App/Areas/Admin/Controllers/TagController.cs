using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using MVC_App.Models;

namespace MVC_App.Areas.Admin.Controllers
{
    [Area("Admin")]


    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags.ToListAsync();
            return View(tags);

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag is null)
            {
                return NotFound();
            }
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]

        public async Task<IActionResult> Update(int id)
        {
            Tag foundTag = await _context.Tags.FindAsync(id);
            if (foundTag is not { })
            {
                return NotFound();
            }
            return View(foundTag);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var foundTag = await _context.Tags.FindAsync(tag.Id);
            if (foundTag is null)
            {
                return NotFound();
            }
            foundTag.Name = tag.Name;

            _context.Tags.Update(foundTag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}

