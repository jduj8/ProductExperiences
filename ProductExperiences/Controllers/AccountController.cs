using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductExperiences.ViewModels;

using Microsoft.AspNetCore.Authorization;
using ProductExperiences.Helpers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductExperiences.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {

            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = registerVM.UserName,
                    Email = registerVM.Email
                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);

                
                if (result.Succeeded)
                {
                    await MessageHelper.SendEmailFromAppToUserAsync(registerVM.Email, "Product experiences stranica", "Uspješno ste se registrirali u sustav," +
                        " za sva dodatna pitanja slobodno nas kontaktirajte!");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(registerVM);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {


                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl); //defense from attackers with their url
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }


                ModelState.AddModelError(string.Empty, "Invalid login attemp");

            }
            return View(model);
        }

    }
}
 