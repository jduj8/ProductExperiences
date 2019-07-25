using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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

                string uniqueFileName = null;
                if (experienceCreateVM.Photo != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/products");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + experienceCreateVM.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    experienceCreateVM.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    
                }

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
        public ViewResult InitialCreate()
        {
            return View();
        }

        

    }
}
