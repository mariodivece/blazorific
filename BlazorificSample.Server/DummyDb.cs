namespace BlazorificSample.Server
{
    using BlazorificSample.Shared;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    public class DummyDb
    {
        private static readonly ConcurrentBag<Product> m_Products = new ConcurrentBag<Product>();

        private static readonly string[] BrandNames = { "BMW", "Lincoln", "Ferrari", "Volvo", "Mercedes-Benz" };
        private static readonly string[] ProductTypes = { "Sedan", "SUV", "Compact", "Supercar", "Hypercar", "Limo" };
        private static readonly string[] ProductQualifiers = { "Deluxe", "Standard", "Professional", "Sport", "Basic", "Ultra" };

        static DummyDb()
        {
            var random = new Random();
            var id = 1;
            foreach (var brand in BrandNames)
            {
                foreach (var productType in ProductTypes)
                {
                    foreach (var qualifier in ProductQualifiers)
                    {

                        m_Products.Add(new Product
                        {
                            CurrencyCode = random.Next(1, 2) == 1 ? "USD" : "MXN",
                            DateCreated = DateTime.UtcNow.AddDays(-1 * random.Next(10, 60)).AddSeconds(-1 * random.Next(200, 3600)),
                            DateModified = DateTime.UtcNow.AddDays(-1 * random.Next(10, 60)).AddSeconds(-1 * random.Next(200, 3600)),
                            Name = $"{brand} {productType} {qualifier}",
                            Description = $"This is a car branded {brand}, it's also a {productType} and qualifies as '{qualifier}'",
                            Price = random.Next(80000, 300000),
                            ProductId = id,
                            StockCount = random.Next(100, 200),
                        }); ;

                        id++;
                    }
                }
            }

        }

        public static IQueryable<Product> Products = m_Products.AsQueryable();
    }
}
