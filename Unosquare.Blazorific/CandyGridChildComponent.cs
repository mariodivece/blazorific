namespace Unosquare.Blazorific;

/// <summary>
/// Represents a component that is contained within a <see cref="CandyGrid"/>.
/// </summary>
/// <seealso cref="ComponentBase" />
public abstract class CandyGridChildComponent : ComponentBase
{
    /// <summary>
    /// Gets the parent.
    /// </summary>
    /// <value>
    /// The parent.
    /// </value>
    [CascadingParameter(Name = nameof(Parent))]
    protected CandyGrid? Parent { get; private set; }

    /// <summary>
    /// Gets the grid.
    /// </summary>
    /// <value>
    /// The grid.
    /// </value>
    public CandyGrid? Grid => Parent;
}
