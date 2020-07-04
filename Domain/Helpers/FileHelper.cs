using Domain.Data;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Domain.Helpers
{
    public class FileHelper
    {
        /*
       * https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
       */
        private static Response DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                return new Response() { Success = false, Message = "Source directory does not exist or could not be found" };
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

            return new Response() { Success = true };
        }

        /*
         * https://stackoverflow.com/questions/2553008/directory-move-doesnt-work-file-already-exist
         */
        public static void MoveDirectory(string source, string target)
        {
            var sourcePath = source.TrimEnd('\\', ' ');
            var targetPath = target.TrimEnd('\\', ' ');
            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                 .GroupBy(s => Path.GetDirectoryName(s));

            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);
                foreach (var file in folder)
                {
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);
                }
            }
            Directory.Delete(source, true);
        }

        public static bool CreateFolderIfNotExist(string path)
        {
            var exists = Directory.Exists(path);
            Directory.CreateDirectory(path);
            return exists;
        }

        public static void InitializeProjectFolder(string projectID)
        {
            var _basePath = Path.Combine(".\\Projects", projectID);
            CreateFolderIfNotExist(_basePath);
            var fileList = Directory.GetDirectories(_basePath);
            if (fileList == null || fileList.Length == 0) CreateFolderIfNotExist(Path.Combine(_basePath, "V1"));
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static void CreateDB(string path)
        {
            File.Create(path);
        }

        public static IEnumerable<ProjectFolder> GetVersions(string path)
        {
            string[] contentDirectories = Directory.GetDirectories(path);
            var output = new List<ProjectFolder>();
            foreach (var item in contentDirectories)
            {
                var name = Path.GetFileNameWithoutExtension(item).Substring(0, 1);
                if (name.Equals("V")) output.Add(new ProjectFolder(item));
            }

            return output;
        }

        public static IEnumerable<IData> GetFolderContent(string path)
        {
            var output = new List<IData>();

            if (!Directory.Exists(path)) return output;

            string[] contentDirectories = Directory.GetDirectories(path);
            string[] contentFiles = Directory.GetFiles(path);

            foreach (var item in contentDirectories)
            {
                output.Add(new ProjectFolder(item));
            }
            foreach (var item in contentFiles)
            {
                output.Add(new ProjectFile(item));
            }

            return output;
        }

        public static Response Copy(IData data, string newPath, bool overwrite, bool newVersion = false)
        {
            if (data is ProjectFile file)
            {
                try
                {
                    File.Copy(file.Path, Path.Combine(newPath, $"{data.Name}{file.Extention}"), overwrite);
                    return new Response { Success = true };
                }
                catch (Exception e) { return new Response { Success = false, Message = e.Message }; }
            }
            if (data is ProjectFolder folder)
            {
                if (!overwrite && !newVersion && Directory.Exists(folder.Path)) return new Response() { Success = false, Message = "Directory already exist" };
                return DirectoryCopy(folder.Path, Path.Combine(newPath, data.Name));
            }

            return new Response() { Success = false, Message = "Not supported copy action" };
        }

        public static bool CanNavigateBack(string currentPath, string basePath)
        {
            return string.Compare(currentPath, basePath) != 0;
        }

        public static string GetParentPath (string currentPath)
        {
            string[] directories = currentPath.Split(Path.DirectorySeparatorChar);
            if (directories.Length > 1) directories.SetValue(string.Empty, directories.Length - 1);

            return Path.Combine(directories);
        }

        public static string GetFullParentPath(string path)
        {
            return Directory.GetParent(path).FullName;
        }

        public static string GetExecutablePath(string path)
        {
            var parent = Directory.GetParent(path).FullName;
            return $"{Path.Combine(parent, Path.GetFileName(path))}";
        }

        public static string GetFullPath(string path)
        {
            return Path.Combine(GetFullParentPath(path), Path.GetFileName(path));
        }

        public static string Combine(string basePath, string oldPath)
        {
            return Path.Combine(basePath, Path.GetFileName(oldPath));
        }

        private static ParameterizedResponse<bool> IsDirectory(string path)
        {
            FileAttributes attr;
            try
            {
                attr = File.GetAttributes(path);
            }
            catch (FileNotFoundException)
            {
                return new ParameterizedResponse<bool>() { Message = "Sorry, coulnd't drop file. It might not exist" };
            }

            return new ParameterizedResponse<bool>() {Data = attr.HasFlag(FileAttributes.Directory) };
        }

        public static ParameterizedResponse<IEnumerable<IData>> GetFilesFromPath(string[] filePaths)
        {
            var output = new List<IData>();
            foreach (var path in filePaths)
            {

                var isDirectory = IsDirectory(path);
                if (!string.IsNullOrEmpty(isDirectory.Message))
                {
                    return new ParameterizedResponse<IEnumerable<IData>>() { Message = isDirectory.Message};
                }

                IData data;
                if (isDirectory.Data) data = new ProjectFolder(path);
                else data = new ProjectFile(path);

                output.Add(data);
            }
            return new ParameterizedResponse<IEnumerable<IData>>() { Data = output };
        }

        public static bool Contains(IEnumerable<IData> data, string fileName)
        {

            foreach (var item in data)
            { 
               if (Path.GetFileName(item.Path).Equals(fileName)) return true;
            }

            return false;
        }
    }
}
