using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Ime proizvoda bi trebalo biti unutar 50 znakova")]
        public string ProductName { get; set; }

        [Required]
        public Category Category { get; set; }

    }
}
