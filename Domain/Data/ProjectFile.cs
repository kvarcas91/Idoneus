using Domain.Helpers;
using Domain.Models;
using System.Drawing;

namespace Domain.Data
{
    public class ProjectFile : IData
    {
        public string Path { get; set; }
        public Icon Icon { get; set; }
        public string Name { get; set; }
        public string Extention { get; set; }

        public ProjectFile()
        {

        }

        public ProjectFile(string filePath)
        {
            Path = filePath;
            Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Extention = System.IO.Path.GetExtension(filePath);
            Icon = Icon.ExtractAssociatedIcon(filePath);
        }

        public Response Copy(string newPath)
        {
            return FileHelper.Copy(this, newPath);

        }
            
    }
}
