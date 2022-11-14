namespace Unosquare.Blazorific.Common;

/// <summary>
/// Defines a basic class for grid event arguments.
/// </summary>
/// <seealso cref="System.EventArgs" />
public class GridEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridEventArgs"/> class.
    /// </summary>
    /// <param name="grid">The grid.</param>
    public GridEventArgs(CandyGrid? grid)
    {
        Grid = grid;
    }

    /// <summary>
    /// Gets the grid.
    /// </summary>
    /// <value>
    /// The grid.
    /// </value>
    public CandyGrid? Grid { get; }
}

/// <summary>
/// Defines the arguments for a change of state in a grid.
/// </summary>
/// <seealso cref="GridEventArgs" />
public class GridStateEventArgs : GridEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridStateEventArgs"/> class.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <param name="state">The state.</param>
    /// <param name="isReset">if set to <c>true</c> [is reset].</param>
    public GridStateEventArgs(CandyGrid grid, GridState state, bool isReset)
        : base(grid)
    {
        State = state;
        IsReset = isReset;
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public GridState State { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is reset.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is reset; otherwise, <c>false</c>.
    /// </value>
    public bool IsReset { get; }
}

/// <summary>
/// Provides a class with arguments for grid exception events.
/// </summary>
/// <seealso cref="GridEventArgs" />
public class GridExceptionEventArgs : GridEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridExceptionEventArgs"/> class.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <param name="exception">The exception.</param>
    public GridExceptionEventArgs(CandyGrid grid, Exception exception)
        : base(grid)
    {
        Exception = exception;
    }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    public Exception Exception { get; }
}

/// <summary>
/// Provides event arguments for mouse events on the grid.
/// </summary>
/// <seealso cref="GridEventArgs" />
public class GridRowMouseEventArgs : GridEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridRowMouseEventArgs"/> class.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="mouse">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    public GridRowMouseEventArgs(CandyGridRow? row, MouseEventArgs? mouse)
        : base(row?.Grid)
    {
        Row = row;
        Mouse = mouse;
    }

    /// <summary>
    /// Gets the row.
    /// </summary>
    /// <value>
    /// The row.
    /// </value>
    public CandyGridRow? Row { get; }

    /// <summary>
    /// Gets the mouse.
    /// </summary>
    /// <value>
    /// The mouse.
    /// </value>
    public MouseEventArgs? Mouse { get; }

    /// <summary>
    /// Gets the data item.
    /// </summary>
    /// <value>
    /// The data item.
    /// </value>
    public object? DataItem => Row?.DataItem;
}

/// <summary>
/// Provides event arguments for checkbox events on the grid.
/// </summary>
/// <seealso cref="GridEventArgs" />
public class GridCellCheckboxEventArgs : GridEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GridCellCheckboxEventArgs"/> class.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
    public GridCellCheckboxEventArgs(CandyGridRow row, CandyGridColumn column, bool isChecked)
        : base(row.Grid)
    {
        IsChecked = isChecked;
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Gets the row.
    /// </summary>
    /// <value>
    /// The row.
    /// </value>
    public CandyGridRow Row { get; }

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>
    /// The column.
    /// </value>
    public CandyGridColumn Column { get; }

    /// <summary>
    /// Gets the data item.
    /// </summary>
    /// <value>
    /// The data item.
    /// </value>
    public object? DataItem => Row?.DataItem;

    /// <summary>
    /// Gets a value indicating whether this instance is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is checked; otherwise, <c>false</c>.
    /// </value>
    public bool IsChecked { get; }
}
