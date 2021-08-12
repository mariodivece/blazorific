namespace Unosquare.Blazorific.Common
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static partial class Extensions
    {
        public static async ValueTask StorageRemoveItemAsync(this IJSRuntime js, string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
                await js.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public static async ValueTask<string> StorageGetItemAsync(this IJSRuntime js, string key) =>
            await js.InvokeAsync<string>("localStorage.getItem", key);

        public static async ValueTask StorageSetItemAsync(this IJSRuntime js, string key, string value) =>
            await js.InvokeVoidAsync("localStorage.setItem", key, value);

        public static async ValueTask BindTooltipAsync(this IJSRuntime js, ElementReference element)
        {
            if (string.IsNullOrWhiteSpace(element.Id))
                return;

            await js.InvokeVoidAsync($"CandyApp.bindTooltip", element);
        }

        public static async ValueTask<ICollection<string>> GetThemeNamesAsync(this IJSRuntime js)
        {
            var result = await js.InvokeAsync<string[]>($"CandyApp.getThemeNames");
            return result;
        }

        public static async ValueTask ApplyThemeAsync(this IJSRuntime js, string themeName) =>
            await js.InvokeVoidAsync($"CandyApp.applyTheme", themeName);

        public static async ValueTask FocusElementAsync(this IJSRuntime js, ElementReference element) =>
            await js.InvokeVoidAsync($"CandyApp.focusElement", element);

        public static async ValueTask<string> GetCurrentThemeNameAsync(this IJSRuntime js) =>
            await js.InvokeAsync<string>($"CandyApp.getCurrentThemeName");

        internal static async ValueTask GridFireOnRendered(this IJSRuntime js, ElementReference rootElement, bool firstRender) =>
            await js.InvokeVoidAsync($"{nameof(CandyGrid)}.onRendered", rootElement, firstRender);

        internal static async ValueTask GridFireOnDataLoading(this IJSRuntime js, ElementReference rootElement) =>
            await js.InvokeVoidAsync($"{nameof(CandyGrid)}.onDataLoading", rootElement);

        internal static async ValueTask GridFireOnDataLoaded(this IJSRuntime js, ElementReference rootElement) =>
            await js.InvokeVoidAsync($"{nameof(CandyGrid)}.onDataLoaded", rootElement);

        internal static async ValueTask GridBindFilterDropdown(this IJSRuntime js, ElementReference filterElement) =>
            await js.InvokeVoidAsync($"{nameof(CandyGrid)}.bindColumnFilterDropdown", filterElement);

        internal static async ValueTask ModalShow(this IJSRuntime js, ElementReference element) =>
            await js.InvokeVoidAsync($"{nameof(CandyModal)}.show", element);

        internal static async ValueTask ModalHide(this IJSRuntime js, ElementReference element) =>
            await js.InvokeVoidAsync($"{nameof(CandyModal)}.hide", element);

        internal static async ValueTask TabShow(this IJSRuntime js, ElementReference element) =>
            await js.InvokeVoidAsync($"{nameof(CandyTabSet)}.show", element);
    }
}
