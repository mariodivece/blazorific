namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;

    public partial class CandyGrid
    {
        private readonly List<CandyGridColumn> m_Columns = new List<CandyGridColumn>(32);
        private IEnumerable<object> m_Data;
        private Type DataItemType;

        public CandyGrid()
        {
            // placeholder
        }

        [Parameter]
        public EventCallback<GridBodyRowEventArgs> OnBodyRowDoubleClick { get; set; }

        [Parameter]
        public IEnumerable Data
        {
            get { return m_Data; }
            set { m_Data = value?.Cast<object>(); }
        }

        [Parameter]
        public RenderFragment CandyGridColumns { get; set; }

        internal bool IsEmpty => m_Data == null || m_Data.Count() <= 0;

        private IReadOnlyList<CandyGridColumn> Columns => m_Columns;

        internal void AddColumn(CandyGridColumn column)
        {
            m_Columns.Add(column);
            StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (!firstRender) return;
            Js.InvokeVoidAsync($"{nameof(CandyGrid)}.initialize");
        }

        private object GetColumnValue(CandyGridColumn column, object dataItem)
        {
            if (string.IsNullOrWhiteSpace(column.Field))
                return null;

            if (dataItem == null)
                return null;

            if (column.Property == null)
                UpdatePropertyProxies(dataItem);

            if (column.Property == null)
                return null;

            return column.Property.GetValue(dataItem);
        }

        private void SetColumnValue(CandyGridColumn column, object dataItem, object value)
        {
            if (string.IsNullOrWhiteSpace(column.Field))
                return;

            if (dataItem == null)
                return;

            if (column.Property == null)
                UpdatePropertyProxies(dataItem);

            if (column.Property == null)
                return;

            column.Property.SetValue(dataItem, value);
        }

        private void UpdatePropertyProxies(object dataItem)
        {
            if (dataItem == null)
                return;

            var itemType = dataItem.GetType();
            if (itemType == DataItemType)
                return;

            DataItemType = itemType;
            var properties = dataItem.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary((k) => k.Name, (v) => v.CreatePropertyProxy());

            foreach (var col in Columns)
            {
                if (string.IsNullOrWhiteSpace(col.Field))
                    continue;

                col.Property = properties.ContainsKey(col.Field) ? properties[col.Field] : null;
            }
        }

        private async Task RaiseOnBodyRowDoubleClick(object dataItem)
        {
            if (!OnBodyRowDoubleClick.HasDelegate)
                return;

            await OnBodyRowDoubleClick.InvokeAsync(new GridBodyRowEventArgs(this as CandyGrid, dataItem));
        }
    }
}
