namespace Unosquare.Blazorific.Common
{
    using System.Collections.Generic;

    public class GridState
    {
        public string SearchText { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public IReadOnlyCollection<GridColumnState> Columns { get; set; }
    }

    public class GridColumnState : IGridDataColumn
    {
        public GridColumnState()
        {

        }

        internal GridColumnState(IGridDataColumn other)
        {
            Name = other.Name;
            SortDirection = other.SortDirection;
            SortOrder = other.SortOrder;
            Filter = other.Filter;
            Sortable = other.Sortable;
            DataType = other.DataType;
            Searchable = other.Searchable;
            Aggregate = other.Aggregate;
        }

        public string Name { get; set; }

        public SortDirection SortDirection { get; set; }

        public int SortOrder { get; set; }

        public GridDataFilter Filter { get; set; }

        public bool Sortable { get; set; }

        public DataType DataType { get; set; }

        public bool Searchable { get; set; }

        public AggregationFunction Aggregate { get; set; }
    }
}
