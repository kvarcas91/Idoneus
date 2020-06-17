using System.Text;

namespace Domain.Helpers
{
    public class QueryHelper
    {
        public static string GetComplexQuery<T>(T obj, string t1, string t2, string t3, (string, string) p1, (string, string) p2, int ID, string orderParam, params string[] orderBy)
        {
            var ts1 = 'a';
            var ts2 = 'b';
            var ts3 = 'c';

            var param = new StringBuilder(string.Empty);
            var properties = new StringBuilder(string.Empty);
            var propertyList = PropertyHelper.GetProperties(obj);
            if (propertyList.Count == 0) properties.Append("*");

            for (int i = 0; i < propertyList.Count; i++)
            {
                properties.Append($"{ts1}.{propertyList[i]}");
                properties.Append(i+1 >= propertyList.Count ? "" : ", ");
            }
           
            param.Append("ORDER BY ");
            for (int i = 0; i < orderBy.Length; i++)
            {
                param.Append($"{ts1}.{orderBy[i]}");
                param.Append(i+1 >= orderBy.Length ? $" {orderParam}" : ", ");
            }

        
            
            var query = $"SELECT {properties.ToString()} FROM {t1} {ts1} "+
                $"INNER JOIN {t3} {ts3} ON {ts3}.{p1.Item1} = {ts1}.{p1.Item2} "+
                $"INNER JOIN {t2} {ts2} ON {ts2}.{p2.Item1} = {ts3}.{p2.Item2} WHERE {ts2}.ID = {ID} {param.ToString()}";
            return query;
        }
    }
}
