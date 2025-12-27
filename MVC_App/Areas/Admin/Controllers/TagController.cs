using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_App.Contexts;
using MVC_App.Models;
using MVC_App.ViewModels.Product;
using MVC_App.ViewModels.Tag;

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

            List<TagGetVM> tags = await _context.Tags .Select(tag=>new TagGetVM()
                {
                    Id=tag.Id,
                    Name=tag.Name

                }).ToListAsync();
            return View(tags);

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TagCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Tag tag = new()
            {
                Name = model.Name
            };
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
            TagUpdateVM model = new()
            {
                Id = id,
                Name = foundTag.Name
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Update(TagUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var foundTag = await _context.Tags.FindAsync(model.Id);
            if (foundTag is null)
            {
                return NotFound();
            }
            foundTag.Name = model.Name;

            _context.Tags.Update(foundTag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}

