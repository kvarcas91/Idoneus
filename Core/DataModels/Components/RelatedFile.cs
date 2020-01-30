using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    public class RelatedFile : IFile
    {
        #region Properties


        /// <summary>
        /// related to the project file name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Related to the project file absolute path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Related to the project file extention
        /// </summary>
        public string Extention { get; set; }

        public Icon Icon { get; set; }

        /// <summary>
        /// File version
        /// </summary>
        public ulong Version { get; set; } = 0;
        uint IComponent.Version { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        #endregion

        #region Constructors

        public RelatedFile() { }

        public RelatedFile(string filePath)
        {
            Path = filePath;
            Name = System.IO.Path.GetFileNameWithoutExtension(filePath);
            Extention = System.IO.Path.GetExtension(filePath);
            Icon = Icon.ExtractAssociatedIcon(filePath);
        }

        #endregion

        #region Public Methods

        public bool Copy(string newName)
        {
            throw new NotImplementedException();
        }

        public bool Replace(string destination)
        {
            throw new NotImplementedException();
        }

        public bool Move(string destination)
        {
            var fullFileDestination = $"{destination}{System.IO.Path.DirectorySeparatorChar}{System.IO.Path.GetFileName(Path)}";
            if (!FileHelper.MoveFile(Path, fullFileDestination)) return false;

            Path = fullFileDestination;
            return true;
        }

        public bool Rename(string newName)
        {
            var parent = Directory.GetParent(Path);
            var destination = $"{parent}{System.IO.Path.DirectorySeparatorChar}{newName}{Extention}";
            if (!FileHelper.MoveFile(Path, destination)) return false;
            Path = destination;
            Name = newName;
            return true;
        }

        public bool Delete()
        {
            File.Delete(Path);
            return true;
        }

        #endregion
    }
}
