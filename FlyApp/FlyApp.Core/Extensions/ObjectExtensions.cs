namespace FlyApp.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Generic object equality check.
        /// The methods considers objects equal if they are both <code>null</code> or are equal
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns><code>true</code> if equal</returns>
        public static bool ObjectsEqual(this object value1, object value2)
        {
            if(value1 is string str1 && value2 == null && string.IsNullOrEmpty(str1))
            {
                return true;
            }

            if(value2 is string str2 && value1 == null && string.IsNullOrEmpty(str2))
            {
                return true;
            }

            return value1 == null && value2 == null || value1 != null && value2 != null && value1.Equals(value2);
        }
    }
}