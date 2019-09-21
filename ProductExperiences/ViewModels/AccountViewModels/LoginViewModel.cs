using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Unesite korisničko ime")]
        [Remote("UserExistsAsync", "Account", ErrorMessage = "Korisničko ime ne postoji u sustavu")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Unesite lozinku")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
