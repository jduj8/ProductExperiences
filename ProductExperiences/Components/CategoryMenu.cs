using Microsoft.AspNetCore.Mvc;
using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Components
{
    public class CategoryMenu: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var categories = Enum.GetValues(typeof(Category));

            return View(categories);


        }
    }
}
