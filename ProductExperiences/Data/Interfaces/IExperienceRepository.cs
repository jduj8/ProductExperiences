﻿using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Interfaces
{
    public interface IExperienceRepository
    {
        Experience GetExperience(int experienceID);
        Experience GetExperienceForEdit(int experienceID);
        IEnumerable<Experience> GetAllExperiences();
        IEnumerable<Experience> GetAllExperiencesIncludeCategory();
        IEnumerable<Experience> GetExperiencesWithProduct(int productID);
        IEnumerable<Experience> GetExperiencesWithProductName(string term);
        IEnumerable<Experience> GetLastThreeExperiences();
        Experience AddExperience(Experience experience);
        Experience UpdateExperience(Experience experienceChanges);
        Experience DeleteExperience(int experienceID);
        IEnumerable<Experience> GetExperiencesFromCategory(string category);
        IEnumerable<Experience> GetExperienceOfUser(string userName);

    }
}
