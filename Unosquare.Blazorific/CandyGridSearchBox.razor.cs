namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using System.Linq;

    public partial class CandyGridSearchBox
    {
        [Parameter]
        public string Placeholder { get; set; } = "search . . .";

        public bool IsVisible => Parent?.Columns.Any(c => c.IsSearchable) ?? false;

        private void OnSearchInput(ChangeEventArgs e)
        {
            var searchInput = (e.Value as string ?? string.Empty).Trim();
            Parent.ChangeSearchText(searchInput);
        }
    }
}
