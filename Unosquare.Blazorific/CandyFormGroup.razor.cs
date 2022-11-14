namespace Unosquare.Blazorific;

/// <summary>
/// Defines a group of elements that make up a form field.
/// </summary>
/// <seealso cref="CandyComponentBase" />
public partial class CandyFormGroup
{
    /// <summary>
    /// Predefined input group sizes.
    /// </summary>
    public enum Sizes
    {
        /// <summary>
        /// The small size.
        /// </summary>
        Small,

        /// <summary>
        /// The normal size.
        /// </summary>
        Normal,

        /// <summary>
        /// The large size.
        /// </summary>
        Large,
    }

    /// <summary>
    /// The render layouts.
    /// </summary>
    public enum Layouts
    {
        /// <summary>
        /// The vertical
        /// </summary>
        Vertical,

        /// <summary>
        /// The horizontal
        /// </summary>
        Horizontal,

        /// <summary>
        /// The floating
        /// </summary>
        Floating
    }

    /// <summary>
    /// The tooltip element when tooltips are enabled.
    /// </summary>
    protected ElementReference TooltipElement;

    /// <summary>
    /// Gets or sets the content of the child.
    /// </summary>
    /// <value>
    /// The content of the child.
    /// </value>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    [Parameter]
    public Sizes Size { get; set; } = Sizes.Normal;

    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    /// <value>
    /// The label.
    /// </value>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the prepend icon class.
    /// </summary>
    /// <value>
    /// The prepend icon class.
    /// </value>
    [Parameter]
    public string? PrependIconClass { get; set; }

    /// <summary>
    /// Gets or sets the append text.
    /// </summary>
    /// <value>
    /// The append text.
    /// </value>
    [Parameter]
    public string? AppendText { get; set; }

    /// <summary>
    /// Gets or sets the help text.
    /// </summary>
    /// <value>
    /// The help text.
    /// </value>
    [Parameter]
    public string? HelpText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [use horizontal layout].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [use horizontal layout]; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public Layouts Layout { get; set; } = Layouts.Vertical;

    /// <summary>
    /// Gets or sets a value indicating whether [use help tooltip].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [use help tooltip]; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool UseHelpTooltip { get; set; }

    private string SizeCssClass => Layout is Layouts.Floating
        ? string.Empty
        : Size switch
        {
            Sizes.Normal => string.Empty,
            Sizes.Small => "input-group-sm",
            Sizes.Large => "input-group-lg",
            _ => string.Empty
        };

    /// <inheridoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        if (Js is null)
            return;

        await Js.BindTooltipAsync(TooltipElement!);
    }
}