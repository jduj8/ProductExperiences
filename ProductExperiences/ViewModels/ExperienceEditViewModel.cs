using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.ViewModels
{
    public class ExperienceEditViewModel: ExperienceCreateViewModel
    {
        public int ExperienceID { get; set; }

        public string ExistingPhotoPath { get; set; }
    }
}
