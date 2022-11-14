namespace Unosquare.Blazorific;

/// <summary>
/// Represents a cell within a <see cref="CandyGrid"/>.
/// </summary>
/// <seealso cref="ComponentBase" />
/// <seealso cref="Unosquare.Blazorific.Common.IAttachedComponent" />
/// <seealso cref="System.IDisposable" />
public sealed partial class CandyGridCell : IAttachedComponent, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CandyGridCell"/> class.
    /// </summary>
    public CandyGridCell()
    {
        Attributes = new AttributeDictionary(StateHasChanged);
    }

    private enum GridButtonEventType
    {
        DetailsButtonClick,
        EditButtonClick,
        DeleteButtonClick,
    }

    public AttributeDictionary Attributes { get; }

    /// <inheridoc />
    public int Index { get; private set; } = -1;

    /// <summary>
    /// Gets the column.
    /// </summary>
    /// <value>
    /// The column.
    /// </value>
    [CascadingParameter(Name = nameof(Column))]
    public CandyGridColumn Column { get; private set; }

    /// <summary>
    /// Gets the row.
    /// </summary>
    /// <value>
    /// The row.
    /// </value>
    [CascadingParameter(Name = nameof(Row))]
    public CandyGridRow Row { get; private set; }

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <value>
    /// The property.
    /// </value>
    public IPropertyProxy? Property => Column?.Property;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is checked; otherwise, <c>false</c>.
    /// </value>
    private bool IsChecked
    {
        get
        {
            return DataItem is not null && (bool)(Column.CheckedPropertyProxy?.Read(DataItem) ?? false);
        }
        set
        {
            if (value == IsChecked || DataItem is null) return;
            Column.CheckedPropertyProxy?.Write(DataItem, value);
            RaiseOnCellCheckedChanged(value);
        }
    }

    private string? TextAlignCssClass { get; set; }

    private object? DataItem => Row?.DataItem;

    private bool HasAutomaticButtons =>
        Column.OnDeleteButtonClick is not null ||
        Column.OnDetailsButtonClick is not null ||
        Column.OnEditButtonClick is not null;

    private bool HasAutomaticCheckbox => Column.CheckedPropertyProxy is not null;

    /// <inheridoc />
    public void Dispose()
    {
        Row?.RemoveCell(this);
    }

    /// <inheridoc />
    protected override void OnInitialized()
    {
        Index = Row?.AddCell(this) ?? -1;
        TextAlignCssClass = GetTextAlignCssClass();
    }

    /// <inheridoc />
    private string? GetTextAlignCssClass()
    {
        if (Column.Alignment != TextAlignment.Auto)
        {
            return Column.Alignment switch
            {
                TextAlignment.Center => "text-center",
                TextAlignment.Left => "text-start",
                TextAlignment.Right => "text-end",
                TextAlignment.Justify => "text-start",
                _ => null
            };
        }

        if (Property is null) return null;

        var t = Property.PropertyType;
        return t.IsNumeric || t.BackingType.NativeType == typeof(DateTime)
            ? "text-end"
            : t.BackingType.NativeType == typeof(bool)
            ? "text-center"
            : null;
    }

    private void RaiseOnRowButtonClick(MouseEventArgs e, GridButtonEventType eventType)
    {
        var callback = eventType switch
        {
            GridButtonEventType.EditButtonClick => Column.OnEditButtonClick,
            GridButtonEventType.DeleteButtonClick => Column.OnDeleteButtonClick,
            _ => Column.OnDetailsButtonClick
        };

        if (callback is null) return;

        $"EVENT".Log(nameof(CandyGridCell), $"On{eventType}");
        callback?.Invoke(new GridRowMouseEventArgs(Row, e));
        Row?.NotifyStateChanged();
    }

    private void RaiseOnCellCheckedChanged(bool isChecked)
    {
        $"EVENT".Log(nameof(CandyGridCell), nameof(CandyGridColumn.OnCellCheckedChanged));
        Column.OnCellCheckedChanged?.Invoke(new GridCellCheckboxEventArgs(Row, Column, isChecked));
        Row?.NotifyStateChanged();
    }
}
