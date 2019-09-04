using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels.AdministrationViewModels
{
    public class AddCategoryViewModel
    {
    
        [Required]
        [MaxLength(31, ErrorMessage = "Max length of category is 31")]
        [Remote("CategoryExists", "Administration", ErrorMessage = "Category already exists")]
        public string CategoryName { get; set; }
    }
}
