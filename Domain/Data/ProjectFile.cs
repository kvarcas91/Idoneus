using Domain.Helpers;
using Domain.Models;
using System;
using System.Drawing;
using System.IO;

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

        public Response Copy(string newPath, bool overwrite, bool newVersion = false)
        {
            return FileHelper.Copy(this, newPath, overwrite, newVersion);
        }

        public Response Delete()
        {
            try
            {
                File.Delete(Path);
                return new Response() { Success = true };
            }
            catch (Exception e)
            {
                return new Response() { Success = false, Message = e.Message };
            }
        }

        public Response Move(string destinationPath, bool overwrite, bool combineDestinationPath = true)
        {
            try
            {
                var destination = combineDestinationPath ? FileHelper.Combine(destinationPath, Path) : destinationPath;
                File.Move(Path, destination, overwrite);
                return new Response() { Success = true };
            }
            catch(Exception e)
            {
                return new Response() { Success = false, Message = e.Message };
            }
        }

        public Response Rename(string newName)
        {
            var parent = FileHelper.GetParentPath(Path);
            var destinationPath = FileHelper.Combine(parent, $"{newName}{Extention}");
            try
            {
                var response =  Move(destinationPath, false, false);
                if (!response.Success) return new Response() { Success = false, Message = "Failed to rename file" };

                Name = newName;
                Path = destinationPath;
                return new Response() { Success = true };
            }
            catch(Exception e)
            {
                return new Response() { Success = false, Message = e.Message };
            }
        }
    }
}
