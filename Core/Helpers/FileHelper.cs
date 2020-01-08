using System.IO;

namespace Core.Helpers
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

    }
}
