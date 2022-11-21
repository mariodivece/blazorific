namespace Unosquare.Blazorific.Common;

/// <summary>
/// Provides extension methods for this library.
/// </summary>
public static partial class Extensions
{
    private const StringComparison PropertyNameComparer = StringComparison.InvariantCultureIgnoreCase;

    /// <summary>
    /// Determines whether the specified type is considered flat.
    /// Flat types are all value types and strings as they contain no structural depth.
    /// Nullable value types are also considered flat types.
    /// </summary>
    /// <param name="t">The type.</param>
    /// <returns>
    ///   <c>true</c> if the specified type is considered flat; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsFlatType(this ITypeInfo t) =>
        t.BackingType.IsValueType || t.NativeType == typeof(string);

    /// <summary>
    /// Determines whether [is flat type].
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns>
    ///   <c>true</c> if [is flat type] [the specified t]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsFlatType(this IPropertyProxy t) =>
        t.PropertyType.IsFlatType();

    /// <summary>
    /// Copies the flat (shallow) properties between objects of the same type.
    /// </summary>
    /// <typeparam name="T">The property type</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    public static void CopyFlatPropertiesTo<T>(this T source, T target)
        where T : class
    {
        var properties = typeof(T).Properties();
        foreach (var p in properties)
        {
            if (!p.IsFlatType())
                continue;

            try
            {
                if (p.TryRead(source, out var value))
                    p.TryWrite(target, value);
            }
            catch
            {
                // ignore
            }
        }
    }

    /// <summary>
    /// The json options
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Serializes the json.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    public static string SerializeJson<T>(this T item) =>
        JsonSerializer.Serialize<T>(item, JsonOptions);

    /// <summary>
    /// Converts to json content.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    public static HttpContent ToJsonContent<T>(this T item) =>
        new StringContent(item.SerializeJson(), Encoding.UTF8, "application/json");

    /// <summary>
    /// Deserializes the json.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json">The json.</param>
    /// <returns></returns>
    public static T? DeserializeJson<T>(this string json) =>
        JsonSerializer.Deserialize<T>(json, JsonOptions);

    /// <summary>
    /// Gets the grid data request columns.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <returns></returns>
    internal static IGridDataColumn[] GetGridDataRequestColumns(this CandyGrid grid)
    {
        if (grid is null || grid.CoercedDataAdapter is null)
            return Array.Empty<IGridDataColumn>();

        var properties = grid.CoercedDataAdapter.DataItemType.Properties().Where(t => t.IsFlatType());
        var result = new List<IGridDataColumn>(properties.Count());

        foreach (var property in properties)
        {
            var column = grid.Columns
                .FirstOrDefault(c => property.PropertyName.Equals(c.Field, PropertyNameComparer));

            result.Add(column as IGridDataColumn
                ?? new CandyGridColumn(property, null));
        }

        return result.ToArray();
    }

    /// <summary>
    /// Determines whether this instance is numeric.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns>
    ///   <c>true</c> if the specified t is numeric; otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsNumeric(this Type t) => t.TypeInfo().IsNumeric;

    /// <summary>
    /// Determines whether [is date time].
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns>
    ///   <c>true</c> if [is date time] [the specified t]; otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsDateTime(this Type t) => t.TypeInfo().BackingType.NativeType == typeof(DateTime);

    /// <summary>
    /// Determines whether this instance is boolean.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns>
    ///   <c>true</c> if the specified t is boolean; otherwise, <c>false</c>.
    /// </returns>
    internal static bool IsBoolean(this Type t) => t.TypeInfo().BackingType.NativeType == typeof(bool);

    /// <summary>
    /// Gets the type of the data.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns></returns>
    internal static DataType GetDataType(this Type t)
    {
        if (t.IsBoolean()) return DataType.Boolean;
        if (t.IsDateTime()) return DataType.DateTimeUtc;
        if (t.IsNumeric()) return DataType.Numeric;

        return DataType.String;
    }

    /// <summary>
    /// Computes the total pages.
    /// </summary>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="totalCount">The total count.</param>
    /// <returns></returns>
    internal static int ComputeTotalPages(int pageSize, int totalCount)
    {
        if (totalCount <= 0) return 0;
        if (pageSize <= 0) return 1;

        return (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
    }

    /// <summary>
    /// Generates the random HTML identifier.
    /// </summary>
    /// <returns></returns>
    internal static string GenerateRandomHtmlId() => $"guid_{Guid.NewGuid():N}";

    /// <summary>
    /// Gets the column value.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="dataItem">The data item.</param>
    /// <returns></returns>
    internal static object? GetColumnValue(this CandyGridColumn column, object dataItem)
    {
        if (column is null)
            return null;

        return !string.IsNullOrWhiteSpace(column.Field) &&
            column.Property is not null &&
            column.Property.TryRead(dataItem, out var value)
            ? value
            : null;
    }

    /// <summary>
    /// Gets the formatted value.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="dataItem">The data item.</param>
    /// <returns></returns>
    internal static string GetFormattedValue(this CandyGridColumn column, object? dataItem)
    {
        if (column is null)
            return string.Empty;

        if (dataItem is null)
            return column.EmptyDisplayString ?? string.Empty;

        var columnValue = column.GetColumnValue(dataItem);
        if (columnValue is null) return column.EmptyDisplayString ?? string.Empty;

        var stringValue = columnValue?.ToString() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(column.FormatString))
        {
            try { stringValue = string.Format(column.FormatString, columnValue); }
            catch { /* ignore */ }
        }

        return stringValue;
    }

    /// <summary>
    /// Logs the specified source.
    /// </summary>
    /// <param name="info">The information.</param>
    /// <param name="source">The source.</param>
    /// <param name="member">The member.</param>
    internal static void Log(this string info, string source, string member)
    {
        var sourceFormat = $"{source,16}";
        var memberFormat = $"{member,-22}";
        Console.WriteLine($"DBG {sourceFormat}.{memberFormat} | {info}");
    }

    /// <summary>
    /// Adds the attached component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="container">The container.</param>
    /// <param name="child">The child.</param>
    /// <returns></returns>
    internal static int AddAttachedComponent<T>(this IReadOnlyList<T> container, T child)
        where T : IAttachedComponent
    {
        if (child is null) return -1;

        if (container is IList<T> collection)
        {
            collection.Add(child);
            return collection.Count - 1;
        }

        return -1;
    }

    /// <summary>
    /// Removes the attached component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="container">The container.</param>
    /// <param name="child">The child.</param>
    internal static void RemoveAttachedComponent<T>(this IReadOnlyList<T> container, T child)
        where T : IAttachedComponent
    {
        if (container is not IList<T> collection)
            return;

        if (collection.Count == 0 || child is null)
            return;

        var childindex = child.Index > 0 && child.Index < collection.Count && ReferenceEquals(child, collection[child.Index])
            ? child.Index
            : collection.IndexOf(child);

        if (childindex < 0) return;
        collection.RemoveAt(childindex);
    }
}
