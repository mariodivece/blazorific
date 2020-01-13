﻿namespace Unosquare.Blazorific
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Web;
    using Unosquare.Blazorific.Common;

    public class CandyGridColumn : IComponent, IGridColumn
    {
        private bool HasInitialized;

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Field { get; set; }

        [Parameter]
        public bool IsSortable { get; set; }

        [Parameter]
        public bool IsSearchable { get; set; }

        [Parameter]
        public int SortOrder { get; set; }

        [Parameter]
        public SortDirection SortDirection { get; set; }

        [Parameter]
        public AggregationFunction Aggregate { get; set; }

        [Parameter]
        public RenderFragment<object> DataTemplate { get; set; }

        public GridDataFilter Filter { get; } = new GridDataFilter();

        [CascadingParameter(Name = nameof(Parent))]
        protected CandyGrid Parent { get; set; }

        internal IPropertyProxy Property { get; set; }

        string IGridColumn.Name => Field;

        string IGridColumn.Label => Title;

        bool IGridColumn.Sortable => IsSortable;

        bool IGridColumn.Searchable => IsSearchable;

        public void ChangeSortDirection()
        {
            var nextSortDirection = SortDirection == SortDirection.None
                ? SortDirection.Ascending
                : SortDirection == SortDirection.Ascending
                ? SortDirection.Descending
                : SortDirection.None;

            SortDirection = nextSortDirection;
            SortOrder = nextSortDirection == SortDirection.None ? 0 : 1;
            Parent.QueueDataUpdate();
        }

        void IComponent.Attach(RenderHandle renderHandle)
        {
            // placeholder
        }

        Task IComponent.SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (HasInitialized) return Task.CompletedTask;

            Parent.AddColumn(this);
            HasInitialized = true;

            return Task.CompletedTask;
        }
    }
}