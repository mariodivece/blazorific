namespace Unosquare.Blazorific
{
    using Common;
    using Microsoft.AspNetCore.Components;
    using System;

    public class CandyModalService
    {
        internal event EventHandler<ModalShowEventArgs> OnShowRequested;
        internal event EventHandler OnCloseRequested;

        public void ShowModalDialog(string title, Type componentType, Action<IComponent> onComponentAdded)
        {
            if (componentType.GetInterface(typeof(IComponent).FullName) == null)
                throw new ArgumentException($"{componentType.FullName} must implement {typeof(IComponent).FullName}");

            IComponent componentInstance = null;

            var content = new RenderFragment(builder =>
            {
                builder.OpenComponent(0, componentType);

                if (onComponentAdded != null)
                {
                    builder.AddComponentReferenceCapture(1, instance =>
                    {
                        componentInstance = instance as IComponent;
                        onComponentAdded?.Invoke(componentInstance);
                    });
                }

                builder.CloseComponent();
            });

            OnShowRequested?.Invoke(this, new ModalShowEventArgs(title, content));
        }

        public void ShowModalDialog(string title, Type componentType) => ShowModalDialog(title, componentType, null);

        public void ShowModalDialog<T>(string title, Action<T> onComponentAdded)
            where T : class, IComponent, new()
        {
            ShowModalDialog(title, typeof(T), (c) => { onComponentAdded?.Invoke(c as T); });
        }

        public void ShowModalDialog<T>(string title)
            where T : class, IComponent, new() => ShowModalDialog<T>(title, null);

        public void CloseModalDialog()
        {
            OnCloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
