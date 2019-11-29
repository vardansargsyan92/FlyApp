using System;
using System.Linq;
using System.Reflection;

namespace FlyApp.Core.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the value of the property specified by <paramref name="propertyName"/> for the object <paramref name="obj"/>. 
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the property</returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetRuntimeProperty(propertyName).GetValue(obj);
        }

        /// <summary>
        /// Sets the value of the property specified by <paramref name="propertyName"/> for the object <paramref name="obj"/> to <paramref name="value"/>
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            obj.GetType().GetRuntimeProperty(propertyName).SetValue(obj, value);
        }

        public static bool CheckPropertyIsTypeOf(this Type obj, string propertyName, Type type)
        {
            return obj.GetRuntimeProperties().First(property => property.Name == propertyName).PropertyType == type;
        }
    }
}