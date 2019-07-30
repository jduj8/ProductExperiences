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
using ProductExperiences.ViewModels;

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
        public IActionResult Index()
        {
            return View();
        }

        public ViewResult List(string category)
        {
            string _category = category;

            string currentCategory = string.Empty;

            IEnumerable<Experience> experiences = new List<Experience>();

            if (string.IsNullOrEmpty(category))
            {
                experiences = _experienceRepository.GetAllExperiences();
            }

            else
            {
                experiences = _experienceRepository.GetExperiencesFromCategory(category);
            }
            

            var experienceListVM = new ExperienceListViewModel()
            {
                Experiences = experiences,
                Category = category
            };
            //var experiences = _experienceRepository.GetAllExperiences();
            return View(experienceListVM);
        }

        public ViewResult Details(int experienceID)
        {
            var experience = _experienceRepository.GetExperience(experienceID);
            return View(experience);
        }

        /*
        public string AddData()
        {


            Product product4 = new Product
            {
                ProductName = "Samsung A6 2018",
                Category = Category.Informatika
            };

            var product = _productRepository.AddProduct(product4);

            Experience experience = new Experience
            {
                Describe = "Kao prednosti mobitela naveo bih moderan dizajn i kvalitetu izrade, solidan zaslon, pristojne performanse, brzo punjenje baterije. Negativne strane uređaja: nema led obavijesti ni always ON prikaz, čitač ostisaka preblizu kamere i stariji mikro USB",
                ProductID = product.ProductID,
                Evaluation = 7,
                Recommendation = Recommendation.Možda,
                Email = "ppetrovic@gmail.com",
                PhotoPath = "~/images/products/samsung_A6_2018.jpg"
            };

            _experienceRepository.AddExperience(experience);

            return "adding data to DB";
        }
        */

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
                    PhotoPath = uniqueFileName
                    
                };

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
                    PhotoPath = uniqueFileName

                };

                var addedExperience = _experienceRepository.AddExperience(experience);
                return RedirectToAction("details", new { experienceID = addedExperience.ExperienceID });
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public ViewResult InitialCreate()
        {
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
            Debug.WriteLine(experienceEditVM.ProductName);
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
                    PhotoPath = photoPath

                };

                Debug.WriteLine(experience.Recommendation);

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
