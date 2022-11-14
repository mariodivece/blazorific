namespace BlazorificSample.Server.Controllers
{
    using BlazorificSample.Shared;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using Unosquare.Tubular;
    using System.Linq.Dynamic.Core;

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
        public GridDataResponse GetGridData(GridDataRequest request) =>
            request.CreateGridDataResponse(DummyDb.Products);

        [HttpPost]
        [Route("filteroptions/{fieldName}")]
        public Dictionary<string, string> GetFilterOptions(string fieldName)
        {
            var result = new Dictionary<string, string>();
            foreach (var item in DummyDb.Products.Select($"new ({fieldName} as Key, {fieldName} as Value)").Distinct().OrderBy("Key"))
            {
                var kvp = item as dynamic;
                if (kvp is null || kvp.Key is null)
                    continue;
                
                result[$"{kvp.Key}"] = $"{kvp.Value}";
            }

            return result;
        }
    }
}
