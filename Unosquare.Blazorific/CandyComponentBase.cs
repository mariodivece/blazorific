namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System.Net.Http;
    using System.Threading.Tasks;

    public abstract class CandyComponentBase : ComponentBase
    {
        [Inject]
        protected NavigationManager Navigation { get; set; }

        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected IJSRuntime Js { get; set; }

        public string AssetUrl(string assetFile) =>
            Navigation.ToAbsoluteUri($"/_content/{nameof(Unosquare)}.{nameof(Blazorific)}/{assetFile}").ToString();
    }
}
