namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using System;

    public partial class CandyModal
    {
        [Inject]
        protected CandyModalService ModalService { get; set; }

        protected bool IsVisible { get; set; }

        protected string Title { get; set; }

        protected RenderFragment Content { get; set; }

        protected override void OnInitialized()
        {
            ModalService.OnShowRequested += OnShowModalRequested;
            ModalService.OnCloseRequested += OnCloseModalRequested;
            base.OnInitialized();
        }

        protected void OnShowModalRequested(object sender, ModalShowEventArgs e)
        {
            Title = e.Title;
            Content = e.Content;
            IsVisible = true;

            StateHasChanged();
        }

        protected void OnCloseModalRequested(object sender, EventArgs e)
        {
            IsVisible = false;
            Title = null;
            Content = null;

            StateHasChanged();
        }

        public virtual void Dispose()
        {
            ModalService.OnShowRequested -= OnShowModalRequested;
            ModalService.OnCloseRequested -= OnCloseModalRequested;
        }
    }
}
