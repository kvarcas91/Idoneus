using Dapper.Contrib.Extensions;
using Domain.Extentions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Helpers
{
    public class PropertyHelper
    {
        public static List<string> GetProperties<T>(T obj, bool includeID = true)
        {
            var output = new List<string>();
            foreach (var property in obj.GetType().GetProperties())
            {
                if (!includeID)
                {
                    if (property.HasAttribute(typeof(KeyAttribute))) continue;
                }
                if (property.HasAttribute(typeof(ComputedAttribute))) continue;

                output.Add(property.Name);
            }
            return output;
        }
       
    }
}
