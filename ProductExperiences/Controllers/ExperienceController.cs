using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public ExperienceController(IProductRepository productRepository, IExperienceRepository experienceRepository)
        {
            _productRepository = productRepository;
            _experienceRepository = experienceRepository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public ViewResult List()
        {
            var experiences = _experienceRepository.GetAllExperiences();
            return View(experiences);
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
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ExperienceCreateViewModel experienceCreateVM)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductName = experienceCreateVM.ProductName,
                    Category = experienceCreateVM.Category
                };

                var addedProduct = _productRepository.AddProduct(product);

                var experience = new Experience
                {
                    ProductID = addedProduct.ProductID,
                    Evaluation = experienceCreateVM.Evaluation,
                    Describe = experienceCreateVM.Describe,
                    Recommendation = experienceCreateVM.Recommendation,
                    Email = "test@gmail.com"
                };

                var addedExperience = _experienceRepository.AddExperience(experience);
                return RedirectToAction("details", new { experienceID = addedExperience.ExperienceID });
            }

            return View();
        }


    }
}
