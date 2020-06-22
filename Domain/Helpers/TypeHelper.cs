using System;

namespace Domain.Helpers
{
    internal static class TypeHelper
    {

        public static dynamic GetValueBasedOnType(dynamic value)
        {
            var type = value.GetType();
            if (type.IsEnum)
            {
                return $"'{(int)value}'";
            }
            if (type == typeof(DateTime))
            {
                return $"'{value.ToString("yyyy-MM-dd HH:mm:ss")}'";
            }
            if (type == typeof(string))
            {
                return $"'{value}'";
            }
            if (type == typeof(bool))
            {
                return $"'{Convert.ToInt32(value)}'";
            }

            return $"'{value}'";
        }
    }
}
