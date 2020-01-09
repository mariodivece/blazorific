namespace Unosquare.Blazorific
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Unosquare.Blazorific.Common;

    public class CandyGridColumn : IComponent
    {
        private bool HasInitialized;

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Field { get; set; }

        [Parameter]
        public RenderFragment<object> DataTemplate { get; set; }

        [CascadingParameter(Name = nameof(Parent))]
        protected CandyGrid Parent { get; set; }

        internal IPropertyProxy Property { get; set; }

        void IComponent.Attach(RenderHandle renderHandle)
        {
            // placeholder
        }

        Task IComponent.SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (HasInitialized) return Task.CompletedTask;

            Parent.AddColumn(this);
            HasInitialized = true;

            return Task.CompletedTask;
        }
    }
}