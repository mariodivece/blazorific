namespace Unosquare.Blazorific.Common;

/// <summary>
/// Defines a basic class for Tab Set event arguments.
/// </summary>
/// <seealso cref="EventArgs" />
public class TabSetEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TabSetEventArgs"/> class.
    /// </summary>
    /// <param name="tabSet">The tab set.</param>
    public TabSetEventArgs(CandyTabSet? tabSet)
    {
        TabSet = tabSet;
    }

    /// <summary>
    /// Gets the tab set.
    /// </summary>
    public CandyTabSet? TabSet { get; }
}

/// <summary>
/// Defines a basic class for Tab Set Tab event arguments.
/// </summary>
/// <seealso cref="EventArgs" />
public class TabSetTabEventArgs : TabSetEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TabSetTabEventArgs"/> class.
    /// </summary>
    /// <param name="tabSet">The tab set.</param>
    /// <param name="tab">The tab.</param>
    public TabSetTabEventArgs(CandyTabSet? tabSet, CandyTab? tab)
        : base(tabSet)
    {
        Tab = tab;
    }

    /// <summary>
    /// Gets the tab.
    /// </summary>
    public CandyTab? Tab { get; }
}