using Microsoft.EntityFrameworkCore;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public Product AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;

        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products;
        }

        public Product GetProduct(int productID)
        {
            
            var product = _context.Products.Include(c => c.Category).FirstOrDefault(p => p.ProductID == productID);
            return product;
        }

        public Product UpdateProduct(Product productChanges)
        {
            var product = _context.Products.Attach(productChanges);
            product.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return productChanges;

        }

        public IEnumerable<Product> GetProductsFromCategory(string category)
        {
            return _context.Products.Where(p => p.Category.ToString() == category);
        }

        public Product GetProductUsingNameAndCategory(string productName, int categoryID)
        {
            foreach (Product product in _context.Products.Include(c => c.Category))
            {
                if (product.ProductName.ToLower() == productName.ToLower() && product.Category.CategoryID == categoryID)
                {
                    return product;
                }
            }

            return null;
        }
    }
}
