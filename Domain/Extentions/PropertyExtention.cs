using System;
using System.Reflection;

namespace Domain.Extentions
{
    internal static class PropertyExtention
    {
        public static bool HasAttribute(this PropertyInfo prop, params Type[] attr)
        {
            if (attr == null || attr.Length == 0) throw new ArgumentNullException("parameters cannot be null or empty");

            foreach (var attribute in attr)
            {
                if (Attribute.IsDefined(prop, attribute)) return true;
            }
            return false;
        }
    }
}
