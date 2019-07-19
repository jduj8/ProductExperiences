﻿using Microsoft.EntityFrameworkCore;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
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

        public IEnumerable<Experience> GetExperiencesFromCategory(string category)
        {
            return _context.Experiences.Include(p => p.Product).Where(e => e.Product.Category.ToString() == category);
        }

        public IEnumerable<Experience> GetExperiencesWithProduct(int productID)
        {
            IEnumerable<Experience> experiences = _context.Experiences.Where(e => e.ProductID == productID).Include(p => p.Product);
            return experiences;

        }

        public IEnumerable<Experience> GetLastThreeExperiences()
        {
            throw new NotImplementedException();
        }

        public Experience UpdateExperience(Experience experienceChanges)
        {
            var experience = _context.Experiences.Attach(experienceChanges);
            experience.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return experienceChanges;
        }
    }
}
