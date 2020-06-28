﻿using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Domain.Extentions
{
    public static class ListExtention
    {

        public static ObservableCollection<T> Clone<T>(this ObservableCollection<T> list)
        {
           
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream, list);
            }
            catch
            {
                return list;
            }
           
            stream.Position = 0;
            return (ObservableCollection<T>)formatter.Deserialize(stream);
            
        }
    }
}
