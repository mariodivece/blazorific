namespace Unosquare.Blazorific.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a Data Request from a Tubular Grid.
    /// This model is how Tubular Grid sends data to any server.
    /// </summary>
    public sealed class GridDataRequest
    {
        private readonly CandyGrid Grid;
        private int m_PageSize = 20;
        private int m_PageNumber = 1;

        internal GridDataRequest(CandyGrid grid)
        {
            Grid = grid;
        }

        /// <summary>
        /// Request's counter.
        /// </summary>
        public int Counter { get; internal set; } = 1;

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
        public int Skip => PageSize <= 0 ? 0 : Math.Max(0, PageNumber - 1) * PageSize;

        /// <summary>
        /// Set how many records take, for pagination.
        /// </summary>
        public int Take => PageSize;

        /// <summary>
        /// Defines the columns.
        /// </summary>
        public IReadOnlyList<IGridDataColumn> Columns =>
            Grid.GetGridDataRequestColumns();

        /// <summary>
        /// Sent the minutes difference between UTC and local time.
        /// TODO: Blazor right now returns Now and UTcNow as the same date. They are both UTC.
        /// </summary>
        public int TimezoneOffset =>
            (int)Math.Round(DateTime.UtcNow.Subtract(DateTime.Now).TotalMinutes, 0);

        [JsonIgnore]
        public int PageSize
        {
            get => m_PageSize > 0 ? m_PageSize : -1;
            set => m_PageSize = value;
        }

        [JsonIgnore]
        public int PageNumber
        {
            get
            {
                var maxPageNumber = PageSize > 0
                    ? Extensions.ComputeTotalPages(PageSize, Grid.FilteredRecordCount)
                    : 1;

                return m_PageNumber < 1
                    ? 1
                    : m_PageNumber > maxPageNumber
                    ? maxPageNumber
                    : m_PageNumber;
            }
            set
            {
                m_PageNumber = value;
            }
        }
    }
}
