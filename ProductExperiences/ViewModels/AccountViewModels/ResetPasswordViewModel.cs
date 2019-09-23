using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Unseite korisničko ime")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Unesite email adresu")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 znakova")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
