using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 znakova")]
        [DataType(DataType.Password)]        
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Unesite lozinku")]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 znakova")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "Lozinke se ne podudaraju")]
        public string NewPasswordConfirm { get; set; }
    }
}
