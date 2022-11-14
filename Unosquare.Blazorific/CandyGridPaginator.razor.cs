﻿namespace Unosquare.Blazorific;

/// <summary>
/// Provides a pagination component for the <see cref="CandyGrid"/>.
/// </summary>
/// <seealso cref="CandyGridChildComponent" />
public partial class CandyGridPaginator
{
    private int m_ButtonCount = 5;

    /// <summary>
    /// Gets or sets a value indicating whether [show first button].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show first button]; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool ShowFirstButton { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether [show last button].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show last button]; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool ShowLastButton { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether [show previous button].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show previous button]; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool ShowPreviousButton { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether [show next button].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show next button]; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool ShowNextButton { get; set; } = true;

    /// <summary>
    /// Gets or sets the button count.
    /// </summary>
    /// <value>
    /// The button count.
    /// </value>
    [Parameter]
    public int ButtonCount
    {
        get => m_ButtonCount < 1 ? 1 : m_ButtonCount;
        set => m_ButtonCount = value;
    }

    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    /// <value>
    /// The page number.
    /// </value>
    public int PageNumber
    {
        get => Parent?.PageNumber ?? 0;
        set => Parent?.ChangePageNumber(value);
    }
}
