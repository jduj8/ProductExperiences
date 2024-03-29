﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels
{
    public class UpdatePersonalDataViewModel
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "Unesite ime")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Unesite prezime")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Unesite mail")]
        [EmailAddress(ErrorMessage = "Nevaljan email")]
        public string Email { get; set; }

      
    }
}
