namespace Unosquare.Blazorific.Common;

/// <summary>
/// Represents a Data Request from a Tubular Grid.
/// This model is how Tubular Grid sends data to any server.
/// </summary>
public sealed class GridDataRequest 
{
    /// <summary>
    /// Request's counter.
    /// </summary>
    public int Counter { get; private set; } = 1;

    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    /// <value>
    /// The search text.
    /// </value>
    public string SearchText { get; set; } = string.Empty;

    /// <summary>
    /// Set how many records skip, for pagination.
    /// </summary>
    public int Skip { get; private set; }

    /// <summary>
    /// Set how many records take, for pagination.
    /// </summary>
    public int Take { get; private set; }

    /// <summary>
    /// Defines the columns.
    /// </summary>
    public IReadOnlyCollection<IGridDataColumn>? Columns { get; private set; }

    /// <summary>
    /// Sent the minutes difference between UTC and local time.
    /// In Blazor we never shift the datetime offset because we want the original datetime datum coming from the server.
    /// </summary>
    public int TimezoneOffset { get; set; }  // (int)Math.Round(DateTime.UtcNow.Subtract(DateTime.Now).TotalMinutes, 0);

    /// <summary>
    /// Updates this request object from a CandyGrid.
    /// </summary>
    /// <param name="grid"></param>
    internal void UpdateFrom(CandyGrid grid)
    {
        Counter += 1;
        Columns = grid.GetGridDataRequestColumns();
        Skip = grid.PageSize <= 0 ? 0 : Math.Max(0, grid.PageNumber - 1) * grid.PageSize;
        Take = grid.PageSize < 0 ? -1 : grid.PageSize;
        SearchText = grid.SearchText ?? string.Empty;
    }
}
