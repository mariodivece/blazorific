namespace BlazorificSample.Client
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.DependencyInjection;
    using System.Globalization;
    using System.Net.Http;

    public static class Extensions
    {
        public static IServiceCollection AddDataAccessService(this IServiceCollection services)
        {
            return services.AddScoped(typeof(DataAccessService), (serviceProvider) =>
            {
                var navigation = serviceProvider.GetService<NavigationManager>();
                var httpClient = serviceProvider.GetService<HttpClient>();
                return new DataAccessService(navigation.BaseUri, httpClient);
            });
        }

        public static void SetCultureInfo(string cultureName)
        {
            // Set the culture before adding the app component
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
