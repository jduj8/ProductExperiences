﻿using Microsoft.AspNetCore.Http;
using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels
{
    public class ExperienceCreateForExistingProductViewModel
    {
      
        public string ProductName { get; set; }

        
        public string CategoryID { get; set; }

        [Required(ErrorMessage = "Unesite ocjenu proizvoda")]
        [Range(1, 10, ErrorMessage = "Ocjena proizvoda mora biti između 1 i 10")]
        public int Evaluation { get; set; }

        public string Describe { get; set; }

        [Required(ErrorMessage = "Preporucujete li proizvod?")]
        public Recommendation Recommendation { get; set; }

        public IFormFile Photo { get; set; }
    }
}
