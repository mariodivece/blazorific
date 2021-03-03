namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System.Threading.Tasks;
    using Common;

    public partial class CandyFormGroup
    {
        protected ElementReference TooltipElement;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public string PrependIconClass { get; set; }

        [Parameter]
        public string AppendText { get; set; }

        [Parameter]
        public string HelpText { get; set; }

        [Parameter]
        public bool UseHorizontalLayout { get; set; }

        [Parameter]
        public bool UseHelpTooltip { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            await Js.BindTooltip(TooltipElement);
        }
    }
}