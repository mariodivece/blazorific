namespace Unosquare.Blazorific
{
    using Microsoft.Extensions.DependencyInjection;

    public static class CandyServices
    {
        public static IServiceCollection AddCandyModal(this IServiceCollection services)
        {
            return services.AddScoped<CandyModalService>();
        }
    }
}
