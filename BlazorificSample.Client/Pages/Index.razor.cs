namespace BlazorificSample.Client.Pages
{
    using BlazorificSample.Shared;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;
    using Unosquare.Blazorific.Tubular;

    public partial class Index
    {
        private Product[] Items;

        protected override async Task OnInitializedAsync()
        {
            Items = await Http.GetJsonAsync<Product[]>("Products");
            await base.OnInitializedAsync();
        }

        protected async void OnBodyRowDoubleClick(GridBodyRowEventArgs e)
        {
            Console.WriteLine($"Forecast on {(e.DataItem as Product).ProductId} Double clicked");
            (e.DataItem as Product).Name = string.Empty;

            Items = null;
            var request = e.Sender.CreateTubularGridDataRequest(typeof(Product));
            var response = await Http.PostJsonAsync<TubularGridDataResponse>("Products/grid", request);
            Console.WriteLine($"response payload count is {response.Payload.Count}");
            e.Sender.ApplyGridDataResponse(typeof(Product), response);
        }
    }
}
