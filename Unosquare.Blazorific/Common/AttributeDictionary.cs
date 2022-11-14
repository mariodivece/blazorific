namespace Unosquare.Blazorific.Common;

/// <summary>
/// Represents a set of attributes 
/// </summary>
public sealed class AttributeDictionary
{
    private readonly Action ChangeCallback;
    private readonly Dictionary<string, object?> Attributes = new(16, StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeDictionary"/> class.
    /// </summary>
    /// <param name="changeCallback">The change callback.</param>
    internal AttributeDictionary(Action changeCallback)
    {
        ChangeCallback = changeCallback;
    }

    /// <summary>
    /// Gets or sets a value with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public object? this[string name]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(name)) return default;
            return Attributes.TryGetValue(name, out var value) ? value : null;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            Attributes[name] = value;
            ChangeCallback?.Invoke();
        }
    }

    /// <summary>
    /// Gets or sets the CSS class.
    /// </summary>
    /// <value>
    /// The CSS class.
    /// </value>
    public string? CssClass
    {
        get => this["class"] as string;
        set => this["class"] = value;
    }

    /// <summary>
    /// Gets or sets the style.
    /// </summary>
    /// <value>
    /// The style.
    /// </value>
    public string? Style
    {
        get => this["style"] as string;
        set => this["style"] = value;
    }

    /// <summary>
    /// Gets the attribute names.
    /// </summary>
    /// <value>
    /// The names.
    /// </value>
    public IEnumerable<string> Names => Attributes.Keys;

    /// <summary>
    /// Gets the attibute values.
    /// </summary>
    /// <value>
    /// The values.
    /// </value>
    public IEnumerable<object?> Values => Attributes.Values;

    /// <summary>
    /// Gets the dictionary.
    /// </summary>
    /// <value>
    /// The dictionary.
    /// </value>
    public IReadOnlyDictionary<string, object?> Dictionary => Attributes;

    /// <summary>
    /// Removes an attribute with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    public void Remove(string name)
    {
        if (Attributes.ContainsKey(name))
        {
            Attributes.Remove(name);
            ChangeCallback?.Invoke();
        }
    }

    /// <summary>
    /// Clears all the attributes.
    /// </summary>
    public void Clear()
    {
        Attributes.Clear();
        ChangeCallback?.Invoke();
    }

    /// <summary>
    /// Converts to dictionary.
    /// </summary>
    /// <value>
    /// To dictionary.
    /// </value>
    public IDictionary<string, object?> ToDictionary => new Dictionary<string, object?>(Attributes);

    /// <summary>
    /// Replaces the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    public void Replace(IDictionary<string, object?> other)
    {
        Attributes.Clear();
        Merge(other);
    }

    /// <summary>
    /// Merges the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    public void Merge(IDictionary<string, object?> other)
    {
        if (other is null)
            return;

        foreach (var kvp in other)
            Attributes[kvp.Key] = kvp.Value;

        ChangeCallback?.Invoke();
    }
}
