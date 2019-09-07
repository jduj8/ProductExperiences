using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels.AdministrationViewModels
{
    public class EditCategoryViewModel
    {
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(31, ErrorMessage = "Max length of category is 31")]
        public string CategoryName { get; set; }

        public string ExistingPhotoPath { get; set; }

        public IFormFile Photo { get; set; }
    }
}
