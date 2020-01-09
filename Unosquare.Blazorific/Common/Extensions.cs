namespace Unosquare.Blazorific.Common
{
    using System;
    using System.Reflection;

    public static class Extensions
    {
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
    }
}
