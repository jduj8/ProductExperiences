using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductExperiences.Data.Models;
using ProductExperiences.Services;
using ProductExperiences.ViewModels;
using ProductExperiences.ViewModels.AccountViewModels;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductExperiences.Controllers
{
    [RequireHttps]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
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
                var user = new ApplicationUser
                {
                    FirstName = registerVM.FirstName,
                    LastName = registerVM.LastName,
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
                   

                    await SendConfirmationEmail(user);
                    
                    return RedirectToAction("CheckEmail", user);
                
                }

                AddErrors(result);
            }

            return View(registerVM);
        }

        [HttpGet]
        public IActionResult CheckEmail(ApplicationUser user) => View(user);

        [HttpPost]
        public async Task<IActionResult> CheckEmail(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                await SendConfirmationEmail(user);
            }


            return View("CheckEmail", user);
        }

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
                FirstName = user.FirstName,
                LastName = user.LastName,
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
                user.FirstName = updatePersonalDataVM.FirstName;
                user.LastName = updatePersonalDataVM.LastName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _emailSender.SendEmailAsync(user.Email, "Product experiences stranica", "Uspješno ste izmjenili osobne podatke!");

                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);

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
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var codeLink = Url.Action("ResetPassword", "Account", new
                {
                    userId = user.Id,
                    code = code
                },
                protocol: HttpContext.Request.Scheme);


                await _emailSender.SendEmailAsync(forgotPasswordVM.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{codeLink}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(forgotPasswordVM);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();
      

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            AddErrors(result);

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [NonAction]
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);          
        }

        [NonAction]
        public async Task SendConfirmationEmail(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var codeLink = Url.Action("ConfirmEmail", "Account", new
            {
                userId = user.Id,
                code = code
            },
                protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Potvrdite vašu mail adresu",
                 $"Potvrdite vaš račun jednim <a href = '{HtmlEncoder.Default.Encode(codeLink)}'> KLIKOM </a>.");
        }
        
        public async Task<JsonResult> UserAlreadyExistsAsync(string userName)
        {
            var result = await _userManager.FindByNameAsync(userName);
            return Json(result == null);
        }

        public async Task<JsonResult> EmailAlreadyExistsAsync(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
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
 