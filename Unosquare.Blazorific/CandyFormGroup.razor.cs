namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;

    public partial class CandyFormGroup
    {
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
    }
}