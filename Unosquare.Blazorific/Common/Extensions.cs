namespace Unosquare.Blazorific.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public static class Extensions
    {
        private static readonly object SyncLock = new object();
        private static readonly Dictionary<Type, IPropertyProxy[]> ProxiesCache = new Dictionary<Type, IPropertyProxy[]>(64);

        /// <summary>
        /// Creates a property proxy that stores getter and setter delegates.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>The property proxy.</returns>
        internal static IPropertyProxy CreatePropertyProxy(this PropertyInfo propertyInfo)
        {
            var genericType = typeof(PropertyProxy<,>)
                .MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
            return Activator.CreateInstance(genericType, propertyInfo) as IPropertyProxy;
        }

        internal static IPropertyProxy[] GetPropertyProxies(this Type t)
        {
            lock (SyncLock)
            {
                if (ProxiesCache.ContainsKey(t))
                    return ProxiesCache[t];

                var result = new List<IPropertyProxy>(64);
                var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in props)
                    result.Add(p.CreatePropertyProxy());

                ProxiesCache[t] = result.ToArray();
                return ProxiesCache[t];
            }
        }

        internal static GridDataColumn[] GetGridDataRequestColumns(this Type t, CandyGrid grid)
        {
            var properties = t.GetPropertyProxies();
            var result = new List<GridDataColumn>(properties.Length);

            foreach (var property in properties)
            {
                result.Add(new GridDataColumn
                {
                    DataType = property.PropertyType.ToGridColumnDataType(),
                    Name = property.Name,
                });
            }

            return result.ToArray();
        }

        internal static GridColumnDataType ToGridColumnDataType(this Type t)
        {
            var type = Nullable.GetUnderlyingType(t) ?? t;

            if (type.IsValueType)
            {
                var value = Activator.CreateInstance(type);
                if (type == typeof(bool)) return GridColumnDataType.Boolean;
                if (type == typeof(DateTime)) return GridColumnDataType.DateTime;
                if (type.IsEnum || value.IsNumeric()) return GridColumnDataType.Numeric;
            }

            return GridColumnDataType.String;
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
