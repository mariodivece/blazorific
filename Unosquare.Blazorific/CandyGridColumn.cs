namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;

    public class CandyGridColumn : IComponent, IGridDataColumn
    {
        private static readonly int MinSortDirection = Enum.GetValues(typeof(SortDirection)).Cast<int>().Min();
        private static readonly int MaxSortDirection = Enum.GetValues(typeof(SortDirection)).Cast<int>().Max();

        private bool HasInitialized;

        public CandyGridColumn()
        {
            // placeholder  
        }

        internal CandyGridColumn(IPropertyProxy property)
        {
            // TODO: Create automatic columns from type.
            // Make it smarter
            Title = property.Name;
            Field = property.Name;
        }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Field { get; set; }

        [Parameter]
        public bool IsSortable { get; set; }

        [Parameter]
        public bool IsSearchable { get; set; }

        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public int SortOrder { get; set; }

        [Parameter]
        public SortDirection SortDirection { get; set; }

        [Parameter]
        public AggregationFunction Aggregate { get; set; }

        [Parameter]
        public RenderFragment<object> DataTemplate { get; set; }

        [Parameter]
        public RenderFragment<CandyGridColumn> HeaderTemplate { get; set; }

        [Parameter]
        public string FormatString { get; set; }

        [Parameter]
        public string EmptyDisplayString { get; set; } = string.Empty;

        [Parameter]
        public TextAlignment Alignment { get; set; } = TextAlignment.Auto;

        [Parameter]
        public string CssClass { get; set; } = "candygrid-cell clearfix";

        public GridDataFilter Filter { get; } = new GridDataFilter();

        [CascadingParameter(Name = nameof(Parent))]
        protected CandyGrid Parent { get; set; }

        string IGridDataColumn.Name => Field;

        bool IGridDataColumn.Sortable => IsSortable;

        bool IGridDataColumn.Searchable => IsSearchable;

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
            foreach (var column in Parent.Columns)
            {
                column.SortOrder = column.SortDirection != SortDirection.None
                    ? column.SortOrder
                    : 0;
            }

            if (multiColumnSorting)
            {
                SortOrder = SortOrder <= 0
                    ? Parent.Columns.Max(c => c.SortOrder) + 1
                    : SortOrder;
            }
            else
            {
                // Reset sort order and sort direction for all columns
                // except for this one
                foreach (var column in Parent.Columns)
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
            var sortedColumns = Parent.Columns.Where(c => c.SortOrder > 0).OrderBy(c => c.SortOrder);
            
            foreach (var column in sortedColumns)
                column.SortOrder = columnSortOrder++;

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