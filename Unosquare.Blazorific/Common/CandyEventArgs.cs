namespace Unosquare.Blazorific.Common;

/// <summary>
/// A basic class that represents <see cref="EventArgs"/> for a given <see cref="CandyComponentBase"/>.
/// </summary>
/// <typeparam name="T">The type of <see cref="CandyComponentBase"/>.</typeparam>
/// <seealso cref="EventArgs" />
public class CandyEventArgs<T> : EventArgs
    where T : CandyComponentBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CandyEventArgs{T}"/> class.
    /// </summary>
    /// <param name="component">The component.</param>
    public CandyEventArgs(T? component)
    {
        Component = component;
    }

    /// <summary>
    /// Gets or sets the component.
    /// </summary>
    public T? Component { get; }
}
