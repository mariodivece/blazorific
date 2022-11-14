namespace Unosquare.Blazorific;

/// <summary>
/// Base class for most components in the library.
/// </summary>
public abstract class CandyComponentBase : ComponentBase
{
    /// <summary>
    /// Gets the injected <see cref="NavigationManager"/>.
    /// </summary>
    [Inject]
    protected NavigationManager? Navigation { get; set; }

    /// <summary>
    /// Gets the injeted <see cref="HttpClient"/>.
    /// </summary>
    [Inject]
    protected HttpClient? Http { get; set; }

    /// <summary>
    /// Gets the injected <see cref="IJSRuntime"/>.
    /// </summary>
    [Inject]
    protected IJSRuntime? Js { get; set; }

    /// <summary>
    /// Receives the native <see cref="CandyThemeManager"/> as a cascading parameter.
    /// </summary>
    [CascadingParameter(Name = nameof(ThemeManager))]
    public CandyThemeManager? ThemeManager { get; private set; }

    /// <summary>
    /// Gets the absolute URL of the given Blazorific asset file.
    /// </summary>
    /// <param name="assetFile">The asset file name.</param>
    /// <returns></returns>
    public string? AssetUrl(string assetFile) =>
        Navigation?.ToAbsoluteUri($"/_content/{nameof(Unosquare)}.{nameof(Blazorific)}/{assetFile}").ToString();
}
