namespace Unosquare.Blazorific;

/// <summary>
/// Defines a group of elements and interaction logic that represent a tabular data display.
/// </summary>
/// <seealso cref="CandyComponentBase" />
/// <seealso cref="IDisposable" />
public partial class CandyGrid : IDisposable
{
    private const int QueueProcessorDueTimeMs = 100;

    private readonly object SyncLock = new();
    private readonly Timer QueueProcessor;
    private readonly List<CandyGridColumn> m_Columns = new(32);

    private bool IsDisposed;
    private bool HasRendered;
    private bool HasLoadedState;
    private bool IsProcessingQueue;
    private int PendingAdapterUpdates;
    private int PendingRenderUpdates;
    private DateTime LastRenderTime;

    private IGridDataAdapter? m_DataAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CandyGrid"/> class.
    /// </summary>
    public CandyGrid()
    {
        Request = new GridDataRequest();
        QueueProcessor = new Timer(async (s) =>
        {
            var pendingDataUpdates = 0;
            var pendingRenderUpdates = 0;
            lock (SyncLock)
            {
                if (IsProcessingQueue)
                    return;

                IsProcessingQueue = true;
                pendingDataUpdates = PendingAdapterUpdates;
                pendingRenderUpdates = PendingRenderUpdates;
            }

            try
            {
                if (pendingDataUpdates > 0 && !IsDisposed)
                    await UpdateDataAsync();
            }
            finally
            {
                lock (SyncLock)
                {
                    PendingAdapterUpdates -= pendingDataUpdates;
                    PendingRenderUpdates -= pendingRenderUpdates;
                    IsProcessingQueue = false;

                    if (PendingAdapterUpdates <= 0 && PendingRenderUpdates <= 0)
                        StateHasChanged();
                }
            }
        }, null, Timeout.Infinite, Timeout.Infinite);
    }

    #region Parameters: Data

    /// <summary>
    /// Gets or sets the data adapter.
    /// </summary>
    /// <value>
    /// The data adapter.
    /// </value>
    /// <exception cref="System.InvalidOperationException">The {nameof(DataAdapter)} cannot be set to null.</exception>
    [Parameter]
    public IGridDataAdapter? DataAdapter
    {
        get
        {
            return m_DataAdapter;
        }
        set
        {
            if (value is null)
                throw new InvalidOperationException($"The {nameof(DataAdapter)} cannot be set to null.");

            if (value == m_DataAdapter)
                return;

            m_DataAdapter = value;
            QueueDataUpdate();
            $"SET Called".Log(nameof(CandyGrid), nameof(DataAdapter));
        }
    }

    /// <summary>
    /// Gets or sets the candy grid columns.
    /// </summary>
    /// <value>
    /// The candy grid columns.
    /// </value>
    [Parameter]
    public RenderFragment? CandyGridColumns { get; set; }

    /// <summary>
    /// Gets or sets the local storage key.
    /// </summary>
    /// <value>
    /// The local storage key.
    /// </value>
    [Parameter]
    public string? LocalStorageKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [disable virtualization].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [disable virtualization]; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool DisableVirtualization { get; set; }

    #endregion

    #region Parameters: CSS Classes

    /// <summary>
    /// Gets or sets the root CSS class.
    /// </summary>
    /// <value>
    /// The root CSS class.
    /// </value>
    [Parameter]
    public string? RootCssClass { get; set; }

    /// <summary>
    /// Gets or sets the table container CSS class.
    /// </summary>
    /// <value>
    /// The table container CSS class.
    /// </value>
    [Parameter]
    public string? TableContainerCssClass { get; set; } = "table-responsive";

    /// <summary>
    /// Gets or sets the table CSS class.
    /// </summary>
    /// <value>
    /// The table CSS class.
    /// </value>
    [Parameter]
    public string? TableCssClass { get; set; } = "table table-striped table-hover table-sm";

    /// <summary>
    /// Gets or sets the table header CSS class.
    /// </summary>
    /// <value>
    /// The table header CSS class.
    /// </value>
    [Parameter]
    public string? TableHeaderCssClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the table body CSS class.
    /// </summary>
    /// <value>
    /// The table body CSS class.
    /// </value>
    [Parameter]
    public string? TableBodyCssClass { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the table footer CSS class.
    /// </summary>
    /// <value>
    /// The table footer CSS class.
    /// </value>
    [Parameter]
    public string? TableFooterCssClass { get; set; } = string.Empty;

    #endregion

    #region Parameters: Templates

    /// <summary>
    /// Gets or sets the empty records template.
    /// </summary>
    /// <value>
    /// The empty records template.
    /// </value>
    [Parameter]
    public RenderFragment<CandyGrid>? EmptyRecordsTemplate { get; set; }

    /// <summary>
    /// Gets or sets the toolbar template.
    /// </summary>
    /// <value>
    /// The toolbar template.
    /// </value>
    [Parameter]
    public RenderFragment<CandyGrid>? ToolbarTemplate { get; set; }

    /// <summary>
    /// Gets or sets the emty records text.
    /// </summary>
    /// <value>
    /// The emty records text.
    /// </value>
    [Parameter]
    public string EmtyRecordsText { get; set; } = "No records to display.";

    #endregion

    #region Parameters: Event Callbacks

    /// <summary>
    /// Gets or sets the on body row double click.
    /// </summary>
    /// <value>
    /// The on body row double click.
    /// </value>
    [Parameter]
    public Action<GridRowMouseEventArgs>? OnBodyRowDoubleClick { get; set; }

    /// <summary>
    /// Gets or sets the on body row click.
    /// </summary>
    /// <value>
    /// The on body row click.
    /// </value>
    [Parameter]
    public Action<GridRowMouseEventArgs>? OnBodyRowClick { get; set; }

    /// <summary>
    /// Gets or sets the on data loaded.
    /// </summary>
    /// <value>
    /// The on data loaded.
    /// </value>
    [Parameter]
    public Action<GridEventArgs>? OnDataLoaded { get; set; }

    /// <summary>
    /// Gets or sets the on data load failed.
    /// </summary>
    /// <value>
    /// The on data load failed.
    /// </value>
    [Parameter]
    public Action<GridExceptionEventArgs>? OnDataLoadFailed { get; set; }

    #endregion

    /// <summary>
    /// Gets or sets the root element.
    /// </summary>
    /// <value>
    /// The root element.
    /// </value>
    private ElementReference RootElement { get; set; }

    /// <summary>
    /// Gets or sets the search box.
    /// </summary>
    /// <value>
    /// The search box.
    /// </value>
    private CandyGridSearchBox? SearchBox { get; set; }

    /// <summary>
    /// Gets the columns.
    /// </summary>
    /// <value>
    /// The columns.
    /// </value>
    public IReadOnlyList<CandyGridColumn> Columns => m_Columns;

    /// <summary>
    /// Gets the rows.
    /// </summary>
    /// <value>
    /// The rows.
    /// </value>
    public IReadOnlyList<CandyGridRow> Rows { get; } = new List<CandyGridRow>(1024);

    /// <summary>
    /// Gets or sets the data items.
    /// </summary>
    /// <value>
    /// The data items.
    /// </value>
    public ICollection<object>? DataItems { get; protected set; }

    /// <summary>
    /// Gets or sets the aggregate data item.
    /// </summary>
    /// <value>
    /// The aggregate data item.
    /// </value>
    public object? AggregateDataItem { get; protected set; }

    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    /// <value>
    /// The page number.
    /// </value>
    public int PageNumber { get; protected set; } = 1;

    /// <summary>
    /// Gets or sets the size of the page.
    /// </summary>
    /// <value>
    /// The size of the page.
    /// </value>
    public int PageSize { get; protected set; } = 20;

    /// <summary>
    /// Gets or sets the total record count.
    /// </summary>
    /// <value>
    /// The total record count.
    /// </value>
    public int TotalRecordCount { get; protected set; }

    /// <summary>
    /// Gets or sets the page record count.
    /// </summary>
    /// <value>
    /// The page record count.
    /// </value>
    public int PageRecordCount { get; protected set; }

    /// <summary>
    /// Gets or sets the filtered record count.
    /// </summary>
    /// <value>
    /// The filtered record count.
    /// </value>
    public int FilteredRecordCount { get; protected set; }

    /// <summary>
    /// Gets or sets the total pages.
    /// </summary>
    /// <value>
    /// The total pages.
    /// </value>
    public int TotalPages { get; protected set; }

    /// <summary>
    /// Gets the start record number.
    /// </summary>
    /// <value>
    /// The start record number.
    /// </value>
    public int StartRecordNumber => (DataItems?.Count ?? 0) > 0
        ? Math.Max(0, 1 + (PageSize * (PageNumber - 1)))
        : 0;

    /// <summary>
    /// Gets the end record number.
    /// </summary>
    /// <value>
    /// The end record number.
    /// </value>
    public int EndRecordNumber => Math.Max(0, StartRecordNumber + (DataItems?.Count ?? 0) - 1);

    /// <summary>
    /// Gets or sets the search text.
    /// </summary>
    /// <value>
    /// The search text.
    /// </value>
    public string? SearchText { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether this instance is loading.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is loading; otherwise, <c>false</c>.
    /// </value>
    public bool IsLoading
    {
        get
        {
            lock (SyncLock)
            {
                return !IsDisposed && (!HasRendered || PendingAdapterUpdates > 0 || PendingRenderUpdates > 0);
            }
        }
    }

    private GridDataRequest Request { get; }

    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IReadOnlyList<T> GetData<T>() => DataItems?.Cast<T>()?.ToList() as IReadOnlyList<T> ?? Array.Empty<T>();

    /// <summary>
    /// Asks for the grid to update the data from the data adapter.
    /// </summary>
    public void QueueDataUpdate()
    {
        lock (SyncLock)
        {
            if (IsDisposed) return;
            PendingAdapterUpdates++;
            _ = SignalDataLoading();
            QueueProcessor.Change(QueueProcessorDueTimeMs, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Queues the render update.
    /// </summary>
    public void QueueRenderUpdate()
    {
        lock (SyncLock)
        {
            if (IsDisposed) return;
            PendingRenderUpdates++;
            _ = SignalDataLoading();
            QueueProcessor.Change(QueueProcessorDueTimeMs, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Changes the size of the page.
    /// </summary>
    /// <param name="pageSize">Size of the page.</param>
    /// <returns></returns>
    public int ChangePageSize(int pageSize)
    {
        if (pageSize != PageSize)
        {
            PageSize = pageSize;
            QueueDataUpdate();
        }

        return PageSize;
    }

    /// <summary>
    /// Changes the page number.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <returns></returns>
    public int ChangePageNumber(int pageNumber)
    {
        if (pageNumber != PageNumber)
        {
            PageNumber = pageNumber;
            QueueDataUpdate();
        }

        return PageNumber;
    }

    /// <summary>
    /// Changes the search text.
    /// </summary>
    /// <param name="searchText">The search text.</param>
    public void ChangeSearchText(string? searchText)
    {
        var effectiveTerm = (searchText ?? string.Empty).Length > 2 ? searchText : string.Empty;
        if (string.IsNullOrWhiteSpace(SearchText)) SearchText = string.Empty;

        if (effectiveTerm != SearchText)
        {
            SearchText = effectiveTerm;
            QueueDataUpdate();
        }
    }

    /// <summary>
    /// Resets the state.
    /// </summary>
    public async Task ResetState()
    {
        if (Js is not null && !string.IsNullOrWhiteSpace(LocalStorageKey))
            await Js.StorageRemoveItemAsync(LocalStorageKey);

        var state = GridState.CreateDefault(this);
        CoerceGridState(state);
        QueueDataUpdate();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Adds the column.
    /// </summary>
    /// <param name="column">The column.</param>
    internal void AddColumn(CandyGridColumn column)
    {
        m_Columns.Add(column);
        QueueRenderUpdate();
    }

    /// <summary>
    /// Adds the row.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns></returns>
    internal int AddRow(CandyGridRow row) => Rows.AddAttachedComponent(row);

    /// <summary>
    /// Removes the row.
    /// </summary>
    /// <param name="row">The row.</param>
    internal void RemoveRow(CandyGridRow row) => Rows.RemoveAttachedComponent(row);

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var intervalDuration = LastRenderTime == default
            ? 0
            : DateTime.UtcNow.Subtract(LastRenderTime).TotalMilliseconds;
        LastRenderTime = DateTime.UtcNow;
        var currentRenderTime = $"{LastRenderTime.Minute:00}:{LastRenderTime.Second:00}:{LastRenderTime.Millisecond:000}";

        if (HasRendered)
        {
            foreach (var col in Columns)
            {
                if (col.HeaderComponent is not null)
                    await col.HeaderComponent.RefreshFilterState();
            }
        }

        $"Current: {currentRenderTime} Previous: {intervalDuration} ms. ago (FR: {firstRender})".Log(nameof(CandyGrid), nameof(OnAfterRender));

        if (Js is not null)
            await Js.GridFireOnRendered(RootElement, firstRender);

        if (!firstRender)
            return;

        HasRendered = true;
        QueueDataUpdate();
    }

    /// <inheritdoc />
    protected virtual void Dispose(bool alsoManaged)
    {
        lock (SyncLock)
        {
            if (IsDisposed)
                return;

            QueueProcessor.Change(Timeout.Infinite, Timeout.Infinite);

            if (alsoManaged)
                QueueProcessor.Dispose();

            IsDisposed = true;
        }
    }

    /// <summary>
    /// Generates the type of the columns from the given type's properties.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    private CandyGridColumn[] GenerateColumnsFromType(Type t)
    {
        var proxies = t.Properties().Where(t => t.IsFlatType());
        var result = new List<CandyGridColumn>(proxies.Count());
        foreach (var proxy in proxies)
            result.Add(new CandyGridColumn(proxy, this));

        return result.ToArray();
    }

    /// <summary>
    /// Gets the relative width of the given column.
    /// </summary>
    /// <param name="col">The col.</param>
    /// <returns></returns>
    private string GetRelativeWidth(CandyGridColumn col)
    {
        var automaticColumns = Columns.Where(c => c.Width <= 0).ToArray();
        var specificColumns = Columns.Where(c => c.Width > 0).ToArray();

        var sumSpecific = specificColumns.Length > 0 ? (double)specificColumns.Sum(c => c.Width) : 0;
        var averageSpecific = specificColumns.Length > 0 ? sumSpecific / specificColumns.Length : 1;
        var totalWidth = sumSpecific + (automaticColumns.Length * averageSpecific);
        var relativeWidth = (col.Width <= 0 ? averageSpecific : col.Width) / totalWidth;

        return $"{Math.Round((relativeWidth * 100), 2):0.00}%";
    }

    /// <summary>
    /// Loads the state.
    /// </summary>
    private async Task LoadState()
    {
        if (string.IsNullOrWhiteSpace(LocalStorageKey) || Js is null)
            return;

        var json = await Js.StorageGetItemAsync(LocalStorageKey);
        if (string.IsNullOrWhiteSpace(json))
            return;

        GridState? state = null;
        try
        {
            state = GridState.Deserialize(json);
        }
        catch (Exception ex)
        {
            $"Unable to deserialize state. {ex.Message}".Log(nameof(CandyGrid), nameof(LoadState));
            await Js.StorageRemoveItemAsync(LocalStorageKey);
        }

        if (state is null)
            return;

        CoerceGridState(state);
    }

    /// <summary>
    /// Saves the state.
    /// </summary>
    private async Task SaveState()
    {
        if (string.IsNullOrWhiteSpace(LocalStorageKey) || Js is null)
            return;

        var json = GridState.FromGrid(this, Request).Serialize();
        await Js.StorageSetItemAsync(LocalStorageKey, json);
    }

    /// <summary>
    /// Updates the data asynchronous.
    /// </summary>
    private async Task UpdateDataAsync()
    {
        if (HasRendered && Columns.Count <= 0 && m_DataAdapter is not null)
        {
            m_Columns.AddRange(
                GenerateColumnsFromType(m_DataAdapter.DataItemType));
        }

        try
        {
            if (m_DataAdapter is null)
            {
                DataItems = default;
                PageNumber = default;
                FilteredRecordCount = default;
                TotalRecordCount = default;
                PageRecordCount = default;
                TotalPages = default;
                return;
            }

            if (!HasLoadedState)
            {
                await LoadState();
                HasLoadedState = true;
            }

            Request.UpdateFrom(this);
            var response = await m_DataAdapter.RetrieveDataAsync(Request);

            lock (SyncLock)
            {
                DataItems = response.DataItems;
                AggregateDataItem = response.AggregateDataItem;
                FilteredRecordCount = response.FilteredRecordCount;
                TotalRecordCount = response.TotalRecordCount;
                PageRecordCount = response.DataItems?.Count ?? 0;
                TotalPages = Extensions.ComputeTotalPages(PageSize, response.FilteredRecordCount);
                PageNumber = response.CurrentPage > TotalPages
                    ? TotalPages
                    : response.CurrentPage < 0
                    ? 1
                    : response.CurrentPage;
            }

            await SaveState();
            await InvokeAsync(() => OnDataLoaded?.Invoke(new GridEventArgs(this)));

            if (Js is not null)
                await Js.GridFireOnDataLoaded(RootElement);
        }
        catch (Exception ex)
        {
            $"Failed to update. {ex.Message} - {ex.StackTrace}".Log(nameof(CandyGrid), nameof(UpdateDataAsync));
            await InvokeAsync(() => OnDataLoadFailed?.Invoke(new GridExceptionEventArgs(this, ex)));
        }
    }

    /// <summary>
    /// Signals the data loading.
    /// Prevents re-rendering the grid just to block the UI, so we do it via Javascript interop
    /// </summary>
    private async Task SignalDataLoading()
    {
        if (Js is not null)
            await Js.GridFireOnDataLoading(RootElement);
    }

    /// <summary>
    /// Coerces the state of the grid.
    /// </summary>
    /// <param name="state">The state.</param>
    private void CoerceGridState(GridState state)
    {
        SearchText = state.SearchText;
        PageNumber = state.PageNumber;
        PageSize = state.PageSize;

        if (SearchBox is not null)
            SearchBox.SearchText = SearchText;

        foreach (var c in Columns)
            c.CoerceState(state);
    }
}
