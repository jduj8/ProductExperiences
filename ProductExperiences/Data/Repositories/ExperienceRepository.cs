using Microsoft.EntityFrameworkCore;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Repositories
{
    public class ExperienceRepository : IExperienceRepository
    {
        private readonly AppDbContext _context;

        public ExperienceRepository(AppDbContext context)
        {
            _context = context;
        }

        public Experience AddExperience(Experience experience)
        {
            experience.Date = DateTime.Today.Date;
            _context.Experiences.Add(experience);
            _context.SaveChanges();
            return experience;
        }

        public Experience DeleteExperience(int experienceID)
        {
            var experience = _context.Experiences.FirstOrDefault(e => e.ExperienceID == experienceID);

            if (experience != null)
            {
                _context.Experiences.Remove(experience);
                _context.SaveChanges();
            }

            return experience;


        }

        public IEnumerable<Experience> GetAllExperiences()
        {
            return _context.Experiences.Include(p => p.Product);
        }


        public Experience GetExperience(int experienceID)
        {
            var experience = _context.Experiences.Include(p => p.Product).FirstOrDefault(e => e.ExperienceID == experienceID);
            return experience;
        }

        public IEnumerable<Experience> GetExperienceOfUser(string userName)
        {
            return _context.Experiences.Include(p => p.Product).Where(e => e.UserName == userName);
        }

        public IEnumerable<Experience> GetExperiencesFromCategory(string category)
        {
            return _context.Experiences.Include(p => p.Product).Where(e => e.Product.Category.ToString() == category);
        }

        public IEnumerable<Experience> GetExperiencesWithProduct(int productID)
        {
            IEnumerable<Experience> experiences = _context.Experiences.Where(e => e.ProductID == productID).Include(p => p.Product);
            return experiences;

        }

        public IEnumerable<Experience> GetExperiencesWithProductName(string term)
        {
            return _context.Experiences.Include(p => p.Product).Where(e => e.Product.ProductName.ToLower().Contains(term.ToLower()));
        }

        public IEnumerable<Experience> GetLastThreeExperiences()
        {
            throw new NotImplementedException();
        }


        public Experience UpdateExperience(Experience experienceChanges)
        {
            
            var experience = _context.Experiences.Attach(experienceChanges);
            experience.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            
            /*
            Debug.WriteLine(experienceChanges.ExperienceID);
            Experience experience = _context.Experiences.Include(p => p.Product).FirstOrDefault(e => e.ExperienceID == experienceChanges.ExperienceID);
            Debug.WriteLine(experience.ExperienceID);
            experience.ProductID = experienceChanges.ProductID;
            experience.Evaluation = experienceChanges.Evaluation;
            experience.Describe = experienceChanges.Describe;
            experience.PhotoPath = experienceChanges.PhotoPath;
            experience.Recommendation = experienceChanges.Recommendation;

            _context.Experiences.Update(experienceChanges);
            */

            _context.SaveChanges();
            return experienceChanges;
        }
    }
}
