namespace BlazorificSample.Server.Controllers
{
    using BlazorificSample.Shared;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    [ApiController]
    [Route("[controller]")]
    public class ProductsController
    {
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return DummyDb.Products.ToArray();
        }
    }
}
