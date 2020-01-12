namespace Unosquare.Blazorific.Common
{
    public class GridDataColumn
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public bool Sortable { get; set; }

        public int SortOrder { get; set; }

        public SortDirection SortDirection { get; set; }

        public GridDataFilter Filter { get; set; }

        public bool Searchable { get; set; }

        public GridColumnDataType DataType { get; set; }

        public AggregationFunction Aggregate { get; set; }
    }
}
