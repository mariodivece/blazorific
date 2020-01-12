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
        private readonly List<CandyGridColumn> m_Columns = new List<CandyGridColumn>(32);
        private List<object> DataItems;
        private Type DataItemType;
        private readonly Timer QueueProcessor;

        public CandyGrid()
        {
            QueueProcessor = new Timer(async (s) =>
            {
                if (RequestedPageSize != PageSize ||
                    RequestedPageNumber != CurrentPage)
                {
                    await UpdateDataAsync();
                }

            }, null, Timeout.Infinite, Timeout.Infinite);
        }

        public int RequestedPageSize { get; set; } = 20;

        public int RequestedPageNumber { get; set; } = 1;

        [Parameter]
        public IGridDataAdapter DataAdapter { get; set; }

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

        public int StartRecordNumber => 1 + (PageSize * (CurrentPage - 1));

        public int EndRecordNumber => StartRecordNumber + (DataItems?.Count ?? 0) - 1;

        public IReadOnlyList<CandyGridColumn> Columns => m_Columns;

        public IEnumerable GetDataItems() => DataItems;

        public IEnumerable<T> GetDataItems<T>() => DataItems?.Cast<T>();

        public async Task UpdateDataAsync()
        {
            IsLoading = true;
            StatusText = "Loading data";
            try
            {
                if (DataAdapter == null)
                {
                    DataItemType = default;
                    DataItems = default;
                    CurrentPage = default;
                    FilteredRecordCount = default;
                    TotalRecordCount = default;
                    TotalPages = default;
                    StatusText = null;
                    return;
                }

                var response = await DataAdapter.RetrieveDataAsync(this);
                DataItemType = response.DataItemType;
                DataItems = new List<object>(response.DataItems.OfType<object>());
                CurrentPage = response.CurrentPage;
                FilteredRecordCount = response.FilteredRecordCount;
                TotalRecordCount = response.TotalRecordCount;
                TotalPages = response.TotalPages;
                StatusText = "Loaded grid data";
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
            }
            finally
            {
                IsLoading = false;
                PageSize = RequestedPageSize;
                CurrentPage = RequestedPageNumber;
                StateHasChanged();
            }
        }

        internal void AddColumn(CandyGridColumn column)
        {
            m_Columns.Add(column);
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender) return;
            await Js.InvokeVoidAsync($"{nameof(CandyGrid)}.initialize");
            await UpdateDataAsync();
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

        private async Task RequestPage(int pageNumber)
        {
            RequestedPageNumber = pageNumber;
            await UpdateDataAsync();
        }

        private async Task RaiseOnBodyRowDoubleClick(object dataItem)
        {
            if (!OnBodyRowDoubleClick.HasDelegate)
                return;

            await OnBodyRowDoubleClick.InvokeAsync(new GridBodyRowEventArgs(this as CandyGrid, dataItem));
        }
    }
}
