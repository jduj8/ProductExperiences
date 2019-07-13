using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Models
{
    public class Experience
    {
        public int ExperienceID { get; set; }

        public int ProductID { get; set; }

        public int Evaluation { get; set; }

        public Recommendation Recommendation { get; set; }

        public string Email { get; set; }

        public virtual Product Product { get; set; }

    }
}
