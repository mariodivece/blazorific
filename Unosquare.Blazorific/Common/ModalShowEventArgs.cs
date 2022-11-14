namespace Unosquare.Blazorific.Common;

/// <summary>
/// Defines arguments for the show event of modal dialogs.
/// </summary>
/// <seealso cref="EventArgs" />
public sealed class ModalShowEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModalShowEventArgs"/> class.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="content">The content.</param>
    internal ModalShowEventArgs(string? title, RenderFragment? content)
    {
        Title = title;
        Content = content;
    }

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string? Title { get; }

    /// <summary>
    /// Gets the content.
    /// </summary>
    /// <value>
    /// The content.
    /// </value>
    public RenderFragment? Content { get; }
}
