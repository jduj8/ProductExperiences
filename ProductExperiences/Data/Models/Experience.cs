using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Models
{
    public class Experience
    {
        public int ExperienceID { get; set; }

        [Required]
        public int ProductID { get; set; }


        public string Describe { get; set; }

        [Required]
        [RegularExpression("@^(?:[1-9]|0[1-9]|10)$", ErrorMessage = "Ocjena proizvoda mora biti između 1 i 10")]
        public int Evaluation { get; set; }

        [Required]
        public Recommendation Recommendation { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Neispravan mail")]
        public string Email { get; set; }

        public string PhotoPath { get; set; }

        public virtual Product Product { get; set; }

    }
}
