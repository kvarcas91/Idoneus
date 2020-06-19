using Dapper.Contrib.Extensions;
using Domain.Attributes;
using Domain.Extentions;
using Domain.Models;
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

        public static List<PropInfo> GetProperties<T>(T obj, bool searchable = false, bool includeID = true)
        {
            var propValues = new List<PropInfo>();
            foreach (var property in obj.GetType().GetProperties())
            {
                if (!includeID)
                {
                    if (property.HasAttribute(typeof(KeyAttribute))) continue;
                }
                if (property.HasAttribute(typeof(ComputedAttribute))) continue;
                if (searchable && !property.HasAttribute(typeof(SearchableAttribute))) continue;

                propValues.Add(new PropInfo
                {
                    Type = property.PropertyType,
                    Name = property.Name,
                    Value = property.GetValue(obj)
                });
            }

            return propValues;
        }

        public static string GetSearchablePropertyAllocations(List<PropInfo> props, string key, bool caseSensitive, string parentChar = "")
        {
            var output = new StringBuilder();
            for (int i = 0; i < props.Count; i++)
            {
                if (caseSensitive) output.Append($"{parentChar}{ props[i].Name} LIKE '%{key}%'");
                else output.Append($"lower({parentChar}{ props[i].Name}) LIKE lower('%{key}%')");

                if (i + 1 < props.Count) output.Append(" OR ");
            }
            return output.ToString();
        }
    }
}
