﻿namespace Unosquare.Blazorific.Common
{
    /// <summary>
    /// Enumeration Sort directions.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Not sorting
        /// </summary>
        None,

        /// <summary>
        /// Ascending sorting
        /// </summary>
        Ascending,

        /// <summary>
        /// Descending sorting
        /// </summary>
        Descending,
    }

    /// <summary>
    /// Enumeration filtering operators.
    /// </summary>
    public enum CompareOperators
    {
        /// <summary>
        /// None operator
        /// </summary>
        None,

        /// <summary>
        /// Auto-filtering
        /// </summary>
        Auto,

        /// <summary>
        /// Equals operator
        /// </summary>
        Equals,

        /// <summary>
        /// Not Equals operator
        /// </summary>
        NotEquals,

        /// <summary>
        /// Contains filter
        /// </summary>
        Contains,

        /// <summary>
        /// StartsWith filter
        /// </summary>
        StartsWith,

        /// <summary>
        /// EndsWith filter
        /// </summary>
        EndsWith,

        /// <summary>
        /// Greater than or equal filter
        /// </summary>
        Gte,

        /// <summary>
        /// Greater than filter
        /// </summary>
        Gt,

        /// <summary>
        /// Less than or equal filter
        /// </summary>
        Lte,

        /// <summary>
        /// Less than filter
        /// </summary>
        Lt,

        /// <summary>
        /// Multiple options filter
        /// </summary>
        Multiple,

        /// <summary>
        /// Between values filter
        /// </summary>
        Between,

        /// <summary>
        /// Not Contains filter
        /// </summary>
        NotContains,

        /// <summary>
        /// Not StartsWith filter
        /// </summary>
        NotStartsWith,

        /// <summary>
        /// Not EndsWith filter
        /// </summary>
        NotEndsWith,
    }

    /// <summary>
    /// Aggregation Functions.
    /// </summary>
    public enum AggregationFunction
    {
        /// <summary>
        /// None function
        /// </summary>
        None,

        /// <summary>
        /// Sum function
        /// </summary>
        Sum,

        /// <summary>
        /// Average function
        /// </summary>
        Average,

        /// <summary>
        /// Count function
        /// </summary>
        Count,

        /// <summary>
        /// Distinct Count function
        /// </summary>
        DistinctCount,

        /// <summary>
        /// Max function
        /// </summary>
        Max,

        /// <summary>
        /// Min function
        /// </summary>
        Min,
    }

    /// <summary>
    /// Text alignment options
    /// </summary>
    public enum TextAlignment
    {
        Auto,
        Left,
        Right,
        Center,
    }
}
