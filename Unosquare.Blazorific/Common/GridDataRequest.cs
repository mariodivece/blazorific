namespace Unosquare.Blazorific.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a Data Request from a Tubular Grid.
    /// This model is how Tubular Grid sends data to any server.
    /// </summary>
    public sealed class GridDataRequest
    {
        /// <summary>
        /// Request's counter.
        /// </summary>
        public int Counter { get; private set; } = 1;

        /// <summary>
        /// The free-text search.
        /// </summary>
        public GridDataFilter Search { get; } = new GridDataFilter
        {
            Text = string.Empty,
            Operator = CompareOperators.None
        };

        /// <summary>
        /// Set how many records skip, for pagination.
        /// </summary>
        public int Skip { get; private set; }

        /// <summary>
        /// Set how many records take, for pagination.
        /// </summary>
        public int Take { get; private set; }

        /// <summary>
        /// Defines the columns.
        /// </summary>
        public IReadOnlyCollection<IGridDataColumn> Columns { get; private set; }

        /// <summary>
        /// Sent the minutes difference between UTC and local time.
        /// TODO: Blazor right now returns Now and UTcNow as the same date. They are both UTC.
        /// </summary>
        public int TimezoneOffset =>
            (int)Math.Round(DateTime.UtcNow.Subtract(DateTime.Now).TotalMinutes, 0);

        internal void UpdateFrom(CandyGrid grid)
        {
            Counter += 1;
            Columns = grid.GetGridDataRequestColumns();
            Skip = grid.PageSize <= 0 ? 0 : Math.Max(0, grid.PageNumber - 1) * grid.PageSize;
            Take = grid.PageSize < 0 ? -1 : grid.PageSize;
            Search.Text = grid.SearchText ?? string.Empty;
            Search.Operator = string.IsNullOrWhiteSpace(Search.Text)
                ? CompareOperators.None
                : CompareOperators.Auto;
        }
    }
}
