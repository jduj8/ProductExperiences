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
using PagedList;
using PagedList.Mvc;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Models;
using ProductExperiences.Helpers;
using ProductExperiences.ViewModels;
using ReflectionIT.Mvc.Paging;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductExperiences.Controllers
{
    public class ExperienceController : Controller
    {

        private readonly IProductRepository _productRepository;
        private readonly IExperienceRepository _experienceRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        //test

        public ExperienceController(IProductRepository productRepository, IExperienceRepository experienceRepository, IHostingEnvironment hostingEnvironment)
        {
            _productRepository = productRepository;
            _experienceRepository = experienceRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(string category, string searchTerm, int? pageNumber)
        {

            IOrderedQueryable<Experience> query;

            if (string.IsNullOrEmpty(searchTerm))
            {
                if (string.IsNullOrEmpty(category) || category == "Sve kategorije")
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

            ViewData["category"] = string.IsNullOrEmpty(category) ? "Sve kategorije" : category;                     
            ViewData["searchTerm"] = searchTerm;

            int pageSize = 6;
            var model = await PaginatedList<Experience>.CreateAsync(query, pageNumber ?? 1, pageSize);

            return View(model);
        }


        public ViewResult Details(int experienceID)
        {
            var experience = _experienceRepository.GetExperience(experienceID);
            return View(experience);
        }


        [HttpGet]
        [Authorize]
        public ViewResult Create()
        {
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
                    Category = experienceCreateVM.Category
                };

                var addedProduct = _productRepository.AddProduct(product);

                string uniqueFileName = SaveImageAndReturnUniqueFileName(experienceCreateVM.Photo);


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

                Debug.WriteLine(experience.Date.ToString("yyyy/mm/dd hh:mm"));

                var addedExperience = _experienceRepository.AddExperience(experience);
                return RedirectToAction("details", new { experienceID = addedExperience.ExperienceID });
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public ViewResult CreateFor(int productID)
        {
            Product product = _productRepository.GetProduct(productID);

            ExperienceCreateForExistingProductViewModel experienceCreateVM = new ExperienceCreateForExistingProductViewModel
            {
                ProductName = product.ProductName,
                Category = product.Category             
            };

            TempData["ProductID"] = productID;

            return View(experienceCreateVM);
        
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateFor(ExperienceCreateForExistingProductViewModel experienceCreateVM)
        {

            if (ModelState.IsValid)
            {
                string uniqueFileName = SaveImageAndReturnUniqueFileName(experienceCreateVM.Photo);

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
        public ViewResult MyList()
        {
            IEnumerable<Experience> myExperiences = _experienceRepository.GetExperienceOfUser(User.FindFirst(ClaimTypes.Name).Value);

            return View(myExperiences);
        }

        [HttpGet]
        [Authorize]
        public ViewResult Edit(int experienceID)
        {
            Experience experience = _experienceRepository.GetExperience(experienceID);


            ExperienceEditViewModel experienceEditVM = new ExperienceEditViewModel
            {
                ExperienceID = experience.ExperienceID,
                ProductName = experience.Product.ProductName,
                Category = experience.Product.Category,
                Evaluation = experience.Evaluation,
                Describe = experience.Describe,
                Recommendation = experience.Recommendation,
                ExistingPhotoPath = experience.PhotoPath              
            };

            return View(experienceEditVM);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(ExperienceEditViewModel experienceEditVM)
        {
            if (ModelState.IsValid)
            {

                var product = _productRepository.GetProductUsingNameAndCategory(experienceEditVM.ProductName, experienceEditVM.Category);
                if (product == null)
                {
                    product = new Product
                    {
                        ProductName = experienceEditVM.ProductName,
                        Category = experienceEditVM.Category
                    };

                    _productRepository.AddProduct(product);
                }

                Debug.WriteLine(product.ProductID);

                string uniqueFileName = SaveImageAndReturnUniqueFileName(experienceEditVM.Photo);               

                var photoPath = uniqueFileName == null ? experienceEditVM.ExistingPhotoPath : uniqueFileName;

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
        public IActionResult Delete(int experienceID)
        {
            var experience = _experienceRepository.DeleteExperience(experienceID);

            return RedirectToAction("MyList", "Experience");
        }

        
        [NonAction]
        public string SaveImageAndReturnUniqueFileName(IFormFile photo)
        {
            string uniqueFileName = null;

            if (photo != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/products");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                photo.CopyTo(new FileStream(filePath, FileMode.Create));

            }

            return uniqueFileName;
        }

        

    }
}
