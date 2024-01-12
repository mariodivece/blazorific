namespace Unosquare.Blazorific.Common;

/// <summary>
/// Defines the state of a the <see cref="CandyGrid"/>.
/// </summary>
public class GridState
{
    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    /// <value>
    /// The search text.
    /// </value>
    public string? SearchText { get; set; }

    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    /// <value>
    /// The page number.
    /// </value>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the size of the page.
    /// </summary>
    /// <value>
    /// The size of the page.
    /// </value>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the columns.
    /// </summary>
    /// <value>
    /// The columns.
    /// </value>
    public IReadOnlyCollection<GridColumnState>? Columns { get; set; }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns></returns>
    public string Serialize() => this.SerializeJson();

    /// <summary>
    /// Exctracts a copy of the state from another one.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    internal static GridState FromGrid(CandyGrid grid, GridDataRequest request)
    {
        var result = new GridState
        {
            PageNumber = grid.PageNumber,
            PageSize = grid.PageSize,
            SearchText = grid.SearchText,
        };

        if (request is not null && request.Columns is not null && request.Columns.Count > 0)
        {
            var columns = new List<GridColumnState>(request.Columns.Count);
            foreach (var col in request.Columns)
                columns.Add(GridColumnState.FromOther(col));

            result.Columns = columns;
        }

        return result;
    }

    /// <summary>
    /// Creates a default or initial grid state.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <returns></returns>
    internal static GridState CreateDefault(CandyGrid grid)
    {
        var result = new GridState
        {
            PageNumber = 1,
            PageSize = 20,
            SearchText = string.Empty,
        };

        var columns = new List<GridColumnState>(grid.Columns.Count);
        foreach (var col in grid.Columns)
        {
            columns.Add(new GridColumnState
            {
                Name = col.Field,
                FilterArgument = [],
                FilterOperator = CompareOperators.None,
                FilterText = string.Empty
            });
        }

        result.Columns = columns;
        return result;
    }

    /// <summary>
    /// Deserializes the specified json as a <see cref="GridState"/>.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns></returns>
    internal static GridState? Deserialize(string json) => json.DeserializeJson<GridState>();
}

/// <summary>
/// Represents the state of a <see cref="CandyGridColumn"/>.
/// </summary>
/// <seealso cref="IGridDataColumn" />
public class GridColumnState : IGridDataColumn
{
    /// <inheritdoc />
    public string? Name { get; set; }

    /// <inheritdoc />
    public SortDirection SortDirection { get; set; }

    /// <inheritdoc />
    public int SortOrder { get; set; }

    /// <inheritdoc />
    public bool Sortable { get; set; }

    /// <inheritdoc />
    public DataType DataType { get; set; }

    /// <inheritdoc />
    public bool Searchable { get; set; }

    /// <inheritdoc />
    public AggregationFunction Aggregate { get; set; }

    /// <inheritdoc />
    public string? FilterText { get; set; }

    /// <inheritdoc />
    public string?[] FilterArgument { get; set; }  = [];

    /// <inheritdoc />
    public CompareOperators FilterOperator { get; set; }

    /// <summary>
    /// Creates a new <see cref="GridColumnState"/> beased on an existing <see cref="IGridDataColumn"/>.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns></returns>
    internal static GridColumnState FromOther(IGridDataColumn other) => new()
    {
        Name = other.Name,
        SortDirection = other.SortDirection,
        SortOrder = other.SortOrder,
        FilterText = other.FilterText,
        FilterArgument = other.FilterArgument,
        FilterOperator = other.FilterOperator,
        Sortable = other.Sortable,
        DataType = other.DataType,
        Searchable = other.Searchable,
        Aggregate = other.Aggregate,
    };
}
