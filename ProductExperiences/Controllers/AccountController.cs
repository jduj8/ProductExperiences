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

                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        await MessageHelper.SendEmailFromAppToUserAsync(registerVM.Email, "Product experiences stranica", "Uspješno ste se registrirani u sustav," +
                        " za sva dodatna pitanja slobodno nas kontaktirajte!");
                        return RedirectToAction("ListUsers", "Administration");

                    }

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

                ModelState.AddModelError(string.Empty, "Neispravni korisnički podaci");

            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdatePersonalData(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return View("NotFound");
            }


            var updatePersonalDataVM = new UpdatePersonalDataViewModel
            {
                UserName = user.UserName,
                Email = user.Email
                
            };

            return View(updatePersonalDataVM);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdatePersonalData(UpdatePersonalDataViewModel updatePersonalDataVM)
        {
            var user = await _userManager.FindByNameAsync(updatePersonalDataVM.UserName);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with UserName = {updatePersonalDataVM.UserName} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = updatePersonalDataVM.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await MessageHelper.SendEmailFromAppToUserAsync(user.Email, "Product experiences stranica", "Uspješno ste izmjenili osobne podatke!");

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }



                return View(updatePersonalDataVM);
            }

        }
        
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {  

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordVM);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with UserName = {User.Identity.Name} cannot be found";
                return View("Not found");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordVM.OldPassword, changePasswordVM.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                ModelState.AddModelError("OldPassword", "Neispravna lozinka");
                return View(changePasswordVM);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            await MessageHelper.SendEmailFromAppToUserAsync(user.Email, "Product experiences stranica", "Uspješno ste izmjenili lozinku!");

            return RedirectToAction("Index", "Home");

        }

        [HttpGet] 
        public IActionResult AccessDenied()
        {
            return View();
        }

        
        public async Task<JsonResult> UserAlreadyExistsAsync(string userName)
        {
            var result = await _userManager.FindByNameAsync(userName);
            return Json(result == null);
        }

        public async Task<JsonResult> UserExistsAsync(string userName)
        {
            bool exists = true;
            var result = await _userManager.FindByNameAsync(userName);
            exists = result == null ? false : true;
            return Json(exists);
        }

       
    }
}
 