using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Interfaces
{
    public interface IExperienceRepository
    {
        Experience GetExperience(int experienceID);
        IEnumerable<Experience> GetExperiencesWithProduct(int productID);
        Experience AddExperience(Experience experience);
        Experience UpdateExperience(Experience experienceChanges);
        Experience DeleteExperience(int experienceID);
    }
}
