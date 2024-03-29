﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Models;
using ProductExperiences.Helpers;
using ProductExperiences.ViewModels;
using ProductExperiences.ViewModels.AdministrationViewModels;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductExperiences.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ICategoryRepository categoryRepository, IHostingEnvironment hostingEnvironment)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult CreateRole() => View();
       

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRoleVM)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = createRoleVM.RoleName
                };

                IdentityResult identityResult = await _roleManager.CreateAsync(identityRole);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(createRoleVM);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach(var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel editRoleVM)
        {
            var role = await _roleManager.FindByIdAsync(editRoleVM.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {editRoleVM.Id} cannot be found";
                return View("NotFound");
            }

            else
            {
                role.Name = editRoleVM.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
               

                return View(editRoleVM);
            }

            
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users)
            {
                var userRoleVM = new UserRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleVM.IsSelected = true;
                }

                else
                {
                    userRoleVM.IsSelected = false;
                }

                model.Add(userRoleVM);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
             
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} not exists";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var editUserVM = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            return View(editUserVM); 
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel editUserVM)
        {
            var user = await _userManager.FindByIdAsync(editUserVM.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {editUserVM.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.Email = editUserVM.Email;
                user.UserName = editUserVM.UserName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
               
                return View(editUserVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("Not found");
            }

            else
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);


                return View("ListUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("Not found");
            }

            else
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);


                return View("ListRoles");
            }
        }

        [HttpGet]
        public IActionResult ListCategories()
        {
            var categories = _categoryRepository.Categories.OrderBy(c => c.CategoryName);
            return View(categories);
        }

        [HttpGet]
        public IActionResult AddCategory() => View();
        

        [HttpPost]
        public IActionResult AddCategory(AddCategoryViewModel addCategoryVM)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = PhotoHelper.SaveImageAndReturnUniqueFileName(addCategoryVM.Photo, _hostingEnvironment, "images/categories");
                var category = new Category
                {
                    CategoryName = addCategoryVM.CategoryName,
                    CategoryPhotoPath = uniqueFileName
                };

                _categoryRepository.AddCategory(category);
                return RedirectToAction("ListCategories");
            }
            return View(addCategoryVM);
        }

        [HttpGet]
        public IActionResult EditCategory(int categoryID)
        {
            var category = _categoryRepository.GetCategory(categoryID);
            var editCategoryVM = new EditCategoryViewModel
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                ExistingPhotoPath = category.CategoryPhotoPath
            };
            return View(editCategoryVM);
        }

        [HttpPost]
        public IActionResult EditCategory(EditCategoryViewModel editCategoryVM)
        {
            
            if (ModelState.IsValid)
            {

                string uniqueFileName = PhotoHelper.SaveImageAndReturnUniqueFileName(editCategoryVM.Photo, _hostingEnvironment, "images/categories");

                if (!string.IsNullOrEmpty(uniqueFileName) && System.IO.File.Exists("wwwroot/images/categories/" + editCategoryVM.ExistingPhotoPath))
                {
                    try
                    {
                        System.IO.File.Delete("wwwroot/images/categories/" + editCategoryVM.ExistingPhotoPath);
                    }
                    catch
                    {

                    }
                }

                var existingPhotoPath = editCategoryVM.ExistingPhotoPath;
                var photoPath = uniqueFileName ?? existingPhotoPath;

                var categoryChanges = new Category
                {
                    CategoryID = editCategoryVM.CategoryID,
                    CategoryName = editCategoryVM.CategoryName,
                    CategoryPhotoPath = photoPath
                };

                _categoryRepository.UpdateCategory(categoryChanges);
                return RedirectToAction("ListCategories");
            }

            return View("ListCategories");
        }

        public JsonResult CategoryExists(string categoryName)
        {
            var result = _categoryRepository.Categories.Where(c => c.CategoryName.ToLower() == categoryName.ToLower()).FirstOrDefault();
            return Json(result == null);
        }



    }
}
