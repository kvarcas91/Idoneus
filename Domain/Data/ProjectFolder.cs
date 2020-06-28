using Domain.Helpers;
using Domain.Models;
using Domain.Utils;
using System;
using System.Drawing;
using System.IO;

namespace Domain.Data
{
    public class ProjectFolder : IData
    {
        public string Path { get; set; }
        public Icon Icon { get; set; }
        public string Name { get; set; }

        public ProjectFolder()
        {

        }

        public ProjectFolder(string filePath)
        {
            Path = filePath;
            Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Icon = DefaultIcons.FolderLarge;
        }

        public override string ToString()
        {
            return Name;
        }

        public Response Copy(string newPath)
        {
            
            return FileHelper.Copy(this, newPath);
        }
        public Response Delete()
        {
            var dir = new DirectoryInfo(Path);
            try
            {
                dir.Delete(true);
                return new Response() { Success = true };
            }
            catch (Exception e)
            {
                return new Response() { Success = false, Message = e.Message };
            }
        }
    }
}
