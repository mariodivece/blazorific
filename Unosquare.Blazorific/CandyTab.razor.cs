namespace Unosquare.Blazorific;

/// <summary>
/// Defines a content tab within a <see cref="CandyTabSet"/>.
/// </summary>
/// <seealso cref="CandyComponentBase" />
public partial class CandyTab
{
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
        await base.OnAfterRenderAsync(firstRender);
        if (!firstRender)
            return;

        if (Js is not null)
            await Js.InvokeVoidAsync($"{nameof(CandyTabSet)}.bindEvents", Element, JsElement);

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
    /// Called from Javascript when the event occurs.
    /// </summary>
    [JSInvokable]
    public void JsHandleShownEvent()
    {
        TabSet?.RaiseOnTabShownEvent(this);
    }
}
