﻿namespace Unosquare.Blazorific.Common
{
    /// <summary>
    /// Represents a Data Request from a Tubular Grid.
    /// 
    /// This model is how Tubular Grid sends data to any server.
    /// </summary>
    public class CandyGridDataRequest
    {
        /// <summary>
        /// Request's counter.
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        /// The free-text search.
        /// </summary>
        public CandyGridDataFilter Search { get; set; }

        /// <summary>
        /// Set how many records skip, for pagination.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Set how many records take, for pagination.
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// Defines the columns.
        /// </summary>
        public CandyGridDataColumn[] Columns { get; set; }

        /// <summary>
        /// Sent the minutes difference between UTC and local time.
        /// </summary>
        public int TimezoneOffset { get; set; }
    }
}
