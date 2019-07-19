using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels
{
    public class ExperienceListViewModel
    {
        public IEnumerable<Experience> Experiences { get; set; }

        public string Category { get; set; }
    }
}
