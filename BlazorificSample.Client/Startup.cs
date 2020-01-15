namespace BlazorificSample.Client
{
    using Microsoft.AspNetCore.Components.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Unosquare.Blazorific;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ApplicationState>()
                .AddCandyModal()
                .AddDataAccessService();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            Extensions.SetCultureInfo("en-US");
            app.AddComponent<App>("app");
        }
    }
}
