using Domain.Helpers;
using Domain.Models;
using Domain.Utils;
using System.Drawing;

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
    }
}
