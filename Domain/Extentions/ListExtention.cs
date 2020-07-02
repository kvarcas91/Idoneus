using Domain.Models.Tasks;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Domain.Extentions
{
    public static class ListExtention
    {

        public static ObservableCollection<T> Clone<T>(this ObservableCollection<T> list) where T : class, new()
        {

            var output = new ObservableCollection<T>();
            foreach (var item in list)
            {
                output.Add((T)Activator.CreateInstance(typeof(T), new object[] { item}));
            }

            return output;
           
            //BinaryFormatter formatter = new BinaryFormatter();
            //MemoryStream stream = new MemoryStream();
            //try
            //{
            //    formatter.Serialize(stream, list);
            //}
            //catch (Exception e)
            //{
            //    return list;
            //}
           
            //stream.Position = 0;
            //return (ObservableCollection<T>)formatter.Deserialize(stream);
            
        }

        public static TObject Clone<TObject>(this TObject obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream, obj);
            }
            catch
            {
                return obj;
            }

            stream.Position = 0;
            return (TObject)formatter.Deserialize(stream);
        }
    }
}
