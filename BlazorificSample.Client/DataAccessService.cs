namespace BlazorificSample.Client
{
    using BlazorificSample.Shared;
    using System;
    using Unosquare.Blazorific.Tubular;

    public class DataAccessService
    {
        public DataAccessService(string baseUrl)
        {
            ProductsDataAdapter = new TubularGridDataAdapter<Product>(
                new Uri(new Uri(baseUrl), "/Products/grid").AbsoluteUri);
        }

        public TubularGridDataAdapter<Product> ProductsDataAdapter { get; }
    }
}
