namespace Unosquare.Blazorific.Common
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static partial class Extensions
    {
        public static async Task StorageRemoveItem(this IJSRuntime js, string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                await js.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public static async Task<string> StorageGetItem(this IJSRuntime js, string key) =>
            await js.InvokeAsync<string>("localStorage.getItem", key);

        public static async Task StorageSetItem(this IJSRuntime js, string key, string value) =>
            await js.InvokeVoidAsync("localStorage.setItem", key, value);

        public static async Task BindTooltip(this IJSRuntime js, ElementReference element)
        {
            if (string.IsNullOrWhiteSpace(element.Id))
                return;

            await js.InvokeVoidAsync($"CandyApp.bindTooltip", element);
        }

        public static async Task<ICollection<string>> GetThemeNames(this IJSRuntime js)
        {
            var result = await js.InvokeAsync<string[]>($"CandyApp.getThemeNames");
            return result;
        }

        public static async Task ApplyTheme(this IJSRuntime js, string themeName) =>
            await js.InvokeVoidAsync($"CandyApp.applyTheme", themeName);

        public static async Task<string> GetCurrentThemeName(this IJSRuntime js) =>
            await js.InvokeAsync<string>($"CandyApp.getCurrentThemeName");

        internal static async Task GridFireOnRendered(this IJSRuntime js, ElementReference rootElement, bool firstRender) =>
            await js.InvokeVoidAsync($"{nameof(CandyGrid)}.onRendered", rootElement, firstRender);

        internal static async Task GridFireOnDataLoading(this IJSRuntime js, ElementReference rootElement) =>
            await js.InvokeVoidAsync($"{nameof(CandyGrid)}.onDataLoading", rootElement);

        internal static async Task GridFireOnDataLoaded(this IJSRuntime js, ElementReference rootElement) =>
            await js.InvokeVoidAsync($"{nameof(CandyGrid)}.onDataLoaded", rootElement);

        internal static async Task GridBindFilterDropdown(this IJSRuntime js, ElementReference filterElement) =>
            await js.InvokeVoidAsync($"{nameof(CandyGrid)}.bindColumnFilterDropdown", filterElement);

        internal static async Task ModalShow(this IJSRuntime js, ElementReference element) =>
            await js.InvokeVoidAsync($"{nameof(CandyModal)}.show", element);

        internal static async Task ModalHide(this IJSRuntime js, ElementReference element) =>
            await js.InvokeVoidAsync($"{nameof(CandyModal)}.hide", element);

        internal static async Task TabShow(this IJSRuntime js, ElementReference element) =>
            await js.InvokeVoidAsync($"{nameof(CandyTabSet)}.show", element);
    }
}
