using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Models;

namespace ProductExperiences.Data.Mocks
{
    public class MockProductRepository : IProductRepository
    {

        private List<Product> _products;

        public MockProductRepository()
        {
            _products = new List<Product>()
            {
                new Product
                {
                    ProductID = 1,
                    ProductName = "Stolna lampa Hakaan",
                    Category = new Category()
                },

                new Product
                {
                    ProductID = 2,
                    ProductName = "ASUS Laptop X540MA",
                    Category = new Category()
                },

                new Product
                {
                    ProductID = 3,
                    ProductName = "Makita aku bušilica 26v/5 Ah 2018 god",
                    Category = new Category()
                },

                new Product
                {
                    ProductID = 4,
                    ProductName = "Kingston SSD A400 120GB, SA400S37/120G",
                    Category = new Category()
                }
                
            };
        }

        public Product AddProduct(Product product)
        {
            _products.Add(product);
            return product;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _products;
        }

        public Product GetProduct(int productID)
        {
            var product = _products.FirstOrDefault(p => p.ProductID == productID);
            return product;
        }

        public IEnumerable<Product> GetProductsFromCategory(string category)
        {
            throw new NotImplementedException();
        }

        public Product GetProductUsingNameAndCategory(string productName, int categoryID)
        {
            throw new NotImplementedException();
        }

        public Product UpdateProduct(Product productChanges)
        {
            var product = _products.FirstOrDefault(p => p.ProductID == productChanges.ProductID);

            if (product != null)
            {
                product.ProductName = productChanges.ProductName;
                product.Category = productChanges.Category;
            }

            return product;
        }

    }
}
