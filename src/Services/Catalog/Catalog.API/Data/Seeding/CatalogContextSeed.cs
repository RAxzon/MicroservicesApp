using System;
using System.Collections.Generic;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Data.Seeding
{
    public static class CatalogContextSeed
    {
        public static void SeedData(IMongoCollection<Product> productCollection)
        {
            // Check if the db contains any records
            var doesDbExist = productCollection.Find(p => true).Any();

            if (!doesDbExist)
            {
                try
                {
                    productCollection.InsertManyAsync(GetPreconfiguredProducts());
                }
                catch (Exception ex)
                {
                    throw new Exception("Error", ex);
                }
            }
        }

        private static IEnumerable<Product> GetPreconfiguredProducts()
        {
            return new List<Product>
            {
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f5",
                    Name = "Samsung Galaxy S20",
                    Summary = "Summary of Samsung Galaxy S20",
                    Description = "Description of Samsung Galaxy S20",
                    ImageFile = "product2.png",
                    Price = 750.00M,
                    Category = "SmartPhone"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f6",
                    Name = "Samsung Galaxy S21",
                    Summary = "Summary of Samsung Galaxy S21",
                    Description = "Description of Samsung Galaxy S21",
                    ImageFile = "product1.png",
                    Price = 950.00M,
                    Category = "SmartPhone"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f7",
                    Name = "PlayStation 4",
                    Summary = "Summary of PlayStation 4",
                    Description = "Description of PlayStation 4",
                    ImageFile = "product3.png",
                    Price = 3500.00M,
                    Category = "Console"
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f8",
                    Name = "PlayStation 5",
                    Summary = "Summary of PlayStation 5",
                    Description = "Description of PlayStation 5",
                    ImageFile = "product4.png",
                    Price = 5500.00M,
                    Category = "Console"
                }
            };
        }
    }
}
