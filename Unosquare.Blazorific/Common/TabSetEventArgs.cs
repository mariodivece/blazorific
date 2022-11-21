namespace Unosquare.Blazorific.Common;

/// <summary>
/// Defines a basic class for Tab Set Tab event arguments.
/// </summary>
/// <seealso cref="EventArgs" />
public class TabSetTabEventArgs : CandyEventArgs<CandyTabSet>
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