namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System.Threading.Tasks;

    public partial class CandyTab
    {
        protected ElementReference HeaderElement { get; set; }

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
            if (string.IsNullOrWhiteSpace(Id))
                Id = Extensions.GenerateRandomHtmlId();

            TabSet.AddTab(this);
        }

        public async Task Show()
        {
            await Js.TabShow(HeaderElement);
        }
    }
}
