using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public Category Category { get; set; }

    }
}
