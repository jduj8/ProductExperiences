using Microsoft.AspNetCore.Mvc;
using ProductExperiences.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Components
{
    public class UserExperiences: ViewComponent
    {
        private readonly IExperienceRepository _experienceRepository;

        public UserExperiences(IExperienceRepository experienceRepository)
        {
            _experienceRepository = experienceRepository;
        }

        public IViewComponentResult Invoke()
        {
            if (User.Identity.IsAuthenticated)
            {
               ViewBag.Count = _experienceRepository.GetExperienceOfUser(User.Identity.Name).Count();

            }

            return View();
        }
    }
}
