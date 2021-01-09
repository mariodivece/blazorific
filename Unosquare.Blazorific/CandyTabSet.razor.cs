namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using System.Collections.Generic;

    public partial class CandyTabSet
    {
        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public CandyTabMode Mode { get; set; }

        public IReadOnlyList<CandyTab> Tabs { get; } = new List<CandyTab>();

        public void AddTab(CandyTab tab)
        {
            (Tabs as List<CandyTab>).Add(tab);
            StateHasChanged();
        }

        public int IndexOf(CandyTab tab)
        {
            var tabs = Tabs as List<CandyTab>;
            return tabs.IndexOf(tab);
        }

        protected override void OnInitialized()
        {
            if (string.IsNullOrWhiteSpace(Id))
                Id = Extensions.GenerateRandomHtmlId();

            base.OnInitialized();
        }
    }
}
