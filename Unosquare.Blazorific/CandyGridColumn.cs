namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;

    public class CandyGridColumn : IComponent, IGridDataColumn
    {
        private static readonly int MinSortDirection = Enum.GetValues(typeof(SortDirection)).Cast<int>().Min();
        private static readonly int MaxSortDirection = Enum.GetValues(typeof(SortDirection)).Cast<int>().Max();

        private readonly IPropertyProxy m_Property;
        private bool HasInitialized;

        public CandyGridColumn()
        {
            // placeholder
        }

        internal CandyGridColumn(IPropertyProxy property)
        {
            // TODO: Create automatic columns from type.
            // Make it smarter
            m_Property = property;
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
        public DataType? FilterDataType { get; set; }

        [Parameter]
        public bool ShowFilter { get; set; } = true;

        [Parameter]
        public string CheckedProperty { get; set; }

        [Parameter]
        public string HeaderCssClass { get; set; }

        [Parameter]
        public string CellCssClass { get; set; }

        [Parameter]
        public int Width { get; set; }

        [Parameter]
        public Action<GridRowMouseEventArgs> OnDeleteButtonClick { get; set; }

        [Parameter]
        public Action<GridRowMouseEventArgs> OnDetailsButtonClick { get; set; }

        [Parameter]
        public Action<GridRowMouseEventArgs> OnEditButtonClick { get; set; }

        [Parameter]
        public Action<GridCellCheckboxEventArgs> OnCellCheckedChanged { get; set; }

        [CascadingParameter(Name = nameof(Parent))]
        protected CandyGrid Parent { get; set; }

        public IPropertyProxy Property => m_Property
            ?? Parent?.DataAdapter?.DataItemType.PropertyProxy(Field ?? string.Empty);

        public GridDataFilter Filter { get; } = new GridDataFilter();

        string IGridDataColumn.Name => Field;

        bool IGridDataColumn.Sortable => IsSortable;

        bool IGridDataColumn.Searchable => IsSearchable;

        DataType IGridDataColumn.DataType => FilterDataType
            ?? (Property?.PropertyType ?? typeof(string)).GetDataType();

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

        public void ApplyFilter(CompareOperators filter, params string[] args)
        {
            if (filter == CompareOperators.Auto || filter == CompareOperators.None || args == null || args.Length <= 0)
            {
                Filter.Argument = null;
                Filter.Text = null;
                Filter.Operator = CompareOperators.None;
                Parent?.QueueDataUpdate();
                return;
            }

            Filter.Operator = filter;
            Filter.Text = args[0];
            var arguments = new List<string>(args.Length - 1);
            for (var i = 1; i < args.Length; i++)
                arguments.Add(args[i]);

            Filter.Argument = arguments.ToArray();
            Parent?.QueueDataUpdate();
        }

        void IComponent.Attach(RenderHandle renderHandle)
        {
            // placeholder
        }

        Task IComponent.SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (HasInitialized)
                return Task.CompletedTask;

            Parent.AddColumn(this);
            HasInitialized = true;

            return Task.CompletedTask;
        }
    }
}