namespace Unosquare.Blazorific.Common;


/// <summary>
/// Marks a component as attached to another one providing a component index.
/// </summary>
/// <seealso cref="IComponent" />
/// <seealso cref="IDisposable" />
public interface IAttachedComponent : IComponent, IDisposable
{
    /// <summary>
    /// Gets the index of the attached component.
    /// </summary>
    int Index { get; }
}
