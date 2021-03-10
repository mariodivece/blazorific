namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using System.Linq;
    using System.Threading;

    public partial class CandyGridSearchBox
    {
        private readonly Timer DebounceTimer;

        public CandyGridSearchBox()
        {
            DebounceTimer = new Timer((s) =>
            {
                Parent.ChangeSearchText(SearchText);
            }, null, Timeout.Infinite, Timeout.Infinite);
        }

        [Parameter]
        public string Placeholder { get; set; } = "search . . .";

        public bool IsVisible => Parent?.Columns.Any(c => c.IsSearchable) ?? false;

        public string SearchText { get; set; }

        private void OnSearchInput(ChangeEventArgs e)
        {
            SearchText = (e.Value as string ?? string.Empty).Trim();
            DebounceTimer.Change(250, Timeout.Infinite);
        }
    }
}
