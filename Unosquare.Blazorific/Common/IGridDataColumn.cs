namespace Unosquare.Blazorific.Common
{
    public interface IGridDataColumn
    {
        /// <summary>
        /// Column Name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Set if column is sortable.
        /// </summary>
        bool Sortable { get; }

        /// <summary>
        /// Set the sort order, zero or less are ignored.
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// Set the sort direction.
        /// </summary>
        SortDirection SortDirection { get; }

        /// <summary>
        /// Set if the column is searchable in free-text search.
        /// </summary>
        bool Searchable { get; }

        /// <summary>
        /// Column data type.
        /// </summary>
        DataType DataType { get; }

        /// <summary>
        /// The Aggregation Function.
        /// </summary>
        AggregationFunction Aggregate { get; }

        /// <summary>
        /// Filter search text.
        /// </summary>
        public string FilterText { get; }

        /// <summary>
        /// Filter search params.
        /// </summary>
        public string[] FilterArgument { get; }

        /// <summary>
        /// Filter operator.
        /// </summary>
        public CompareOperators FilterOperator { get; }
    }
}
