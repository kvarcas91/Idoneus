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
    }
}
