namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using System.Threading.Tasks;
    using Unosquare.Blazorific.Common;

    public class CandyGridColumn : IComponent, IGridColumn
    {
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