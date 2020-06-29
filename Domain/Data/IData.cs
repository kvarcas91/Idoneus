using Domain.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Domain.Data
{
    public interface IData
    {
        string Path { get; set; }
        Icon Icon { get; set; }
        string Name { get; set; }

        Response Copy(string newPath, bool overwrite, bool newVersion = false);
        Response Delete();
        Response Rename(string newName);
        Response Move(string newPath, bool overwrite, bool combineDestinationPath = true);
    }
}
