namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;

    public partial class CandyTab
    {
        [CascadingParameter(Name = nameof(TabSet))]
        protected CandyTabSet TabSet { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            TabSet.AddTab(this);
        }
    }
}
