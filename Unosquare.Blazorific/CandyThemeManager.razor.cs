﻿namespace Unosquare.Blazorific;

/// <summary>
/// Component to handle built-in themes.
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class CandyThemeManager
{
    private bool _isInitialized;

    /// <summary>
    /// The default theme storage key
    /// </summary>
    public const string DefaultThemeStorageKey = "Unosquare.Blazorific.Theme";

    /// <summary>
    /// Gets or sets the js.
    /// </summary>
    /// <value>
    /// The js.
    /// </value>
    [Inject]
    protected IJSRuntime? Js { get; set; }

    /// <summary>
    /// Gets or sets the content of the child.
    /// </summary>
    /// <value>
    /// The content of the child.
    /// </value>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the theme storage key.
    /// </summary>
    /// <value>
    /// The theme storage key.
    /// </value>
    [Parameter]
    public string? ThemeStorageKey { get; set; }

    /// <summary>
    /// Gets or sets the default name of the theme.
    /// </summary>
    /// <value>
    /// The default name of the theme.
    /// </value>
    [Parameter]
    public string DefaultThemeName { get; set; } = "Default";

    /// <summary>
    /// Determines if the component is running under the WebAssembly Runtime.
    /// </summary>
    public bool IsWasmRuntime => Js is not null && Js is IJSInProcessRuntime;

    /// <summary>
    /// Gets the theme names.
    /// </summary>
    /// <value>
    /// The theme names.
    /// </value>
    public ICollection<string> ThemeNames { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the name of the current theme.
    /// </summary>
    /// <value>
    /// The name of the current theme.
    /// </value>
    public string CurrentThemeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this instance is storage enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is storage enabled; otherwise, <c>false</c>.
    /// </value>
    private bool IsStorageEnabled => !string.IsNullOrWhiteSpace(ThemeStorageKey);

    /// <summary>
    /// Applies the theme asynchronous.
    /// </summary>
    /// <param name="themeName">Name of the theme.</param>
    public async Task ApplyThemeAsync(string themeName)
    {
        if (Js is null)
            return;

        await Js.ApplyThemeAsync(themeName).ConfigureAwait(false);
        CurrentThemeName = await Js.GetCurrentThemeNameAsync().ConfigureAwait(false);

        if (IsStorageEnabled)
            await Js.StorageSetItemAsync(ThemeStorageKey!, CurrentThemeName).ConfigureAwait(false);

        $"Current theme changed to '{CurrentThemeName}'"
            .Log(nameof(CandyThemeManager), nameof(OnInitializedAsync));

        StateHasChanged();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // if we are in Wasm, we don't need to call this as it is called under OnInitializedAsync
        if (!IsWasmRuntime && firstRender)
            await InitializeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        // for WASM apps this is the one that should be called
        if (IsWasmRuntime)
            await InitializeAsync().ConfigureAwait(false);
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        if (Js is null)
            return;

        ThemeNames = await Js.GetThemeNamesAsync().ConfigureAwait(false);
        CurrentThemeName = IsStorageEnabled
            ? await Js.StorageGetItemAsync(ThemeStorageKey).ConfigureAwait(false) ?? string.Empty
            : string.Empty;

        if (string.IsNullOrWhiteSpace(CurrentThemeName))
            CurrentThemeName = DefaultThemeName;

        if (IsStorageEnabled)
            await Js.StorageSetItemAsync(ThemeStorageKey, CurrentThemeName).ConfigureAwait(false);

        await Js.ApplyThemeAsync(CurrentThemeName).ConfigureAwait(false);

        $"Loaded {ThemeNames?.Count} themes. Current Theme: {CurrentThemeName}"
            .Log(nameof(CandyThemeManager), nameof(OnInitializedAsync));

        _isInitialized = true;
    }
}
