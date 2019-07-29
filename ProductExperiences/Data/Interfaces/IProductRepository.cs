using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data.Interfaces
{
    public interface IProductRepository
    {
        Product GetProduct(int productID);
        IEnumerable<Product> GetAllProducts();
        Product AddProduct(Product product);
        Product UpdateProduct(Product productChanges);
        IEnumerable<Product> GetProductsFromCategory(string category);
        Product GetProductUsingNameAndCategory(string productName, Category category);
    }
}
