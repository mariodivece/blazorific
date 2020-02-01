namespace BlazorificSample.Client
{
    using Microsoft.AspNetCore.Blazor.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Extensions.SetCultureInfo("en-US");
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddSingleton<ApplicationState>()
                .AddDataAccessService();

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
