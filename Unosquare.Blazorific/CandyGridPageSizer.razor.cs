﻿namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using System.Collections.Generic;

    public partial class CandyGridPageSizer
    {
        [Parameter]
        public string Label { get; set; } = "Page Size:";

        [Parameter]
        public IDictionary<int, string> PageSizeOptions { get; set; } = new Dictionary<int, string>
        {
            { 10, "10" },
            { 20, "20" },
            { 50, "50" },
            { 100, "100" },
            { -1, "All" },
        };

        public int PageSize
        {
            get => PageSizeOptions.Keys.Contains(Parent.PageSize) ? Parent.PageSize : -1;
            set => Parent.ChangePageSize(value);
        }
    }
}
