namespace Unosquare.Blazorific;

/// <summary>
/// Represents modal dialog component.
/// </summary>
/// <seealso cref="CandyComponentBase" />
public partial class CandyModal
{
    /// <summary>
    /// Modal dialog sizes.
    /// </summary>
    public enum Sizes
    {
        /// <summary>
        /// The default
        /// </summary>
        Default,

        /// <summary>
        /// The small
        /// </summary>
        Small,

        /// <summary>
        /// The large
        /// </summary>
        Large,

        /// <summary>
        /// The extra large
        /// </summary>
        ExtraLarge,
    }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the child.
    /// </summary>
    /// <value>
    /// The content of the child.
    /// </value>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the footer.
    /// </summary>
    /// <value>
    /// The footer.
    /// </value>
    [Parameter]
    public RenderFragment? Footer { get; set; }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    [Parameter]
    public Sizes Size { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="CandyModal"/> is center.
    /// </summary>
    /// <value>
    ///   <c>true</c> if center; otherwise, <c>false</c>.
    /// </value>
    [Parameter]
    public bool Center { get; set; }

    /// <summary>
    /// Gets or sets the on shown action.
    /// </summary>
    [Parameter]
    public Action<CandyEventArgs<CandyModal>>? OnShown { get; set; }

    /// <summary>
    /// Gets or sets the on hidden action.
    /// </summary>
    [Parameter]
    public Action<CandyEventArgs<CandyModal>>? OnHidden { get; set; }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
            return;

        if (Js is not null)
            await Js.InvokeVoidAsync($"{nameof(CandyModal)}.bindEvents", Element, JsElement);
    }

    private string OptionsClasses
    {
        get
        {
            var sizeClass = Size switch
            {
                Sizes.Default => string.Empty,
                Sizes.ExtraLarge => "modal-xl",
                Sizes.Large => "modal-lg",
                Sizes.Small => "modal-sam",
                _ => string.Empty
            };

            return $"{(Center ? "modal-dialog-centered" : string.Empty)} {sizeClass}".Trim();
        }
    }

    /// <summary>
    /// Shows this instance.
    /// </summary>
    public async Task Show() => await Show(null);

    /// <summary>
    /// Shows the specified title.
    /// </summary>
    /// <param name="title">The title.</param>
    public async Task Show(string? title)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title = title;

        if (Js is not null)
            await Js.ModalShow(Element);

        StateHasChanged();
    }

    /// <summary>
    /// Hides this instance.
    /// </summary>
    public async Task Hide()
    {
        if (Js is not null)
            await Js.ModalHide(Element);
    }

    /// <summary>
    /// Called from Javascript when the event occurs.
    /// </summary>
    [JSInvokable]
    public void JsHandleShownEvent()
    {
        OnShown?.Invoke(new(this));
    }

    /// <summary>
    /// Called from Javascript when the event occurs.
    /// </summary>
    [JSInvokable]
    public void JsHandleHiddenEvent()
    {
        OnHidden?.Invoke(new(this));
    }
}
