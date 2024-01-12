namespace Unosquare.Blazorific.Common;

/// <summary>
/// Extensions for Javascript interop.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Storages the remove item asynchronous.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static async ValueTask StorageRemoveItemAsync(this IJSRuntime js, string? key)
    {
        if (!string.IsNullOrWhiteSpace(key))
            await js.InvokeVoidAsync("localStorage.removeItem", key).ConfigureAwait(false);
    }

    /// <summary>
    /// Storages the get item asynchronous.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static async ValueTask<string?> StorageGetItemAsync(this IJSRuntime js, string? key) => !string.IsNullOrWhiteSpace(key)
        ? await js.InvokeAsync<string>("localStorage.getItem", key).ConfigureAwait(false)
        : default;

    /// <summary>
    /// Storages the set item asynchronous.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static async ValueTask StorageSetItemAsync(this IJSRuntime js, string? key, string value)
    {
        if (!string.IsNullOrWhiteSpace(key))
            await js.InvokeVoidAsync("localStorage.setItem", key, value).ConfigureAwait(false);
    }

    /// <summary>
    /// Binds the tooltip asynchronous.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public static async ValueTask BindTooltipAsync(this IJSRuntime js, ElementReference element)
    {
        if (string.IsNullOrWhiteSpace(element.Id))
            return;

        await js.InvokeVoidAsync($"CandyApp.bindTooltip", element).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the theme names asynchronous.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <returns></returns>
    public static async ValueTask<ICollection<string>> GetThemeNamesAsync(this IJSRuntime js)
    {
        var result = await js.InvokeAsync<string[]>($"CandyApp.getThemeNames").ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Applies the theme asynchronous.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="themeName">Name of the theme.</param>
    /// <returns></returns>
    public static async ValueTask ApplyThemeAsync(this IJSRuntime js, string themeName) =>
        await js.InvokeVoidAsync($"CandyApp.applyTheme", themeName).ConfigureAwait(false);

    /// <summary>
    /// Copies to clipboard asynchronous.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="text">The text.</param>
    /// <param name="alertMessage">The alert message.</param>
    /// <returns></returns>
    public static async ValueTask CopyToClipboardAsync(this IJSRuntime js, string text, string? alertMessage = default) =>
        await js.InvokeVoidAsync($"CandyApp.copyToClipboard", text, alertMessage!).ConfigureAwait(false);

    /// <summary>
    /// Focuses the element asynchronous.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public static async ValueTask FocusElementAsync(this IJSRuntime js, ElementReference element) =>
        await js.InvokeVoidAsync($"CandyApp.focusElement", element).ConfigureAwait(false);

    /// <summary>
    /// Gets the current theme name asynchronous.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <returns></returns>
    public static async ValueTask<string> GetCurrentThemeNameAsync(this IJSRuntime js) =>
        await js.InvokeAsync<string>($"CandyApp.getCurrentThemeName").ConfigureAwait(false);

    /// <summary>
    /// Grids the fire on rendered.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="rootElement">The root element.</param>
    /// <param name="firstRender">if set to <c>true</c> [first render].</param>
    /// <returns></returns>
    internal static async ValueTask GridFireOnRendered(this IJSRuntime js, ElementReference rootElement, bool firstRender) =>
        await js.InvokeVoidAsync($"{nameof(CandyGrid)}.onRendered", rootElement, firstRender).ConfigureAwait(false);

    /// <summary>
    /// Grids the fire on data loading.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="rootElement">The root element.</param>
    /// <returns></returns>
    internal static async ValueTask GridFireOnDataLoading(this IJSRuntime js, ElementReference rootElement) =>
        await js.InvokeVoidAsync($"{nameof(CandyGrid)}.onDataLoading", rootElement).ConfigureAwait(false);

    /// <summary>
    /// Grids the fire on data loaded.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="rootElement">The root element.</param>
    /// <returns></returns>
    internal static async ValueTask GridFireOnDataLoaded(this IJSRuntime js, ElementReference rootElement) =>
        await js.InvokeVoidAsync($"{nameof(CandyGrid)}.onDataLoaded", rootElement).ConfigureAwait(false);

    /// <summary>
    /// Grids the bind filter dropdown.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="filterElement">The filter element.</param>
    /// <returns></returns>
    internal static async ValueTask GridBindFilterDropdown(this IJSRuntime js, ElementReference filterElement) =>
        await js.InvokeVoidAsync($"{nameof(CandyGrid)}.bindColumnFilterDropdown", filterElement).ConfigureAwait(false);

    /// <summary>
    /// Modals the show.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    internal static async ValueTask ModalShow(this IJSRuntime js, ElementReference element) =>
        await js.InvokeVoidAsync($"{nameof(CandyModal)}.show", element).ConfigureAwait(false);

    /// <summary>
    /// Modals the hide.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    internal static async ValueTask ModalHide(this IJSRuntime js, ElementReference element) =>
        await js.InvokeVoidAsync($"{nameof(CandyModal)}.hide", element).ConfigureAwait(false);

    /// <summary>
    /// Tabs the show.
    /// </summary>
    /// <param name="js">The js.</param>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    internal static async ValueTask TabShow(this IJSRuntime js, ElementReference element) =>
        await js.InvokeVoidAsync($"{nameof(CandyTabSet)}.show", element).ConfigureAwait(false);
}
