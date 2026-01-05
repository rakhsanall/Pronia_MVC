using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MVC_App.Contexts;
using MVC_App.Models;
using MVC_App.ViewModels.Account;

namespace MVC_App.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager,SignInManager<AppUser> _signinManager) : Controller
    {

      
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var isExistUsername = await _userManager.FindByNameAsync(model.UserName);
            if (isExistUsername != null)
            {
                ModelState.AddModelError("UserName", "This UserName is already exist");
                return View(model);
            }

             isExistUsername = await _userManager.FindByEmailAsync(model.Email);
            if (isExistUsername != null)
            {
                ModelState.AddModelError("Email", "This Email is already exist");
                return View(model);
            }

            AppUser newUser = new()
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName=model.UserName


            };
           var result= await _userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }


            return Ok("Ok");

        }
        
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
           var user = await _userManager.FindByEmailAsync(model.Email);
            if(user is null)
            {
                ModelState.AddModelError("", "Yalnis email ve ya password");
               


            }
            var loginResult=await _userManager.CheckPasswordAsync(user,model.Password);
            if (!loginResult)
            {
                ModelState.AddModelError("", "Yalnis email ve ya password");
             

            }
            await _signinManager.SignInAsync(user,false);
            return Ok($"{user.FullName} Salam alekum");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
