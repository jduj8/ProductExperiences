using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductExperiences.Data.Models;

namespace ProductExperiences.Data
{
    public class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(Products.Select(p => p.Value));
            }

            if (!context.Experiences.Any())
            {
                context.AddRange
                (
                    new Experience
                    {
                        ProductID = Products["Stolna lampa Hakaan"].ProductID,
                        Product = Products["Stolna lampa Hakaan"],
                        Describe = "Zadovoljan sam navedenim proizvodom. Zasad nisam zamijetio neku manu na proizvodu",
                        Evaluation = 8,
                        Recommendation = Recommendation.Da,
                        UserName = "jdujic87@gmail.com"
                    },

                    
                    new Experience
                    {
                        Describe = "Odlična bušilica, preporučio bih je svakome",
                        ProductID = Products["Makita aku bušilica 26v/5 Ah 2018 god"].ProductID,
                        Product = Products["Makita aku bušilica 26v/5 Ah 2018 god"],
                        Evaluation = 10,
                        Recommendation = Recommendation.Da,
                        UserName = "iivic@gmail.com"
                    },
                    

                    new Experience
                    {
                        Describe = "Sve u svemu zadovoljna sam s proizvodom. Mana laptopa je to što ponekad zna blokirati u radu pri većem opterećenju",
                        ProductID = Products["ASUS Laptop X540MA"].ProductID,
                        Product = Products["ASUS Laptop X540MA"],
                        Evaluation = 7,
                        Recommendation = Recommendation.Možda,
                        UserName = "aanic@gmail.com"
                    },

                    new Experience
                    {
                        Describe = "Osjeti se značajno ubrzanje računala s ovim diskom. Također disk ima malu potrošnju energije",
                        ProductID = Products["Kingston SSD A400 120GB, SA400S37/120G"].ProductID,
                        Product = Products["Kingston SSD A400 120GB, SA400S37/120G"],
                        Evaluation = 8,
                        Recommendation = Recommendation.Da,
                        UserName = "jdujic87@gmail.com"
                    },

                    new Experience
                    {
                        Describe = "Stilski odlično odrađen kako izvana tako i iznutra. Po izgledu i završnoj obradi odaje utisak kao da je u višoj klasi. Lijepo se ponaša na putu i ako tako nastavi da se ponaša bez sumnje odlično uložen novac. Mana je malo tvrđe vješanje naprama Ford Focus-a koji sam prethodno vozio",
                        ProductID = Products["Opel Astra Sport Tourer 1.7 cdti 110ks 2014. god. COSMO"].ProductID,
                        Product = Products["Opel Astra Sport Tourer 1.7 cdti 110ks 2014. god. COSMO"],
                        Evaluation = 8,
                        Recommendation = Recommendation.Da,
                        UserName = "kkarlovic@gmail.com"
                    
                    }

                );
            }

            context.SaveChanges();
        }

        private static Dictionary<string, Product> products;
        public static Dictionary<string, Product> Products
        {
            get
            {
                if (products == null)
                {
                    var productsList = new Product[]
                    {
                        new Product
                        {
                            ProductName = "Stolna lampa Hakaan",
                            Category = Category.Namještaj
                        },

                        new Product
                        {
                            ProductName = "ASUS Laptop X540MA",
                            Category = Category.Informatika
                        },

                        new Product
                        {
                            ProductName = "Makita aku bušilica 26v/5 Ah 2018 god",
                            Category = Category.Alati
                        },

                        new Product
                        {
                            ProductName = "Kingston SSD A400 120GB, SA400S37/120G",
                            Category = Category.Informatika
                        },

                        new Product
                        {
                            ProductName = "Opel Astra Sport Tourer 1.7 cdti 110ks 2014. god. COSMO",
                            Category = Category.Vozila
                        }

                    
                    };

                    products = new Dictionary<string, Product>();

                    foreach (Product product in productsList)
                    {
                        products.Add(product.ProductName, product);
                    }
                }

                return products;
            }
        }
    }
}
