namespace BlazorificSample.Server.Controllers
{
    using BlazorificSample.Shared;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using Unosquare.Tubular;

    [ApiController]
    [Route("[controller]")]
    public class ProductsController
    {
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return DummyDb.Products.ToArray();
        }

        [HttpPost]
        [Route("grid")]
        public GridDataResponse GetGridData(GridDataRequest request)
        {
            return request.CreateGridDataResponse(DummyDb.Products);
        }
    }
}
