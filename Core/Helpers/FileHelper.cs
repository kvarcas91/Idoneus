using Core.DataModels;
using System.Collections.Generic;
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

        public static bool CreateProjectFolder (string path)
        {
            var exists = Directory.Exists(path);
            Directory.CreateDirectory(path);
            return exists;
        }

        /// <summary>
        /// Generates Project path string 
        /// </summary>
        /// <param name="uuid">project path ID</param>
        /// <returns>generated full project path './Projects/...'</returns>
        public static string GetProjectPathFromUUID (string uuid)
        {
            return $".{Path.DirectorySeparatorChar}Projects{Path.DirectorySeparatorChar}{uuid}";
        }

        public static bool DeleteFolder (string path)
        {
            Directory.Delete(path, recursive: true);
            return true;
        }


        public static IList<IData> GetFolderContent(string path)
        {
            var output = new List<IData>();

            if (!Directory.Exists(path)) return output;

            string[] contentDirectories = Directory.GetDirectories(path);
            string[] contentFiles = Directory.GetFiles(path);

            foreach (var item in contentDirectories)
            {
                if(!IsHidden(item)) output.Add(new RelatedFolder(item));
            }
            foreach (var item in contentFiles)
            {
                output.Add(new RelatedFile(item));
            }

            return output;
        }

        private static bool IsHidden (string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            return name.Substring(0, 1) == "_";
        }

        public static bool CanNavigateBack (string currentPath, string homePath)
        {
            return string.Compare(currentPath, homePath) != 0;
        }

        public static bool MoveFile (string source, string destination)
        {
            try
            {
                File.Move(source, destination);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool MoveFolder(string source, string destination)
        {
            try
            {
                Directory.Move(source, destination);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
