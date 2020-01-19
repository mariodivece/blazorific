namespace Unosquare.Blazorific.Common
{
    public interface IGridColumn
    {
        string Name { get; }

        bool Sortable { get; }

        int SortOrder { get; }

        SortDirection SortDirection { get; }

        GridDataFilter Filter { get; }

        bool Searchable { get; }

        AggregationFunction Aggregate { get; }
    }
}
