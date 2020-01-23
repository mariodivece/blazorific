namespace Unosquare.Blazorific.Common
{
    using Microsoft.AspNetCore.Components;
    using System;

    public interface IAttachedComponent : IComponent, IDisposable
    {
        int Index { get; }
    }
}
