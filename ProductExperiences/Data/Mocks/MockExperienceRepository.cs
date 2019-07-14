using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Mocks
{
    public class MockExperienceRepository : IExperienceRepository
    {

        private List<Experience> _experiences;

        public MockExperienceRepository()
        {
            _experiences = new List<Experience>()
            {
                new Experience
                {
                    ExperienceID = 1,
                    ProductID = 1,
                    Describe = "Zadovoljan sam navedenim proizvodom. Zasad nisam zamijetio neku manu na proizvodu",
                    Evaluation = 8,
                    Recommendation = Recommendation.Da,
                    Email = "jdujic87@gmail.com"
                },

                new Experience
                {
                    ExperienceID = 2,
                    Describe = "Odlična bušilica, preporučio bih je svakome",
                    ProductID = 3,
                    Evaluation = 10,
                    Recommendation = Recommendation.Da,
                    Email = "iivic@gmail.com"
                },

                new Experience
                {
                    ExperienceID = 3,
                    Describe = "Sve u svemu zadovoljna sam s proizvodom. Mana laptopa je to što ponekad zna blokirati u radu pri većem opterećenju",
                    ProductID = 2,
                    Evaluation = 7,
                    Recommendation = Recommendation.Možda,
                    Email = "aanic@gmail.com"
                }

            };
        }

        public Experience AddExperience(Experience experience)
        {
            _experiences.Add(experience);
            return experience;
        }

        public Experience DeleteExperience(int experienceID)
        {
            Experience experience = _experiences.FirstOrDefault(e => e.ExperienceID == experienceID);

            if (experience != null)
            {
                _experiences.Remove(experience);
            }

            return experience;
        }

        public Experience GetExperience(int experienceID)
        {
            Experience experience = _experiences.FirstOrDefault(e => e.ExperienceID == experienceID);

            return experience;
        }

        public IEnumerable<Experience> GetExperiencesWithProduct(int productID)
        {
            IEnumerable<Experience> experiences = _experiences.Where(e => e.ProductID == productID);

            return experiences;
        }

        public Experience UpdateExperience(Experience experienceChanges)
        {
            var experience = _experiences.FirstOrDefault(e => e.ExperienceID == experienceChanges.ExperienceID);

            if (experience != null)
            {
                experience.ProductID = experienceChanges.ProductID;
                experience.Evaluation = experienceChanges.Evaluation;
                experience.Recommendation = experienceChanges.Recommendation;
                experience.Describe = experienceChanges.Describe;
                   
            }

            return experience;
        }
    }
}
