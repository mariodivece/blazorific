namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;

    public partial class CandyGrid : IDisposable
    {
        private const int QueueProcessorDueTimeMs = 100;

        private readonly object SyncLock = new object();
        private readonly Timer QueueProcessor;
        private readonly List<CandyGridColumn> m_Columns = new(32);

        private bool IsDisposed;
        private bool HasRendered;
        private bool HasLoadedState;
        private bool IsProcessingQueue;
        private int PendingAdapterUpdates;
        private int PendingRenderUpdates;
        private DateTime LastRenderTime;

        private IGridDataAdapter m_DataAdapter;

        public event EventHandler<GridStateEventArgs> StateLoaded;

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

            StateLoaded += (s, e) =>
            {
                SearchText = e.State.SearchText;
                PageNumber = e.State.PageNumber;
                PageSize = e.State.PageSize;
            };
        }

        #region Parameters: Data

        [Parameter]
        public IGridDataAdapter DataAdapter
        {
            get
            {
                return m_DataAdapter;
            }
            set
            {
                if (value == null)
                    throw new InvalidOperationException($"The {nameof(DataAdapter)} cannot be set to null.");

                if (value == m_DataAdapter)
                    return;

                m_DataAdapter = value;
                QueueDataUpdate();
                $"SET Called".Log(nameof(CandyGrid), nameof(DataAdapter));
            }
        }

        [Parameter]
        public RenderFragment CandyGridColumns { get; set; }

        [Parameter]
        public string LocalStorageKey { get; set; }

        [Parameter]
        public bool DisableVirtualization { get; set; }

        #endregion

        #region Parameters: CSS Classes

        [Parameter]
        public string RootCssClass { get; set; }

        [Parameter]
        public string TableContainerCssClass { get; set; } = "table-responsive";

        [Parameter]
        public string TableCssClass { get; set; } = "table table-striped table-hover table-sm";

        [Parameter]
        public string TableHeaderCssClass { get; set; } = "thead-dark";

        [Parameter]
        public string TableFooterCssClass { get; set; } = "thead-dark";

        #endregion

        #region Parameters: Templates

        [Parameter]
        public RenderFragment<CandyGrid> EmptyRecordsTemplate { get; set; }

        [Parameter]
        public RenderFragment<CandyGrid> ToolbarTemplate { get; set; }

        [Parameter]
        public string EmtyRecordsText { get; set; } = "No records to display.";

        #endregion

        #region Parameters: Event Callbacks

        [Parameter]
        public Action<GridRowMouseEventArgs> OnBodyRowDoubleClick { get; set; }

        [Parameter]
        public Action<GridRowMouseEventArgs> OnBodyRowClick { get; set; }

        [Parameter]
        public Action<GridEventArgs> OnDataLoaded { get; set; }

        [Parameter]
        public Action<GridExceptionEventArgs> OnDataLoadFailed { get; set; }

        #endregion

        private ElementReference RootElement { get; set; }

        public IReadOnlyList<CandyGridColumn> Columns => m_Columns;

        public IReadOnlyList<CandyGridRow> Rows { get; } = new List<CandyGridRow>(1024);

        public ICollection<object> DataItems { get; protected set; }

        public object AggregateDataItem { get; protected set; }

        public int PageNumber { get; protected set; } = 1;

        public int PageSize { get; protected set; } = 20;

        public int TotalRecordCount { get; protected set; }

        public int PageRecordCount { get; protected set; }

        public int FilteredRecordCount { get; protected set; }

        public int TotalPages { get; protected set; }

        public int StartRecordNumber => (DataItems?.Count ?? 0) > 0
            ? Math.Max(0, 1 + (PageSize * (PageNumber - 1)))
            : 0;

        public int EndRecordNumber => Math.Max(0, StartRecordNumber + (DataItems?.Count ?? 0) - 1);

        public string SearchText { get; protected set; }

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

        public IReadOnlyList<T> GetData<T>() => DataItems?.Cast<T>()?.ToList();

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

        public int ChangePageSize(int pageSize)
        {
            if (pageSize != PageSize)
            {
                PageSize = pageSize;
                QueueDataUpdate();
            }

            return PageSize;
        }

        public int ChangePageNumber(int pageNumber)
        {
            if (pageNumber != PageNumber)
            {
                PageNumber = pageNumber;
                QueueDataUpdate();
            }

            return PageNumber;
        }

        public void ChangeSearchText(string searchText)
        {
            var effectiveTerm = (searchText ?? string.Empty).Length > 2 ? searchText : string.Empty;
            if (string.IsNullOrWhiteSpace(SearchText)) SearchText = string.Empty;

            if (effectiveTerm != SearchText)
            {
                SearchText = effectiveTerm;
                QueueDataUpdate();
            }
        }

        public async Task ResetState()
        {
            await Js.StorageRemoveItemAsync(LocalStorageKey);
            var state = GridState.CreateDefault(this);
            await InvokeAsync(() => StateLoaded?.Invoke(this, new GridStateEventArgs(this, state, true)));
            QueueDataUpdate();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void AddColumn(CandyGridColumn column)
        {
            m_Columns.Add(column);
            QueueRenderUpdate();
        }

        internal int AddRow(CandyGridRow row) => Rows.AddAttachedComponent(row);

        internal void RemoveRow(CandyGridRow row) => Rows.RemoveAttachedComponent(row);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var intervalDuration = LastRenderTime == default
                ? 0
                : DateTime.UtcNow.Subtract(LastRenderTime).TotalMilliseconds;
            LastRenderTime = DateTime.UtcNow;
            var currentRenderTime = $"{LastRenderTime.Minute:00}:{LastRenderTime.Second:00}:{LastRenderTime.Millisecond:000}";

            $"Current: {currentRenderTime} Previous: {intervalDuration} ms. ago (FR: {firstRender})".Log(nameof(CandyGrid), nameof(OnAfterRender));
            await Js.GridFireOnRendered(RootElement, firstRender);

            if (!firstRender)
                return;

            HasRendered = true;
            QueueDataUpdate();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            "CALLED".Log(nameof(CandyGrid), nameof(SetParametersAsync));
        }

        protected override void OnParametersSet()
        {
            "CALLED".Log(nameof(CandyGrid), nameof(OnParametersSet));
        }

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

        private CandyGridColumn[] GenerateColumnsFromType(Type t)
        {
            var proxies = t.PropertyProxies().Values.Where(t => t.IsFlatType);
            var result = new List<CandyGridColumn>(proxies.Count());
            foreach (var proxy in proxies)
                result.Add(new CandyGridColumn(proxy, this));

            return result.ToArray();
        }

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

        private async Task LoadState()
        {
            if (string.IsNullOrWhiteSpace(LocalStorageKey))
                return;

            var json = await Js.StorageGetItemAsync(LocalStorageKey);
            if (string.IsNullOrWhiteSpace(json))
                return;

            GridState state = null;
            try
            {
                state = GridState.Deserialize(json);
            }
            catch (Exception ex)
            {
                $"Unable to deserialize state. {ex.Message}".Log(nameof(CandyGrid), nameof(LoadState));
                await Js.StorageRemoveItemAsync(LocalStorageKey);
            }

            if (state == null)
                return;

            await InvokeAsync(() => StateLoaded?.Invoke(this, new GridStateEventArgs(this, state, false)));
        }

        private async Task SaveState()
        {
            if (string.IsNullOrWhiteSpace(LocalStorageKey))
                return;

            var json = GridState.FromGrid(this, Request).Serialize();
            await Js.StorageSetItemAsync(LocalStorageKey, json);
        }

        private async Task UpdateDataAsync()
        {
            if (HasRendered && Columns.Count == 0 && DataAdapter != null)
            {
                m_Columns.AddRange(
                    GenerateColumnsFromType(m_DataAdapter.DataItemType));
            }

            try
            {
                if (DataAdapter == null)
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
                var response = await DataAdapter.RetrieveDataAsync(Request);

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
                await Js.GridFireOnDataLoaded(RootElement);
            }
            catch (Exception ex)
            {
                $"Failed to update. {ex.Message} - {ex.StackTrace}".Log(nameof(CandyGrid), nameof(UpdateDataAsync));
                await InvokeAsync(() => OnDataLoadFailed?.Invoke(new GridExceptionEventArgs(this, ex)));
            }
        }

        private async Task SignalDataLoading()
        {
            // prevents re-rendering the grid just to block the UI, so we do it via Javascript interop
            await Js.GridFireOnDataLoading(RootElement);
        }
    }
}
