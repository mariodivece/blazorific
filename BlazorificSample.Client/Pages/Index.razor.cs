namespace BlazorificSample.Client.Pages
{
    using BlazorificSample.Shared;
    using System;
    using Unosquare.Blazorific.Common;
    using Unosquare.Blazorific.Tubular;

    public partial class Index
    {
        private TubularGridDataAdapter<Product> Adapter;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var requestUrl = Navigation.ToAbsoluteUri("/Products/grid").ToString();
            Adapter = new TubularGridDataAdapter<Product>(requestUrl);
        }

        protected void OnBodyRowDoubleClick(GridBodyRowEventArgs e)
        {
            Console.WriteLine($"Item on {(e.DataItem as Product).ProductId} Double clicked");
            (e.DataItem as Product).Name = string.Empty;
        }
    }
}
