using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Unesite korisničko ime")]
        [MinLength(4, ErrorMessage = "Korisničko ime mora imati najmanje 4 znaka")]
        [Remote("UserAlreadyExistsAsync", "Account", ErrorMessage = "Korisničko ime se već koristi")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Unesite mail")]
        [EmailAddress(ErrorMessage = "Nevaljan email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Unesite lozinku")]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 znakova")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju")]
        public string ConfirmPassword { get; set; }
    }
}
