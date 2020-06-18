using Dapper.Contrib.Extensions;
using Domain.Attributes;
using Domain.Extentions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Helpers
{
    public class PropertyHelper
    {
        public static List<string> GetProperties<T>(T obj, bool includeID = true, bool exportable = false, bool singleKeyValue = false)
        {
            var output = new List<string>();
            foreach (var property in obj.GetType().GetProperties())
            {
                if (!includeID && !singleKeyValue)
                {
                    if (property.HasAttribute(typeof(KeyAttribute))) continue;
                }
                if(singleKeyValue)
                {
                    if (property.HasAttribute(typeof(KeyAttribute)))
                    {
                        output.Add(property.Name);
                        return output;
                    }
                }
                if (exportable)
                {
                    if (property.HasAttribute(typeof(NotExportableAttribute)) && !singleKeyValue) continue;
                }
                else
                {
                    if (property.HasAttribute(typeof(ExportableAttribute)) && !singleKeyValue) continue;
                }
                if (property.HasAttribute(typeof(ComputedAttribute)) && !singleKeyValue) continue;

                output.Add(property.Name);
            }
            return output;
        }
       
    }
}
