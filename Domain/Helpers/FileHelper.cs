using Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Domain.Helpers
{
    public class FileHelper
    {

        public static bool CreateFolderIfNotExist(string path)
        {
            var exists = Directory.Exists(path);
            Directory.CreateDirectory(path);
            return exists;
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static void CreateDB(string path)
        {
            File.Create(path);
        }

    }
}
