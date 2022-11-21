namespace Unosquare.Blazorific;

/// <summary>
/// Defines a content tab within a <see cref="CandyTabSet"/>.
/// </summary>
/// <seealso cref="CandyComponentBase" />
public partial class CandyTab : IDisposable
{
    private DotNetObjectReference<CandyTab>? _JsCandyTab;
    private bool isDsiposed;

    /// <summary>
    /// Gets or sets the tab set.
    /// </summary>
    /// <value>
    /// The tab set.
    /// </value>
    [CascadingParameter(Name = nameof(TabSet))]
    protected CandyTabSet? TabSet { get; set; }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the child.
    /// </summary>
    /// <value>
    /// The content of the child.
    /// </value>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the index of the tab within the parent <see cref="TabSet"/>.
    /// </summary>
    public int TabIndex => TabSet?.IndexOf(this) ?? -1;

    /// <summary>
    /// Gets a value indicating whether this instance is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
    /// </value>
    public bool IsActive => TabSet?.ActiveTab == this;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(Id))
            Id = Extensions.GenerateRandomHtmlId();

        TabSet?.AddTab(this);
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (!firstRender)
            return;

        _JsCandyTab = DotNetObjectReference.Create(this);

        if (Js is not null)
            await Js.InvokeVoidAsync($"{nameof(CandyTabSet)}.bindEvents", Element, _JsCandyTab);

        if (IsExpanded)
            JsHandleShownEvent();
    }

    /// <summary>
    /// Shows this instance.
    /// </summary>
    public async Task Show()
    {
        if (Js is not null)
            await Js.TabShow(Element);
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
            _JsCandyTab?.Dispose();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(alsoManaged: true);
        GC.SuppressFinalize(this);
    }

    [JSInvokable]
    public void JsHandleShownEvent()
    {
        TabSet?.RaiseOnTabShownEvent(this);
    }
}
