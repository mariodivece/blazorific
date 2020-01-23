namespace Unosquare.Blazorific
{
    using Microsoft.AspNetCore.Components;

    public abstract class CandyGridChildComponent : ComponentBase
    {
        [CascadingParameter(Name = nameof(Parent))]
        protected CandyGrid Parent { get; private set; }

        public CandyGrid Grid => Parent;
    }
}
