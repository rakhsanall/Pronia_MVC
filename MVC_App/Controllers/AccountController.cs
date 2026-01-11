using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MVC_App.Abstractions;
using MVC_App.Contexts;
using MVC_App.Models;
using MVC_App.ViewModels.Account;
using System.Threading.Tasks;

namespace MVC_App.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager,SignInManager<AppUser> _signinManager,RoleManager<IdentityRole> _roleManager,IEmailService _emailService) : Controller
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

            await SendConfirmationMailAsync(newUser);
            return RedirectToAction("Login");

        }

        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "User"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Admin"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Moderator"
            });
            return Ok("Roles Created");
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


                return View(model);
            }
            var loginResult=await _userManager.CheckPasswordAsync(user,model.Password);
            if (!loginResult)
            {
                ModelState.AddModelError("", "Yalnis email ve ya password");
             
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please confirm your email");
                await SendConfirmationMailAsync(user);
                return View(model);
            }

            await _signinManager.SignInAsync(user,model.IsRemember);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        private async Task SendConfirmationMailAsync(AppUser user)
        {
           string token= await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string url = Url.Action("ConfirmEmail", "Account", new { token = token, userId = user.Id }, Request.Scheme);
            await _emailService.SendEmailAsync(user.Email, "Confirm email", url);
        }
        public async Task<IActionResult> ConfirmEmail(string token,string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
            {
                return NotFound();
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return RedirectToAction("Index","Home");
        }

    }
}
