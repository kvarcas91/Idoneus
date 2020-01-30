using Core.Helpers;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    public class RelatedFolder : IFolder
    {
        public uint Version { get; set; } = 0;
        public string Path { get; set; }
        public string Name { get; set; }
        public Icon Icon { get; set; }

        public RelatedFolder() { }

        public RelatedFolder(string filePath)
        {
            Path = filePath;
            Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Icon = DefaultIcons.FolderLarge;
        }


        public bool Copy(string destination)
        {
            throw new NotImplementedException();
        }

        public bool Replace(string destination)
        {
            throw new NotImplementedException();
        }

        public bool Delete()
        {
            return FileHelper.DeleteFolder(Path);
        }

        public bool Move(string destination)
        {
            var destinationDirectorry = $"{destination}{System.IO.Path.DirectorySeparatorChar}{Name}";
            if (!FileHelper.MoveFolder(Path, destinationDirectorry)) return false;
            
            Path = destinationDirectorry;
            return true;
        }

        public bool Rename(string newName)
        {
            var parent = Directory.GetParent(Path);
            var destination = ($"{parent}{System.IO.Path.DirectorySeparatorChar}{newName}");
            if (!FileHelper.MoveFolder(Path, destination)) return false;

            Path = destination;
            Name = newName;
            return true;
        }


    }
}
