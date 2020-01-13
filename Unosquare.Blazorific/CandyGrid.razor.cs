namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;

    public partial class CandyGrid
    {
        private const int QueueProcessorIntervalMs = 250;

        private readonly object SyncLock = new object();
        private readonly Timer QueueProcessor;
        private readonly List<CandyGridColumn> m_Columns = new List<CandyGridColumn>(32);

        private bool IsProcessingQueue;
        private long PendingAdapterUpdates;
        private int GridDataRequestIndex;

        private IGridDataAdapter m_DataAdapter;
        private int m_RequestedPageSize = 20;
        private int m_RequestedPageNumber = 1;

        private List<object> DataItems;

        public CandyGrid()
        {
            QueueProcessor = new Timer(async (s) =>
            {
                lock (SyncLock)
                {
                    if (IsProcessingQueue)
                        return;

                    IsProcessingQueue = true;
                }

                var pendingUpdates = Interlocked.Read(ref PendingAdapterUpdates);

                try
                {
                    if (pendingUpdates <= 0)
                        return;

                    await UpdateDataAsync();
                }
                finally
                {
                    Interlocked.Add(ref PendingAdapterUpdates, pendingUpdates * -1);
                    lock (SyncLock)
                        IsProcessingQueue = false;
                }
            }, null, Timeout.Infinite, Timeout.Infinite);
        }

        public int RequestedPageSize
        {
            get
            {
                return m_RequestedPageSize > 0 ? m_RequestedPageSize : -1;
            }
            set
            {
                m_RequestedPageSize = value;
                QueueDataUpdate();
            }
        }

        public int RequestedPageNumber
        {
            get
            {
                var maxPageNumber = RequestedPageSize > 0
                    ? ComputeTotalPages(RequestedPageSize, FilteredRecordCount)
                    : 1;

                return m_RequestedPageNumber < 1
                    ? 1
                    : m_RequestedPageNumber > maxPageNumber
                    ? maxPageNumber
                    : m_RequestedPageNumber;
            }
            set
            {
                m_RequestedPageNumber = value;
                QueueDataUpdate();
            }
        }

        [Parameter]
        public IGridDataAdapter DataAdapter
        {
            get
            {
                return m_DataAdapter;
            }
            set
            {
                if (value == m_DataAdapter)
                    return;

                m_DataAdapter = value;
                if (m_DataAdapter == null)
                {

                    return;
                }

                QueueDataUpdate();
            }
        }

        [Parameter]
        public RenderFragment CandyGridColumns { get; set; }

        [Parameter]
        public EventCallback<GridBodyRowEventArgs> OnBodyRowDoubleClick { get; set; }

        public int CurrentPage { get; protected set; }

        public int PageSize { get; protected set; }

        public int TotalRecordCount { get; protected set; }

        public int FilteredRecordCount { get; protected set; }

        public int TotalPages { get; protected set; }

        public bool IsLoading { get; protected set; }

        public string StatusText { get; protected set; }

        public int StartRecordNumber => Math.Max(0, 1 + (PageSize * (CurrentPage - 1)));

        public int EndRecordNumber => Math.Max(0, StartRecordNumber + (DataItems?.Count ?? 0) - 1);

        public IReadOnlyList<CandyGridColumn> Columns => m_Columns;

        public IEnumerable GetDataItems() => DataItems;

        public IEnumerable<T> GetDataItems<T>() => DataItems?.Cast<T>();

        public GridDataFilter SearchFilter { get; } = new GridDataFilter();

        public void QueueDataUpdate() => Interlocked.Increment(ref PendingAdapterUpdates);

        internal void AddColumn(CandyGridColumn column)
        {
            m_Columns.Add(column);
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
                return;

            await Js.InvokeVoidAsync($"{nameof(CandyGrid)}.initialize");
            QueueProcessor.Change(QueueProcessorIntervalMs, QueueProcessorIntervalMs);
        }

        private async Task UpdateDataAsync()
        {
            IsLoading = true;
            StatusText = "Loading data . . .";
            try
            {
                if (DataAdapter == null)
                {
                    DataItems = default;
                    CurrentPage = default;
                    FilteredRecordCount = default;
                    TotalRecordCount = default;
                    TotalPages = default;
                    StatusText = null;
                    return;
                }

                var request = CreateGridDataRequest();
                var response = await DataAdapter.RetrieveDataAsync(request);

                DataItems = new List<object>(response.DataItems.OfType<object>());
                FilteredRecordCount = response.FilteredRecordCount;
                TotalRecordCount = response.TotalRecordCount;

                PageSize = request.Take <= 0 ? response.FilteredRecordCount : request.Take;
                TotalPages = ComputeTotalPages(PageSize, response.FilteredRecordCount);

                CurrentPage = response.CurrentPage > TotalPages
                    ? TotalPages
                    : response.CurrentPage < 0
                    ? 1
                    : response.CurrentPage;

                StatusText = (DataItems as List<object>).Count <= 0
                    ? "No records to display."
                    : "Loaded grid data";

                Console.WriteLine($"Total Pages = {TotalPages}, Current Page = {CurrentPage}");
            }
            catch (Exception ex)
            {
                StatusText = $"{ex.Message} - {ex.StackTrace}";
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private object GetColumnValue(CandyGridColumn column, object dataItem)
        {
            if (string.IsNullOrWhiteSpace(column.Field))
                return null;

            if (dataItem == null)
                return null;

            if (column.Property == null)
            {
                column.Property = dataItem.GetType()
                    .GetPropertyProxies()
                    .FirstOrDefault(p => p.Name == column.Field);
            }

            return column.Property?.GetValue(dataItem);
        }

        private GridDataRequest CreateGridDataRequest()
        {
            lock (SyncLock)
            {
                return new GridDataRequest
                {
                    Counter = GridDataRequestIndex++,
                    Skip = Math.Max(0, RequestedPageNumber - 1) * RequestedPageSize,
                    Take = RequestedPageSize,
                    TimezoneOffset = (int)Math.Round(DateTime.UtcNow.Subtract(DateTime.Now).TotalMinutes, 0),
                    Search = SearchFilter,
                    Columns = DataAdapter.DataItemType.GetGridDataRequestColumns(this)
                };
            }
        }

        private async Task RaiseOnBodyRowDoubleClick(object dataItem)
        {
            if (!OnBodyRowDoubleClick.HasDelegate)
                return;

            await OnBodyRowDoubleClick.InvokeAsync(new GridBodyRowEventArgs(this as CandyGrid, dataItem));
        }

        private int ComputeTotalPages(int pageSize, int totalCount)
        {
            if (totalCount <= 0) return 0;
            if (pageSize <= 0) return totalCount;

            return (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
        }
    }
}
