namespace BlazorificSample.Client
{
    using BlazorificSample.Shared;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Tubular;
    using System.Text.Json;
    using System.Collections.Generic;

    public class DataAccessService
    {
        public DataAccessService(string baseUrl, HttpClient client)
        {
            Client = client;
            BaseUrl = new Uri(baseUrl);
            ProductsDataAdapter = new TubularGridDataAdapter<Product>(
                new Uri(BaseUrl, "/Products/grid").AbsoluteUri, client);
        }

        public HttpClient Client { get; }

        public Uri BaseUrl { get; }

        public TubularGridDataAdapter<Product> ProductsDataAdapter { get; }

        public async Task<Dictionary<string,string>> GetProductFilterOptionsAsync(string fieldName)
        {
            var requestUrl = new Uri(BaseUrl, $"/Products/filteroptions/{fieldName}").AbsoluteUri;
            using var response = await Client.PostAsync(requestUrl, new StringContent(string.Empty));
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, string>>(responseJson);
        }
    }
}
