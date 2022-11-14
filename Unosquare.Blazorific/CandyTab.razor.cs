namespace Unosquare.Blazorific;

/// <summary>
/// Defines a content tab within a <see cref="CandyTabSet"/>.
/// </summary>
/// <seealso cref="CandyComponentBase" />
public partial class CandyTab
{
    /// <summary>
    /// Gets or sets the header element.
    /// </summary>
    /// <value>
    /// The header element.
    /// </value>
    protected ElementReference HeaderElement { get; set; }

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

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(Id))
            Id = Extensions.GenerateRandomHtmlId();

        TabSet?.AddTab(this);
    }

    /// <summary>
    /// Shows this instance.
    /// </summary>
    public async Task Show()
    {
        if (Js is not null)
            await Js.TabShow(HeaderElement);
    }
}
