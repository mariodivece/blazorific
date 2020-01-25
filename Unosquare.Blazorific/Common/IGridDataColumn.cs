﻿namespace Unosquare.Blazorific.Common
{
    public interface IGridDataColumn
    {
        string Name { get; }

        bool Sortable { get; }

        int SortOrder { get; }

        DataType DataType { get; }

        SortDirection SortDirection { get; }

        GridDataFilter Filter { get; }

        bool Searchable { get; }

        AggregationFunction Aggregate { get; }
    }
}
