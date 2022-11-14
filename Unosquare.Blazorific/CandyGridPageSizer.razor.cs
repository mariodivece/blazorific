namespace Unosquare.Blazorific;

/// <summary>
/// Represents a dropdown that allos the user to select a grid page size.
/// </summary>
/// <seealso cref="Unosquare.Blazorific.CandyGridChildComponent" />
public partial class CandyGridPageSizer
{
    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    /// <value>
    /// The label.
    /// </value>
    [Parameter]
    public string? Label { get; set; } = "Page Size:";

    /// <summary>
    /// Gets or sets the page size options.
    /// </summary>
    /// <value>
    /// The page size options.
    /// </value>
    [Parameter]
    public IDictionary<int, string> PageSizeOptions { get; set; } = new Dictionary<int, string>
    {
        { 10, "10" },
        { 20, "20" },
        { 50, "50" },
        { 100, "100" },
        { -1, "All" },
    };

    /// <summary>
    /// Gets or sets the size of the page.
    /// </summary>
    /// <value>
    /// The size of the page.
    /// </value>
    public int PageSize
    {
        get => PageSizeOptions.ContainsKey(Parent?.PageSize ?? -1) ? Parent?.PageSize ?? -1 : -1;
        set => Parent?.ChangePageSize(value);
    }
}
