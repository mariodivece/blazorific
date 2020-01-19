﻿namespace Unosquare.Blazorific
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
        private int GridDataRequestIndex;

        private IGridDataAdapter m_DataAdapter;
        private int m_RequestedPageSize = 20;
        private int m_RequestedPageNumber = 1;

        private List<object> DataItems;

        public CandyGrid()
        {
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
                if (value == null)
                    throw new InvalidOperationException($"The {nameof(DataAdapter)} cannot be set to null.");

                if (m_DataAdapter != null)
                    throw new InvalidOperationException($"The {nameof(DataAdapter)} cannot be changed once it has been set.");

                if (value == m_DataAdapter)
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
        public EventCallback<GridInputEventArgs<object>> OnBodyRowDoubleClick { get; set; }

        [Parameter]
        public EventCallback<GridInputEventArgs<object>> OnBodyRowClick { get; set; }

        [Parameter]
        public EventCallback<GridEventArgs> OnDataLoaded { get; set; }

        [Parameter]
        public EventCallback<GridEventArgs<Exception>> OnDataLoadFailed { get; set; }

        #endregion

        public IReadOnlyList<CandyGridColumn> Columns => m_Columns;

        public IReadOnlyList<object> Data => DataItems;

        public int PageNumber { get; protected set; }

        public int PageSize { get; protected set; }

        public int TotalRecordCount { get; protected set; }

        public int FilteredRecordCount { get; protected set; }

        public int TotalPages { get; protected set; }

        public int StartRecordNumber => Math.Max(0, 1 + (PageSize * (PageNumber - 1)));

        public int EndRecordNumber => Math.Max(0, StartRecordNumber + (DataItems?.Count ?? 0) - 1);

        public bool IsLoading { get; protected set; }

        private int RequestedPageSize
        {
            get => m_RequestedPageSize > 0 ? m_RequestedPageSize : -1;
            set => m_RequestedPageSize = value;
        }

        private int RequestedPageNumber
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
            }
        }

        private GridDataFilter SearchFilter { get; } = new GridDataFilter();

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

            RequestedPageSize = pageSize;
            QueueDataUpdate();
            return RequestedPageSize;
        }

        public int ChangePageNumber(int pageNumber)
        {
            if (pageNumber == PageNumber)
                return PageNumber;

            RequestedPageNumber = pageNumber;
            QueueDataUpdate();
            return RequestedPageNumber;
        }

        public void ChangeSearchText(string searchText)
        {
            SearchFilter.Text = searchText ?? string.Empty;

            if (SearchFilter.Text.Length > 2)
            {
                RequestedPageNumber = 1;
                SearchFilter.Operator = CompareOperators.Auto;
                QueueDataUpdate();
                return;
            }

            if (SearchFilter.Operator != CompareOperators.None)
            {
                RequestedPageNumber = 1;
                SearchFilter.Operator = CompareOperators.None;
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
        
        private async Task UpdateDataAsync()
        {
            if (HasRendered && Columns.Count == 0 && DataAdapter != null)
            {
                m_Columns.AddRange(
                    GenerateColumnsFromType(m_DataAdapter.DataItemType));
            }

            IsLoading = true;
            try
            {
                if (DataAdapter == null)
                {
                    DataItems = default;
                    PageNumber = default;
                    FilteredRecordCount = default;
                    TotalRecordCount = default;
                    TotalPages = default;
                    return;
                }

                var request = CreateGridDataRequest();
                var response = await DataAdapter.RetrieveDataAsync(request);

                DataItems = new List<object>(response.DataItems.OfType<object>());
                FilteredRecordCount = response.FilteredRecordCount;
                TotalRecordCount = response.TotalRecordCount;

                PageSize = request.Take <= 0 ? response.FilteredRecordCount : request.Take;
                TotalPages = ComputeTotalPages(PageSize, response.FilteredRecordCount);

                PageNumber = response.CurrentPage > TotalPages
                    ? TotalPages
                    : response.CurrentPage < 0
                    ? 1
                    : response.CurrentPage;

                if (OnDataLoaded.HasDelegate)
                    await OnDataLoaded.InvokeAsync(new GridEventArgs(this));
            }
            catch (Exception ex)
            {
                if (OnDataLoadFailed.HasDelegate)
                    await OnDataLoadFailed.InvokeAsync(new GridEventArgs<Exception>(this, ex));
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
                return;

            HasRendered = true;
            await Js.InvokeVoidAsync($"{nameof(CandyGrid)}.initialize");
            QueueProcessor.Change(QueueProcessorDueTimeMs, Timeout.Infinite);
        }

        private object GetColumnValue(CandyGridColumn column, object dataItem)
        {
            if (string.IsNullOrWhiteSpace(column.Field))
                return null;

            if (dataItem == null)
                return null;

            var proxies = dataItem.GetType().PropertyProxies();
            if (column.Property == null && !string.IsNullOrWhiteSpace(column.Field))
            {
                column.Property = proxies.ContainsKey(column.Field)
                    ? proxies[column.Field]
                    : null;
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

        private async Task RaiseOnBodyRowDoubleClick(MouseEventArgs e, object dataItem)
        {
            if (!OnBodyRowDoubleClick.HasDelegate)
                return;

            await OnBodyRowDoubleClick.InvokeAsync(new GridInputEventArgs<object>(this, e, dataItem));
        }

        private async Task RaiseOnBodyRowClick(MouseEventArgs e, object dataItem)
        {
            if (!OnBodyRowClick.HasDelegate)
                return;

            await OnBodyRowClick.InvokeAsync(new GridInputEventArgs<object>(this, e, dataItem));
        }

        private int ComputeTotalPages(int pageSize, int totalCount)
        {
            if (totalCount <= 0) return 0;
            if (pageSize <= 0) return totalCount;

            return (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
        }

        private string GetColumnValueText(CandyGridColumn col, object item)
        {
            var columnValue = GetColumnValue(col, item);
            var stringValue = columnValue as string;

            if (columnValue == null)
            {
                return col.EmptyDisplayString;
            }

            stringValue = columnValue?.ToString() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(col.FormatString))
            {
                try
                {
                    stringValue = string.Format(col.FormatString, columnValue);
                }
                catch
                {
                    // ignore
                }
            }

            return stringValue;
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
