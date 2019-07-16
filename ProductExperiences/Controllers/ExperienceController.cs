using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductExperiences.Data.Interfaces;

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


    }
}
