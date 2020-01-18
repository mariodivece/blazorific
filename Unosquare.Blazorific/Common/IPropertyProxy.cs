namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a generic interface to store getters and setters for high speed access to properties.
    /// </summary>
    public interface IPropertyProxy
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Gets the associated reflection property info.
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Gets the type owning this property proxy.
        /// </summary>
        Type EnclosingType { get; }

        /// <summary>
        /// Gets the property value via a stored delegate.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The property value.</returns>
        object GetValue(object instance);

        /// <summary>
        /// Sets the property value via a stored delegate.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        void SetValue(object instance, object value);
    }

    /// <summary>
    /// Provides functionality to access <see cref="IPropertyProxy"/> objects
    /// associated with types. Getters and setters are stored as delegates compiled
    /// from constructed lambda expressions for fast access.
    /// </summary>
    public static class PropertyProxyExtensions
    {
        private static readonly object SyncLock = new object();
        private static readonly Dictionary<Type, Dictionary<string, IPropertyProxy>> ProxyCache =
            new Dictionary<Type, Dictionary<string, IPropertyProxy>>(32);

        /// <summary>
        /// Gets the property proxies associated with a given type.
        /// </summary>
        /// <param name="t">The type to retrieve property proxies from.</param>
        /// <returns>A dictionary with property names as keys and <see cref="IPropertyProxy"/> objects as values.</returns>
        public static Dictionary<string, IPropertyProxy> PropertyProxies(this Type t)
        {
            lock (SyncLock)
            {
                if (ProxyCache.ContainsKey(t))
                    return ProxyCache[t];

                var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var result = new Dictionary<string, IPropertyProxy>(properties.Length, StringComparer.InvariantCultureIgnoreCase);
                foreach (var propertyInfo in properties)
                    result[propertyInfo.Name] = new PropertyInfoProxy(t, propertyInfo);

                ProxyCache[t] = result;
                return result;
            }
        }

        /// <summary>
        /// Gets the property proxies associated with the provided instance type.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="obj">The instance.</param>
        /// <returns>A dictionary with property names as keys and <see cref="IPropertyProxy"/> objects as values.</returns>
        public static Dictionary<string, IPropertyProxy> PropertyProxies<T>(this T obj) =>
            typeof(T).PropertyProxies();

        /// <summary>
        /// Gets the property proxy given the property name.
        /// </summary>
        /// <param name="t">The associated type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The associated <see cref="IPropertyProxy"/></returns>
        public static IPropertyProxy PropertyProxy(this Type t, string propertyName)
            => t.PropertyProxies()[propertyName];

        /// <summary>
        /// Gets the property proxy given the property name.
        /// </summary>
        /// <typeparam name="T">The type of instance to extract proxies from.</typeparam>
        /// <param name="obj">The instance to extract proxies from.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The associated <see cref="IPropertyProxy"/></returns>
        public static IPropertyProxy PropertyProxy<T>(this T obj, string propertyName)
            => typeof(T).PropertyProxies()[propertyName];

        /// <summary>
        /// Gets the property proxy given the property name as an expression.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <typeparam name="V">The property value type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The associated <see cref="IPropertyProxy"/></returns>
        public static IPropertyProxy PropertyProxy<T, V>(this T obj, Expression<Func<T, V>> propertyExpression)
            => typeof(T).PropertyProxies()[propertyExpression.PropertyName()];

        /// <summary>
        /// Reads the property value.
        /// </summary>
        /// <typeparam name="T">The type to get property proxies from.</typeparam>
        /// <typeparam name="V">The type of the property.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The value obtained from the associated <see cref="IPropertyProxy"/></returns>
        public static V ReadProperty<T, V>(this T instance, Expression<Func<T, V>> propertyExpression) =>
            (V)typeof(T).PropertyProxies()[propertyExpression.PropertyName()].GetValue(instance);

        /// <summary>
        /// Reads the property value.
        /// </summary>
        /// <typeparam name="T">The type to get property proxies from.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value obtained from the associated <see cref="IPropertyProxy"/></returns>
        public static object ReadProperty<T>(this T instance, string propertyName) =>
            typeof(T).PropertyProxies()[propertyName].GetValue(instance);

        /// <summary>
        /// Writes the property value.
        /// </summary>
        /// <typeparam name="T">The type to get property proxies from.</typeparam>
        /// <typeparam name="V">The type of the property.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="value">The value.</param>
        public static void WriteProperty<T, V>(this T instance, Expression<Func<T, V>> propertyExpression, V value) =>
            typeof(T).PropertyProxies()[propertyExpression.PropertyName()].SetValue(instance, value);

        /// <summary>
        /// Writes the property value using the property proxy.
        /// </summary>
        /// <typeparam name="T">The type to get property proxies from.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void WriteProperty<T>(this T instance, string propertyName, object value) =>
            typeof(T).PropertyProxies()[propertyName].SetValue(instance, value);

        private static string PropertyName<T, V>(this Expression<Func<T, V>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression == null
                ? (propertyExpression.Body as UnaryExpression).Operand as MemberExpression
                : propertyExpression.Body as MemberExpression;

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// The concrete and hidden implementation of the <see cref="IPropertyProxy"/> implementation.
        /// </summary>
        /// <seealso cref="System.Reflection.IPropertyProxy" />
        private sealed class PropertyInfoProxy : IPropertyProxy
        {
            private readonly Func<object, object> Getter;
            private readonly Action<object, object> Setter;

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertyInfoProxy"/> class.
            /// </summary>
            /// <param name="declaringType">Type of the declaring.</param>
            /// <param name="propertyInfo">The property information.</param>
            public PropertyInfoProxy(Type declaringType, PropertyInfo propertyInfo)
            {
                Property = propertyInfo;
                EnclosingType = declaringType;
                Getter = CreateLambdaGetter(declaringType, propertyInfo);
                Setter = CreateLambdaSetter(declaringType, propertyInfo);
            }

            /// <inheritdoc />
            public PropertyInfo Property { get; }

            /// <inheritdoc />
            public Type EnclosingType { get; }

            /// <inheritdoc />
            public string Name => Property.Name;

            /// <inheritdoc />
            public Type PropertyType => Property.PropertyType;

            /// <inheritdoc />
            public object GetValue(object instance) => Getter?.Invoke(instance);

            /// <inheritdoc />
            public void SetValue(object instance, object value) => Setter?.Invoke(instance, value);

            private static Func<object, object> CreateLambdaGetter(Type instanceType, PropertyInfo propertyInfo)
            {
                if (!propertyInfo.CanRead)
                    return null;

                var instanceParameter = Expression.Parameter(typeof(object), "instance");
                var typedInstance = Expression.Convert(instanceParameter, instanceType);
                var property = Expression.Property(typedInstance, propertyInfo);
                var convert = Expression.Convert(property, typeof(object));
                var dynamicGetter = (Func<object, object>)Expression.Lambda(convert, instanceParameter).Compile();
                return dynamicGetter;
            }

            private static Action<object, object> CreateLambdaSetter(Type instanceType, PropertyInfo propertyInfo)
            {
                if (!propertyInfo.CanWrite)
                    return null;

                var instanceParameter = Expression.Parameter(typeof(object), "instance");
                var valueParameter = Expression.Parameter(typeof(object), "value");

                var typedInstance = Expression.Convert(instanceParameter, instanceType);
                var property = Expression.Property(typedInstance, propertyInfo);
                var propertyValue = Expression.Convert(valueParameter, propertyInfo.PropertyType);

                var body = Expression.Assign(property, propertyValue);
                var dynamicSetter = Expression.Lambda<Action<object, object>>(body, instanceParameter, valueParameter).Compile();

                return dynamicSetter;
            }
        }
    }
}
