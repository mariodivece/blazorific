namespace Unosquare.Blazorific;

/// <summary>
/// Represents a row within a <see cref="CandyGrid"/>.
/// </summary>
/// <seealso cref="CandyGridChildComponent" />
/// <seealso cref="IAttachedComponent" />
public sealed partial class CandyGridRow : CandyGridChildComponent, IAttachedComponent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CandyGridRow"/> class.
    /// </summary>
    public CandyGridRow()
    {
        Attributes = new AttributeDictionary(StateHasChanged);
    }

    /// <summary>
    /// Gets the data item.
    /// </summary>
    /// <value>
    /// The data item.
    /// </value>
    [CascadingParameter(Name = nameof(DataItem))]
    public object? DataItem { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is footer.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is footer; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool IsFooter { get; set; }

    /// <summary>
    /// Gets the attributes.
    /// </summary>
    /// <value>
    /// The attributes.
    /// </value>
    public AttributeDictionary Attributes { get; }

    /// <summary>
    /// Gets the index of the attached component.
    /// </summary>
    public int Index { get; private set; } = -1;

    /// <summary>
    /// Gets the cells.
    /// </summary>
    /// <value>
    /// The cells.
    /// </value>
    public IReadOnlyList<CandyGridCell> Cells { get; } = new List<CandyGridCell>(32);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Parent?.RemoveRow(this);
        Index = -1;
    }

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// </summary>
    protected override void OnInitialized()
    {
        Index = Parent?.AddRow(this) ?? -1;
    }

    /// <summary>
    /// Notifies the state changed.
    /// </summary>
    internal void NotifyStateChanged() => StateHasChanged();

    /// <summary>
    /// Adds the cell.
    /// </summary>
    /// <param name="cell">The cell.</param>
    /// <returns></returns>
    internal int AddCell(CandyGridCell cell) => Cells.AddAttachedComponent(cell);

    /// <summary>
    /// Removes the cell.
    /// </summary>
    /// <param name="cell">The cell.</param>
    internal void RemoveCell(CandyGridCell cell) => Cells.RemoveAttachedComponent(cell);

    /// <summary>
    /// Raises the on body row double click.
    /// </summary>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void RaiseOnBodyRowDoubleClick(MouseEventArgs e)
    {
        if (IsFooter) return;
        $"EVENT".Log(nameof(CandyGridCell), $"On{nameof(Parent.OnBodyRowDoubleClick)}");
        Parent?.OnBodyRowDoubleClick?.Invoke(new GridRowMouseEventArgs(this, e));
    }

    /// <summary>
    /// Raises the on body row click.
    /// </summary>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void RaiseOnBodyRowClick(MouseEventArgs e)
    {
        if (IsFooter) return;
        $"EVENT".Log(nameof(CandyGridCell), $"On{nameof(Parent.OnBodyRowClick)}");
        Parent?.OnBodyRowClick?.Invoke(new GridRowMouseEventArgs(this, e));
    }
}
