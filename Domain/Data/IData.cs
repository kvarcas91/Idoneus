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
    }
}
