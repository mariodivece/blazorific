namespace Unosquare.Blazorific;

/// <summary>
/// Defines a search box for the <see cref="CandyGrid"/>.
/// </summary>
/// <seealso cref="CandyGridChildComponent" />
public partial class CandyGridSearchBox : IDisposable
{
    private readonly Timer DebounceTimer;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="CandyGridSearchBox"/> class.
    /// </summary>
    public CandyGridSearchBox()
    {
        DebounceTimer = new Timer((s) =>
        {
            Parent?.ChangeSearchText(SearchText);
        }, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// Gets or sets the placeholder.
    /// </summary>
    /// <value>
    /// The placeholder.
    /// </value>
    [Parameter]
    public string Placeholder { get; set; } = "search . . .";

    /// <summary>
    /// Gets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    public bool IsVisible => Parent?.Columns.Any(c => c.IsSearchable) ?? false;

    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    /// <value>
    /// The search text.
    /// </value>
    public string? SearchText { get; set; }

    /// <summary>
    /// Raises the <see cref="E:SearchInput" /> event.
    /// </summary>
    /// <param name="e">The <see cref="ChangeEventArgs"/> instance containing the event data.</param>
    private void OnSearchInput(ChangeEventArgs e)
    {
        SearchText = (e.Value as string ?? string.Empty).Trim();
        DebounceTimer.Change(250, Timeout.Infinite);
    }

    /// <summary>
    /// Disposes unmanaged and optionally managed resources.
    /// </summary>
    /// <param name="alsoManaged">Dispose managed resources.</param>
    protected virtual void Dispose(bool alsoManaged)
    {
        if (isDisposed)
            return;

        isDisposed = true;

        if (alsoManaged)
        {
            DebounceTimer.Dispose();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(alsoManaged: true);
        GC.SuppressFinalize(this);
    }
}
