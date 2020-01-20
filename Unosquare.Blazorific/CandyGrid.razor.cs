namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
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
        private const int QueueProcessorDueTimeMs = 50;

        private readonly object SyncLock = new object();
        private readonly Timer QueueProcessor;
        private readonly List<CandyGridColumn> m_Columns = new List<CandyGridColumn>(32);

        private bool IsDisposed;
        private bool HasRendered;
        private bool IsProcessingQueue;
        private int PendingAdapterUpdates;
        private DateTime LastRenderTime;

        private IGridDataAdapter m_DataAdapter;

        public CandyGrid()
        {
            Request = new GridDataRequest(this);
            QueueProcessor = new Timer(async (s) =>
            {
                var pendingUpdates = 0;
                lock (SyncLock)
                {
                    if (IsProcessingQueue)
                        return;

                    IsProcessingQueue = true;
                    pendingUpdates = PendingAdapterUpdates;
                }

                try
                {
                    if (pendingUpdates <= 0 || IsDisposed)
                        return;

                    await UpdateDataAsync();
                }
                finally
                {
                    lock (SyncLock)
                    {
                        PendingAdapterUpdates -= pendingUpdates;
                        IsProcessingQueue = false;
                    }
                }
            }, null, Timeout.Infinite, Timeout.Infinite);
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
                Console.WriteLine("DataAdapter Rebound");
                if (value == null)
                    throw new InvalidOperationException($"The {nameof(DataAdapter)} cannot be set to null.");

                if (m_DataAdapter != null && value.DataItemType != m_DataAdapter.DataItemType)
                    throw new InvalidOperationException($"The {nameof(DataAdapter)} cannot be changed once it has been set.");

                if (value == m_DataAdapter || (m_DataAdapter != null && m_DataAdapter.DataItemType == value.DataItemType))
                    return;

                m_DataAdapter = value;
                IsLoading = true;
                QueueDataUpdate();
            }
        }

        [Parameter]
        public RenderFragment CandyGridColumns { get; set; }

        #endregion

        #region Parameters: CSS Classes

        [Parameter]
        public string RootCssClass { get; set; } = "candygrid-container";

        [Parameter]
        public string TableContainerCssClass { get; set; } = "table-responsive table-borderless candygrid-table";

        [Parameter]
        public string TableCssClass { get; set; } = "table table-striped table-bordered table-hover table-sm";

        [Parameter]
        public string TableHeaderCssClass { get; set; } = "thead-dark";

        #endregion

        #region Parameters: Templates

        [Parameter]
        public RenderFragment<CandyGrid> EmptyRecordsTemplate { get; set; }

        [Parameter]
        public RenderFragment<CandyGrid> LoadingRecordsTemplate { get; set; }

        [Parameter]
        public RenderFragment<CandyGrid> ToolbarTemplate { get; set; }

        [Parameter]
        public string EmtyRecordsText { get; set; } = "No records to display.";

        #endregion

        #region Parameters: Event Callbacks

        [Parameter]
        public Action<GridInputDataEventArgs> OnBodyRowDoubleClick { get; set; }

        [Parameter]
        public Action<GridInputDataEventArgs> OnBodyRowClick { get; set; }

        [Parameter]
        public Action<GridEventArgs> OnDataLoaded { get; set; }

        [Parameter]
        public Action<GridExceptionEventArgs> OnDataLoadFailed { get; set; }

        #endregion

        public IReadOnlyList<CandyGridColumn> Columns => m_Columns;

        public IReadOnlyList<object> DataItems { get; protected set; }

        public int PageNumber { get; protected set; }

        public int PageSize { get; protected set; }

        public int TotalRecordCount { get; protected set; }

        public int FilteredRecordCount { get; protected set; }

        public int TotalPages { get; protected set; }

        public int StartRecordNumber => Math.Max(0, 1 + (PageSize * (PageNumber - 1)));

        public int EndRecordNumber => Math.Max(0, StartRecordNumber + (DataItems?.Count ?? 0) - 1);

        public bool IsLoading { get; protected set; }

        private GridDataRequest Request { get; }

        public IReadOnlyList<T> GetData<T>() => DataItems?.Cast<T>()?.ToList();

        public void QueueDataUpdate()
        {
            lock (SyncLock)
            {
                if (IsDisposed) return;
                PendingAdapterUpdates++;
                QueueProcessor.Change(QueueProcessorDueTimeMs, Timeout.Infinite);
            }
        }

        public int ChangePageSize(int pageSize)
        {
            if (pageSize == PageSize)
                return PageSize;

            try
            {
                Request.PageSize = pageSize;
                return Request.PageSize;
            }
            finally
            {
                QueueDataUpdate();
            }
        }

        public int ChangePageNumber(int pageNumber)
        {
            if (pageNumber == PageNumber)
                return PageNumber;

            try
            {
                Request.PageNumber = pageNumber;
                return Request.PageNumber;
            }
            finally
            {
                QueueDataUpdate();
            }
        }

        public void ChangeSearchText(string searchText)
        {
            Request.Search.Text = searchText ?? string.Empty;

            if (Request.Search.Text.Length > 2)
            {
                Request.PageNumber = 1;
                Request.Search.Operator = CompareOperators.Auto;
                QueueDataUpdate();
                return;
            }

            if (Request.Search.Operator != CompareOperators.None)
            {
                Request.PageNumber = 1;
                Request.Search.Operator = CompareOperators.None;
                QueueDataUpdate();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void AddColumn(CandyGridColumn column)
        {
            m_Columns.Add(column);
            StateHasChanged();
        }

        internal void NotifyStateChanged() => StateHasChanged();

        private async Task UpdateDataAsync()
        {
            if (HasRendered && Columns.Count == 0 && DataAdapter != null)
            {
                m_Columns.AddRange(
                    GenerateColumnsFromType(m_DataAdapter.DataItemType));
            }

            try
            {
                IsLoading = true;
                if (DataAdapter == null)
                {
                    DataItems = default;
                    PageNumber = default;
                    FilteredRecordCount = default;
                    TotalRecordCount = default;
                    TotalPages = default;
                    return;
                }

                var response = await DataAdapter.RetrieveDataAsync(Request);

                lock (SyncLock)
                {
                    Request.Counter++;
                    DataItems = response.DataItems;
                    FilteredRecordCount = response.FilteredRecordCount;
                    TotalRecordCount = response.TotalRecordCount;
                    PageSize = Request.PageSize <= 0 ? response.FilteredRecordCount : Request.PageSize;
                    TotalPages = Extensions.ComputeTotalPages(PageSize, response.FilteredRecordCount);
                    PageNumber = response.CurrentPage > TotalPages
                        ? TotalPages
                        : response.CurrentPage < 0
                        ? 1
                        : response.CurrentPage;
                }

                OnDataLoaded?.Invoke(new GridEventArgs(this));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update {nameof(CandyGrid)}: {ex.Message} - {ex.StackTrace}");
                OnDataLoadFailed?.Invoke(new GridExceptionEventArgs(this, ex));
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var intervalDuration = LastRenderTime == default
                ? 0
                : DateTime.UtcNow.Subtract(LastRenderTime).TotalMilliseconds;

            LastRenderTime = DateTime.UtcNow;
            Console.WriteLine($"GRID RENDER. {LastRenderTime.Minute:00}:{LastRenderTime.Second:00}:{LastRenderTime.Millisecond:000}. Interval: {intervalDuration:0.00}");
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
                return;

            HasRendered = true;
            await Js.InvokeVoidAsync($"{nameof(CandyGrid)}.initialize");
            QueueDataUpdate();
        }

        private CandyGridColumn[] GenerateColumnsFromType(Type t)
        {
            var proxies = t.PropertyProxies().Values;
            var result = new List<CandyGridColumn>(proxies.Count());
            foreach (var proxy in proxies)
                result.Add(new CandyGridColumn(proxy));

            return result.ToArray();
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
    }
}
