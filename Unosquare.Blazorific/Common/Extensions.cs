namespace Unosquare.Blazorific.Common
{
    using System;
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
    }
}
