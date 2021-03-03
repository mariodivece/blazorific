namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;
    using System.Threading.Tasks;
    using Common;

    public partial class CandyModal
    {
        protected ElementReference ModalElement { get; set; }

        public enum Sizes
        {
            Default,
            Small,
            Large,
            ExtraLarge,
        }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment Footer { get; set; }

        [Parameter]
        public Sizes Size { get; set; }

        [Parameter]
        public bool Center { get; set; }

        private string OptionsClasses
        {
            get
            {
                var sizeClass = Size switch
                {
                    Sizes.Default => string.Empty,
                    Sizes.ExtraLarge => "modal-xl",
                    Sizes.Large => "modal-lg",
                    Sizes.Small => "modal-sam",
                    _ => string.Empty
                };

                return $"{(Center ? "modal-dialog-centered" : string.Empty)} {sizeClass}".Trim();
            }
        }

        public async Task Show() => await Show(null);

        public async Task Show(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;

            await Js.ModalShow(ModalElement);
            StateHasChanged();
        }

        public async Task Hide()
        {
            await Js.ModalHide(ModalElement);
        }
    }
}
