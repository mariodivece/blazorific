namespace Unosquare.Blazorific.Common;

/// <summary>
/// Represents a tabular data response.
/// </summary>
public record GridDataResponse
{
    /// <summary>
    /// Gets or sets the data items.
    /// </summary>
    /// <value>
    /// The data items.
    /// </value>
    public ICollection<object>? DataItems { get; set; }

    /// <summary>
    /// Gets or sets the aggregate data item.
    /// </summary>
    /// <value>
    /// The aggregate data item.
    /// </value>
    public object? AggregateDataItem { get; set; }

    /// <summary>
    /// Gets or sets the type of the data item.
    /// </summary>
    /// <value>
    /// The type of the data item.
    /// </value>
    public Type? DataItemType { get; set; }

    /// <summary>
    /// Set how many records are in the entire set.
    /// </summary>
    public int TotalRecordCount { get; set; }

    /// <summary>
    /// Set how many records are in the filtered set.
    /// </summary>
    public int FilteredRecordCount { get; set; }

    /// <summary>
    /// Set how many pages are available.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Set which page is sent.
    /// </summary>
    public int CurrentPage { get; set; }
}
