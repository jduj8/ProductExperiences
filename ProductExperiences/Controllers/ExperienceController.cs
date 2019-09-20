using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Models;
using ProductExperiences.Helpers;
using ProductExperiences.ViewModels;
using ReflectionIT.Mvc.Paging;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductExperiences.Controllers
{
    [RequireHttps]
    public class ExperienceController : Controller
    {

        private readonly IProductRepository _productRepository;
        private readonly IExperienceRepository _experienceRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        //test

        public ExperienceController(IProductRepository productRepository, IExperienceRepository experienceRepository, ICategoryRepository categoryRepository, IHostingEnvironment hostingEnvironment)
        {
            _productRepository = productRepository;
            _experienceRepository = experienceRepository;
            _categoryRepository = categoryRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index(string category, string searchTerm, int? pageNumber)
        {

            IOrderedQueryable<Experience> query;

            if (string.IsNullOrEmpty(searchTerm))
            {
                if (string.IsNullOrEmpty(category) || category.Equals("Sve kategorije"))
                {
                    query = _experienceRepository.GetAllExperiences().AsQueryable().OrderByDescending(e => e.Date);
                }

                else
                {
                    query = _experienceRepository.GetExperiencesFromCategory(category).AsQueryable().OrderByDescending(e => e.Date);
                }
            }

            else
            {               
                query = _experienceRepository.GetExperiencesWithProductName(searchTerm).AsQueryable().OrderByDescending(e => e.Date);                
            }

            //ViewData["category"] = string.IsNullOrEmpty(category) ? "Sve kategorije" : category;
            ViewData["category"] = category ?? "Sve kategorije";
            ViewData["searchTerm"] = searchTerm;

            int pageSize = 6;
            var model = await PaginatedList<Experience>.CreateAsync(query, pageNumber ?? 1, pageSize);

            return View(model);
        }

        [HttpGet]
        public ViewResult Details(int experienceID)
        {
            var experience = _experienceRepository.GetExperience(experienceID);
            var average = _experienceRepository.GetExperiencesWithProduct(experience.ProductID).Average(e => e.Evaluation);
            var numbersOfRecommendations = _experienceRepository.GetExperiencesWithProduct(experience.ProductID).Where(e => e.Recommendation.ToString() == "Da").Count();
            var totalExperiences = _experienceRepository.GetExperiencesWithProduct(experience.ProductID).Count();

            ExperienceDetailsViewModel experienceDetailsVM = new ExperienceDetailsViewModel
            {
                Experience = experience,
                AverageEvaluation = average,
                NumberOfRecommendations = numbersOfRecommendations,
                TotalExperiences = totalExperiences
            };

            
            ViewBag.Average = average;
            return View(experienceDetailsVM);
        }


        [HttpGet]
        [Authorize]
        public ViewResult Create()
        {
            ViewBag.Categories = _categoryRepository.Categories.OrderBy(c => c.CategoryName);
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(ExperienceCreateViewModel experienceCreateVM)
        {
            if (ModelState.IsValid)//
            {
                var product = new Product
                {
                    ProductName = experienceCreateVM.ProductName,
                    CategoryID = int.Parse(experienceCreateVM.CategoryID)
                };

                var addedProduct = _productRepository.AddProduct(product);

                string uniqueFileName = PhotoHelper.SaveImageAndReturnUniqueFileName(experienceCreateVM.Photo, _hostingEnvironment, "images/products");

                var experience = new Experience
                {
                    ProductID = addedProduct.ProductID,
                    Evaluation = experienceCreateVM.Evaluation,
                    Describe = experienceCreateVM.Describe,
                    Recommendation = experienceCreateVM.Recommendation,
                    UserName = User.FindFirst(ClaimTypes.Name).Value,
                    PhotoPath = uniqueFileName,
                    Date = DateTime.Now
                    
                };


                var addedExperience = _experienceRepository.AddExperience(experience);
                return RedirectToAction("details", new { experienceID = addedExperience.ExperienceID });
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult CreateFor(int productID)
        {
            Product product = _productRepository.GetProduct(productID);

            var checkExist = _experienceRepository.GetAllExperiencesIncludeCategory().Where(
                e => e.ProductID == productID &&
                e.UserName == User.FindFirst(ClaimTypes.Name).Value &&
                e.Product.Category.CategoryID == product.Category.CategoryID
                );

          
            if (checkExist.Count() > 0)
            {
                return RedirectToAction("Edit", new { experienceID = checkExist.ElementAt(0).ExperienceID });
            }

            var experienceCreateVM = new ExperienceCreateForExistingProductViewModel
            {
                ProductName = product.ProductName,
                CategoryID = product.Category.CategoryID.ToString()       
            };

            ViewBag.Categories = _categoryRepository.Categories.OrderBy(c => c.CategoryName);
            TempData["ProductID"] = productID;

            return View(experienceCreateVM);
        
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateFor(ExperienceCreateForExistingProductViewModel experienceCreateVM)
        {

            if (ModelState.IsValid)
            {
                string uniqueFileName = PhotoHelper.SaveImageAndReturnUniqueFileName(experienceCreateVM.Photo, _hostingEnvironment, "images/products");

                var product = _productRepository.GetProduct(int.Parse(TempData["ProductID"].ToString()));

                var experience = new Experience
                {
                    ProductID = product.ProductID,
                    Evaluation = experienceCreateVM.Evaluation,
                    Describe = experienceCreateVM.Describe,
                    Recommendation = experienceCreateVM.Recommendation,
                    UserName = User.FindFirst(ClaimTypes.Name).Value,
                    PhotoPath = uniqueFileName,
                    Date = DateTime.Now

                };

                var addedExperience = _experienceRepository.AddExperience(experience);
                return RedirectToAction("details", new { experienceID = addedExperience.ExperienceID });
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public ViewResult MyList()
        {
            
            IEnumerable<Experience> myExperiences = _experienceRepository.GetExperienceOfUser(User.FindFirst(ClaimTypes.Name).Value).OrderByDescending(e => e.Date);

            return View(myExperiences);
        }

        [HttpGet]
        [Authorize]
        public ViewResult Edit(int experienceID)
        {
            Experience experience = _experienceRepository.GetExperienceForEdit(experienceID);


            ExperienceEditViewModel experienceEditVM = new ExperienceEditViewModel
            {
                ExperienceID = experience.ExperienceID,
                ProductName = experience.Product.ProductName,
                CategoryID = experience.Product.Category.CategoryID.ToString(),
                Evaluation = experience.Evaluation,
                Describe = experience.Describe,
                Recommendation = experience.Recommendation,
                ExistingPhotoPath = experience.PhotoPath              
            };

            ViewBag.Categories = _categoryRepository.Categories.OrderBy(c => c.CategoryName);

            return View(experienceEditVM);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(ExperienceEditViewModel experienceEditVM)
        {
            if (ModelState.IsValid)
            {

                var product = _productRepository.GetProductUsingNameAndCategory(experienceEditVM.ProductName, int.Parse(experienceEditVM.CategoryID)); /*experienceEditVM.Category.CategoryName*/
                if (product == null)
                {
                    product = new Product
                    {
                        ProductName = experienceEditVM.ProductName,
                        CategoryID = int.Parse(experienceEditVM.CategoryID)
                    };

                    _productRepository.AddProduct(product);
                }

                

                string uniqueFileName = PhotoHelper.SaveImageAndReturnUniqueFileName(experienceEditVM.Photo, _hostingEnvironment, "images/products");

                if (!string.IsNullOrEmpty(uniqueFileName) && System.IO.File.Exists("wwwroot/images/products/" + experienceEditVM.ExistingPhotoPath))
                {
                    try
                    {
                        System.IO.File.Delete("wwwroot/images/products/" + experienceEditVM.ExistingPhotoPath);
                    }
                    catch
                    {

                    }
                }


                var photoPath = uniqueFileName ?? experienceEditVM.ExistingPhotoPath;

                var experience = new Experience
                {
                    ExperienceID = experienceEditVM.ExperienceID,
                    ProductID = product.ProductID,
                    Evaluation = experienceEditVM.Evaluation,
                    Describe = experienceEditVM.Describe,
                    Recommendation = experienceEditVM.Recommendation,
                    UserName = User.FindFirst(ClaimTypes.Name).Value,
                    PhotoPath = photoPath,
                    Date = DateTime.Now

                };
                

                var updateExperience = _experienceRepository.UpdateExperience(experience);
                return RedirectToAction("details", new { experienceID = updateExperience.ExperienceID });
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Delete(int experienceID, string returnUrl)
        {
            var experience = _experienceRepository.DeleteExperience(experienceID);

            if (User.IsInRole("Admin"))
            {
               if (!experience.UserName.Equals(User.Identity.Name))
                {
                    return RedirectToAction("Index", new { category = "Sve kategorije" });
                }
            
            }

            return RedirectToAction("MyList", "Experience");
        }

    }
}
