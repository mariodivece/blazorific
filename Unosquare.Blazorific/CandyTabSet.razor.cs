namespace Unosquare.Blazorific;

/// <summary>
/// Represents a component that contains <see cref="CandyTab"/> components.
/// </summary>
/// <seealso cref="CandyComponentBase" />
public partial class CandyTabSet
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the content of the child.
    /// </summary>
    /// <value>
    /// The content of the child.
    /// </value>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the mode.
    /// </summary>
    [Parameter]
    public CandyTabMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the on tab shown action.
    /// </summary>
    [Parameter]
    public Action<TabSetTabEventArgs>? OnTabShown { get; set; }

    /// <summary>
    /// Gets the tabs.
    /// </summary>
    /// <value>
    /// The tabs.
    /// </value>
    public IReadOnlyList<CandyTab> Tabs { get; } = new List<CandyTab>();

    /// <summary>
    /// Gets the active tab.
    /// </summary>
    public CandyTab? ActiveTab { get; protected set; }

    /// <summary>
    /// Adds the tab.
    /// </summary>
    /// <param name="tab">The tab.</param>
    public void AddTab(CandyTab tab)
    {
        (Tabs as List<CandyTab>)?.Add(tab);
        StateHasChanged();
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <param name="tab">The tab.</param>
    /// <returns></returns>
    public int IndexOf(CandyTab tab)
    {
        var tabs = Tabs as List<CandyTab>;
        return tabs?.IndexOf(tab) ?? -1;
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (string.IsNullOrWhiteSpace(Id))
            Id = Extensions.GenerateRandomHtmlId();
    }

    internal void RaiseOnTabShownEvent(CandyTab? tab)
    {
        ActiveTab = tab;
        OnTabShown?.Invoke(new(this, tab));
    }
}
