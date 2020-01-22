namespace Unosquare.Blazorific.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Extensions
    {
        internal static IGridDataColumn[] GetGridDataRequestColumns(this CandyGrid grid)
        {
            var comparison = StringComparison.InvariantCultureIgnoreCase;
            var properties = grid.DataAdapter.DataItemType.PropertyProxies().Values;
            var result = new List<IGridDataColumn>(properties.Count);

            foreach (var property in properties)
            {
                var column = grid.Columns
                    .FirstOrDefault(c => property.Name.Equals(c.Field, comparison));

                result.Add(column as IGridDataColumn ?? new GridDataColumn
                {
                    Name = property.Name,
                });
            }

            return result.ToArray();
        }

        internal static bool IsNumeric(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        internal static object GetDefault(this Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;

        internal static int ComputeTotalPages(int pageSize, int totalCount)
        {
            if (totalCount <= 0) return 0;
            if (pageSize <= 0) return 1;

            return (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
        }

        internal static object GetColumnValue(this CandyGridColumn column, object dataItem)
        {
            if (column == null)
                return null;

            return !string.IsNullOrWhiteSpace(column.Field)
                ? dataItem?.PropertyProxy(column.Field)?.GetValue(dataItem)
                : null;
        }

        internal static string GetFormattedValue(this CandyGridColumn column, object dataItem)
        {
            var columnValue = column.GetColumnValue(dataItem);
            if (columnValue == null) return column.EmptyDisplayString;

            var stringValue = columnValue?.ToString() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(column.FormatString))
            {
                try { stringValue = string.Format(column.FormatString, columnValue); }
                catch { /* ignore */ }
            }

            return stringValue;
        }

        internal static void Log(this string info, string source, string member)
        {
            var sourceFormat = $"{source,16}";
            var memberFormat = $"{member,-22}";
            Console.WriteLine($"DBG {sourceFormat}.{memberFormat} | {info}");
        }

        internal static int AddAttachedComponent<T>(this IReadOnlyList<T> container, T child)
            where T : IAttachedComponent
        {
            if (child == null) return -1;

            if (container is IList<T> collection)
            {
                collection.Add(child);
                return collection.Count -1;
            }

            return -1;
        }

        internal static void RemoveAttachedComponent<T>(this IReadOnlyList<T> container, T child)
            where T : IAttachedComponent
        {
            if (!(container is IList<T> collection))
                return;

            if (collection.Count == 0 || child == null)
                return;

            var childindex = child.Index > 0 && child.Index < collection.Count && ReferenceEquals(child, collection[child.Index])
                ? child.Index
                : collection.IndexOf(child);

            if (childindex < 0) return;
            collection.RemoveAt(childindex);
        }
    }
}
