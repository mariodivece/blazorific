namespace Unosquare.Blazorific.Common
{
    using Microsoft.AspNetCore.Components;
    using System;

    public sealed class ModalShowEventArgs : EventArgs
    {
        internal ModalShowEventArgs(string title, RenderFragment content)
        {
            Title = title;
            Content = content;
        }

        public string Title { get; }

        public RenderFragment Content { get; }
    }
}
