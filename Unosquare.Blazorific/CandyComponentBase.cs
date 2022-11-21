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
    /// Gets or sets the root element of this component.
    /// </summary>
    public ElementReference Element { get; protected set; }

    /// <summary>
    /// Gets the absolute URL of the given Blazorific asset file.
    /// </summary>
    /// <param name="assetFile">The asset file name.</param>
    /// <returns></returns>
    public string? AssetUrl(string assetFile) =>
        Navigation?.ToAbsoluteUri($"/_content/{nameof(Unosquare)}.{nameof(Blazorific)}/{assetFile}").ToString();
}

/// <summary>
/// Base class for which Js Invokable methods can be called from javascript code.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="CandyComponentBase" />
/// <seealso cref="IDisposable" />
public abstract class JsCandyComponentBase<T> : CandyComponentBase, IDisposable
    where T : CandyComponentBase
{
    private bool isDsiposed;

    /// <summary>
    /// Gets the js element that can be called from javascript code.
    /// </summary>
    public virtual DotNetObjectReference<JsCandyComponentBase<T>>? JsElement { get; private set; }

    /// <inheritdoc />
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (!firstRender)
            return;

        JsElement = DotNetObjectReference.Create(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="alsoManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool alsoManaged)
    {
        if (isDsiposed)
            return;

        isDsiposed = true;

        if (alsoManaged)
        {
            JsElement?.Dispose();
            JsElement = null;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(alsoManaged: true);
        GC.SuppressFinalize(this);
    }
}