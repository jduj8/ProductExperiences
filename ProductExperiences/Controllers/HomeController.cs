using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Models;

namespace ProductExperiences.Controllers
{
    public class HomeController : Controller
    {
        private IExperienceRepository _experienceRepository;

        public HomeController(IExperienceRepository experienceRepository)
        {
            _experienceRepository = experienceRepository;
        }



        public IActionResult Index()
        {
            var experiences = _experienceRepository.GetAllExperiences();
            return View(experiences);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
