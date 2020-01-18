namespace Unosquare.Blazorific.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Extensions
    {
        internal static IGridColumn[] GetGridDataRequestColumns(this Type t, CandyGrid grid)
        {
            var properties = t.PropertyProxies().Values;
            var result = new List<IGridColumn>(properties.Count);

            foreach (var property in properties)
            {
                if (grid.Columns.FirstOrDefault(c => c.Property != null && c.Property.Name == property.Name) is IGridColumn column)
                {
                    result.Add(column);
                    continue;
                }

                result.Add(new GridDataColumn
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

        internal static IList CreateGenericList(this Type t)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(t);
            return Activator.CreateInstance(constructedListType) as IList;
        }
    }
}
