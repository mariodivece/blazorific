namespace Unosquare.Blazorific.Common;

/// <summary>
/// Provides model data for grid cell render fragments.
/// </summary>
public class GridCellData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridCellData"/> class.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <param name="dataItem">The data item.</param>
    internal GridCellData(CandyGridRow? row, CandyGridColumn? column, object? dataItem)
    {
        Row = row;
        Column = column;
        DataItem = dataItem;
    }

    /// <summary>
    /// Gets the row.
    /// </summary>
    /// <value>
    /// The row.
    /// </value>
    public CandyGridRow? Row { get; }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>
    /// The column.
    /// </value>
    public CandyGridColumn? Column { get; }

    /// <summary>
    /// Gets the data item.
    /// </summary>
    /// <value>
    /// The data item.
    /// </value>
    public object? DataItem { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is footer.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is footer; otherwise, <c>false</c>.
    /// </value>
    public bool IsFooter => Row?.IsFooter ?? false;

    /// <summary>
    /// Items this instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? Item<T>() where T : class => DataItem as T;
}
