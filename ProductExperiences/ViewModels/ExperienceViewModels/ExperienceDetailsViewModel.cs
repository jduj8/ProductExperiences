using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductExperiences.Data.Models;

namespace ProductExperiences.ViewModels
{
    public class ExperienceDetailsViewModel
    {
        public Experience Experience { get; set; }

        public double AverageEvaluation { get; set; }

        public int NumberOfRecommendations { get; set; }

        public int TotalExperiences { get; set; }

    }
}
