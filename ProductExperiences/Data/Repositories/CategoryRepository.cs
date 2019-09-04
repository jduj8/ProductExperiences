using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Repositories
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public CategoryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public IEnumerable<Category> Categories => _appDbContext.Categories;

        public Category GetCategory(int categoryID)
        {
            var category = _appDbContext.Categories.Where(c => c.CategoryID == categoryID).FirstOrDefault();

            return category;
        }
    }
}
