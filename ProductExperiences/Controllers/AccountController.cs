using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductExperiences.ViewModels;

using Microsoft.AspNetCore.Authorization;
using ProductExperiences.Helpers;
using ProductExperiences.Services;
using System.Text.Encodings.Web;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductExperiences.Controllers
{
    [RequireHttps]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Register() => View();
        

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
                        
                        await _emailSender.SendEmailAsync(registerVM.Email, "Product experiences stranica", "Uspješno ste registrirani u sustav, za sva dodatna pitanja slobodno nas kontaktirajte!");
                        return RedirectToAction("ListUsers", "Administration");

                    }

                    //await MessageHelper.SendEmailFromAppToUserAsync(registerVM.Email, "Product experiences stranica", "Uspješno ste se registrirali u sustav," +
                    //  " za sva dodatna pitanja slobodno nas kontaktirajte!");
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("index", "home");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var codeLink = Url.Action("ConfirmEmail", "Account", new
                        {
                            userId = user.Id,
                            code = code
                        }, 
                        protocol: HttpContext.Request.Scheme);
                    
                    await _emailSender.SendEmailAsync(registerVM.Email, "Potvrdite vašu mail adresu",
                         $"Potvrdite vaš račun jednim <a href = '{HtmlEncoder.Default.Encode(codeLink)}'> KLIKOM </a>.");

                    return RedirectToAction("CheckEmail");
                
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(registerVM);
        }

        public IActionResult CheckEmail() => View();

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }


        [HttpGet]
        public IActionResult Login() => View();
        

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null)
                {
                    if (! await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "Morate potvrditi vašu e-mail adresu");
                        return View(model);
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName, model.Password, model.RememberMe, lockoutOnFailure:true);

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
                    await _emailSender.SendEmailAsync(user.Email, "Product experiences stranica", "Uspješno ste izmjenili osobne podatke!");

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description); ;




                return View(updatePersonalDataVM);
            }

        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View();
       

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

            await _emailSender.SendEmailAsync(user.Email, "Product experiences stranica", "Uspješno ste izmjenili lozinku!");

            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
       

        
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
 