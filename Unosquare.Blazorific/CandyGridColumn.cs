namespace Unosquare.Blazorific;

/// <summary>
/// Represents a column within a <see cref="CandyGrid"/>.
/// </summary>
/// <seealso cref="IComponent" />
/// <seealso cref="IGridDataColumn" />
public class CandyGridColumn : IComponent, IGridDataColumn
{
    private static readonly int MinSortDirection = Enum.GetValues(typeof(SortDirection)).Cast<int>().Min();
    private static readonly int MaxSortDirection = Enum.GetValues(typeof(SortDirection)).Cast<int>().Max();
    private bool HasInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="CandyGridColumn"/> class.
    /// </summary>
    public CandyGridColumn()
    {
        // placeholder
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CandyGridColumn"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="parent">The parent.</param>
    internal CandyGridColumn(IPropertyProxy property, CandyGrid? parent)
    {
        // TODO: Create automatic columns from type.
        // Make it smarter
        PropertyProxy = property;
        Title = property.PropertyName;
        Field = property.PropertyName;
        IsSortable = true;
        IsSearchable = false; // property.PropertyType == typeof(string); // default false because it might be dbqueries.
        Parent = parent;
        HasInitialized = true;
    }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the field.
    /// </summary>
    /// <value>
    /// The field.
    /// </value>
    [Parameter]
    public string? Field { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is sortable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is sortable; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool IsSortable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is searchable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is searchable; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool IsSearchable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool IsVisible { get; set; }

    /// <summary>
    /// Set the sort order, zero or less are ignored.
    /// </summary>
    [Parameter]
    public int SortOrder { get; set; }

    /// <summary>
    /// Set the sort direction.
    /// </summary>
    [Parameter]
    public SortDirection SortDirection { get; set; }

    /// <summary>
    /// The Aggregation Function.
    /// </summary>
    [Parameter]
    public AggregationFunction Aggregate { get; set; }

    /// <summary>
    /// Gets or sets the data template.
    /// </summary>
    /// <value>
    /// The data template.
    /// </value>
    [Parameter]
    public RenderFragment<GridCellData>? DataTemplate { get; set; }

    /// <summary>
    /// Gets or sets the header template.
    /// </summary>
    /// <value>
    /// The header template.
    /// </value>
    [Parameter]
    public RenderFragment<CandyGridColumn>? HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the format string.
    /// </summary>
    /// <value>
    /// The format string.
    /// </value>
    [Parameter]
    public string? FormatString { get; set; }

    /// <summary>
    /// Gets or sets the empty display string.
    /// </summary>
    /// <value>
    /// The empty display string.
    /// </value>
    [Parameter]
    public string? EmptyDisplayString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alignment.
    /// </summary>
    /// <value>
    /// The alignment.
    /// </value>
    [Parameter]
    public TextAlignment Alignment { get; set; } = TextAlignment.Auto;

    /// <summary>
    /// Gets or sets the type of the filter data.
    /// </summary>
    /// <value>
    /// The type of the filter data.
    /// </value>
    [Parameter]
    public DataType? FilterDataType { get; set; }

    /// <summary>
    /// Gets or sets the filter options provider.
    /// </summary>
    /// <value>
    /// The filter options provider.
    /// </value>
    [Parameter]
    public Func<Task<Dictionary<string, string>>>? FilterOptionsProvider { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show filter].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show filter]; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool ShowFilter { get; set; } = true;

    /// <summary>
    /// Gets or sets the checked property.
    /// </summary>
    /// <value>
    /// The checked property.
    /// </value>
    [Parameter]
    public string? CheckedProperty { get; set; }

    /// <summary>
    /// Gets or sets the header CSS class.
    /// </summary>
    /// <value>
    /// The header CSS class.
    /// </value>
    [Parameter]
    public string? HeaderCssClass { get; set; }

    /// <summary>
    /// Gets or sets the cell CSS class.
    /// </summary>
    /// <value>
    /// The cell CSS class.
    /// </value>
    [Parameter]
    public string? CellCssClass { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    [Parameter]
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the on delete button click.
    /// </summary>
    /// <value>
    /// The on delete button click.
    /// </value>
    [Parameter]
    public Action<GridRowMouseEventArgs>? OnDeleteButtonClick { get; set; }

    /// <summary>
    /// Gets or sets the on details button click.
    /// </summary>
    /// <value>
    /// The on details button click.
    /// </value>
    [Parameter]
    public Action<GridRowMouseEventArgs>? OnDetailsButtonClick { get; set; }

    /// <summary>
    /// Gets or sets the on edit button click.
    /// </summary>
    /// <value>
    /// The on edit button click.
    /// </value>
    [Parameter]
    public Action<GridRowMouseEventArgs>? OnEditButtonClick { get; set; }


    /// <summary>
    /// Gets or sets the on cell checked changed.
    /// </summary>
    /// <value>
    /// The on cell checked changed.
    /// </value>
    [Parameter]
    public Action<GridCellCheckboxEventArgs>? OnCellCheckedChanged { get; set; }

    /// <summary>
    /// Gets or sets the parent.
    /// </summary>
    /// <value>
    /// The parent.
    /// </value>
    [CascadingParameter(Name = nameof(Parent))]
    protected CandyGrid? Parent { get; set; }

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <value>
    /// The property.
    /// </value>
    public IPropertyProxy? Property => PropertyProxy
        ?? (string.IsNullOrWhiteSpace(Field) ? null : Parent?.CoercedDataAdapter?.DataItemType.Property(Field));

    /// <summary>
    /// Filter search text.
    /// </summary>
    public string? FilterText { get; set; }

    /// <summary>
    /// Filter search params.
    /// </summary>
    public string?[] FilterArgument { get; set; } = Array.Empty<string?>();

    /// <summary>
    /// Filter operator.
    /// </summary>
    public CompareOperators FilterOperator { get; set; }

    /// <summary>
    /// Gets or sets the header component.
    /// </summary>
    /// <value>
    /// The header component.
    /// </value>
    internal CandyGridColumnHeader? HeaderComponent { get; set; }

    /// <summary>
    /// Gets or sets the property proxy.
    /// </summary>
    /// <value>
    /// The property proxy.
    /// </value>
    private IPropertyProxy? PropertyProxy { get; set; }

    /// <summary>
    /// Gets the checked property proxy.
    /// </summary>
    /// <value>
    /// The checked property proxy.
    /// </value>
    internal IPropertyProxy? CheckedPropertyProxy { get; private set; }

    /// <inheritdoc />
    string? IGridDataColumn.Name => Field;

    /// <inheritdoc />
    bool IGridDataColumn.Sortable => IsSortable;

    /// <inheritdoc />
    bool IGridDataColumn.Searchable => IsSearchable;

    /// <inheritdoc />
    DataType IGridDataColumn.DataType => FilterDataType
        ?? (Property?.PropertyType.NativeType ?? typeof(string)).GetDataType();

    /// <summary>
    /// Changes the sort direction.
    /// </summary>
    /// <param name="multiColumnSorting">if set to <c>true</c> [multi column sorting].</param>
    public void ChangeSortDirection(bool multiColumnSorting)
    {
        // Compute the next sort direction and set it.
        var nextSortDirection = (int)SortDirection + 1;
        if (nextSortDirection > MaxSortDirection)
            nextSortDirection = MinSortDirection;

        SortDirection = multiColumnSorting && nextSortDirection == MinSortDirection
            ? (SortDirection)(MinSortDirection + 1)
            : (SortDirection)nextSortDirection;

        // Clear the sort order for all columns with no sort direction
        foreach (var column in Parent?.Columns ?? Array.Empty<CandyGridColumn>())
        {
            column.SortOrder = column.SortDirection != SortDirection.None
                ? column.SortOrder
                : 0;
        }

        if (multiColumnSorting)
        {
            SortOrder = SortOrder <= 0 && Parent is not null
                ? Parent.Columns.Max(c => c.SortOrder) + 1
                : SortOrder;
        }
        else
        {
            // Reset sort order and sort direction for all columns
            // except for this one
            foreach (var column in Parent?.Columns ?? Array.Empty<CandyGridColumn>())
            {
                if (column == this)
                    continue;

                column.SortDirection = SortDirection.None;
                column.SortOrder = 0;
            }

            SortOrder = SortDirection == SortDirection.None ? 0 : 1;
        }

        // Reorganize sort orders for sorted columns
        var columnSortOrder = 1;
        var sortedColumns = Parent?.Columns.Where(c => c.SortOrder > 0).OrderBy(c => c.SortOrder).ToList()
            ?? new List<CandyGridColumn>(new[] { this });

        foreach (var column in sortedColumns)
            column.SortOrder = columnSortOrder++;

        Parent?.QueueDataUpdate();
    }

    /// <summary>
    /// Applies the filter.
    /// </summary>
    /// <param name="filterOp">The filter op.</param>
    /// <param name="args">The arguments.</param>
    public void ApplyFilter(CompareOperators filterOp, params string?[] args)
    {
        if (filterOp == CompareOperators.Auto || filterOp == CompareOperators.None || args is null || args.Length <= 0)
        {
            FilterArgument = Array.Empty<string>();
            FilterText = null;
            FilterOperator = CompareOperators.None;
            Parent?.QueueDataUpdate();
            return;
        }

        FilterOperator = filterOp;

        if (FilterOperator == CompareOperators.Multiple)
        {
            FilterText = null;
            FilterArgument = args;
        }
        else
        {
            FilterText = args[0];
            var arguments = new List<string>(args.Length - 1);
            for (var i = 1; i < args.Length; i++)
                arguments.Add(args[i]!);

            FilterArgument = arguments.ToArray();
        }

        Parent?.QueueDataUpdate();
    }

    /// <summary>
    /// Coerces the state.
    /// </summary>
    /// <param name="state">The state.</param>
    internal void CoerceState(GridState state)
    {
        var colState = state.Columns?
            .FirstOrDefault(c => c.Name is not null && c.Name.Equals(Field, StringComparison.CurrentCultureIgnoreCase));
        
        if (colState is null)
            return;

        SortDirection = colState.SortDirection;
        SortOrder = colState.SortOrder;
        FilterOperator = colState.FilterOperator;
        if (FilterOperator != CompareOperators.None)
        {
            FilterText = colState.FilterText;
            FilterArgument = colState.FilterArgument;
        }
    }

    /// <inheritdoc />
    void IComponent.Attach(RenderHandle renderHandle)
    {
        // placeholder
    }

    /// <inheritdoc />
    Task IComponent.SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (HasInitialized)
            return Task.CompletedTask;

        try
        {
            OnInitialized();
        }
        finally
        {
            HasInitialized = true;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected virtual void OnInitialized()
    {
        CheckedPropertyProxy = GetCheckedProperty();
        Parent?.AddColumn(this);
    }

    /// <summary>
    /// Gets the checked property.
    /// </summary>
    /// <returns></returns>
    private IPropertyProxy? GetCheckedProperty()
    {
        var checkedProxy = !string.IsNullOrWhiteSpace(CheckedProperty)
            ? Parent?.CoercedDataAdapter?.DataItemType.Property(CheckedProperty)
            : null;

        if (checkedProxy is null || checkedProxy.PropertyType.BackingType.NativeType != typeof(bool))
            return null;

        return checkedProxy;
    }
}