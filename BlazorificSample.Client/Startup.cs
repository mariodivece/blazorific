namespace BlazorificSample.Client
{
    using Microsoft.AspNetCore.Components.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using System.Globalization;
    using Unosquare.Blazorific;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ApplicationState>();
            services.AddCandyModal();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            // Set the culture befor adding the app component
            var culture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            app.AddComponent<App>("app");
        }
    }
}
