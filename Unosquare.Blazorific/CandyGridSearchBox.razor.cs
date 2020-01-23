namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using System.Linq;

    public partial class CandyGridSearchBox
    {
        [Parameter]
        public string Placeholder { get; set; } = "search . . .";

        public bool IsVisible { get; protected set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            IsVisible = Parent.Columns.Any(c => c.IsSearchable);
        }

        private void OnSearchInput(ChangeEventArgs e)
        {
            var searchInput = (e.Value as string ?? string.Empty).Trim();
            Parent.ChangeSearchText(searchInput);
        }
    }
}
