namespace BlazorificSample.Client
{
    using Microsoft.AspNetCore.Components.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ApplicationState>()
                .AddDataAccessService();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            Extensions.SetCultureInfo("en-US");
            app.AddComponent<App>("app");
        }
    }
}
