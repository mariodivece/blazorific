namespace Unosquare.Blazorific.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class GridDataResponse
    {
        public IEnumerable DataItems { get; set; }

        public Type DataItemType { get; set; }

        /// <summary>
        /// Set how many records are in the entire set.
        /// </summary>
        public int TotalRecordCount { get; set; }

        /// <summary>
        /// Set how many records are in the filtered set.
        /// </summary>
        public int FilteredRecordCount { get; set; }

        /// <summary>
        /// Set how many pages are available.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Set which page is sent.
        /// </summary>
        public int CurrentPage { get; set; }
    }
}
