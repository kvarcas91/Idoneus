using System.Drawing;

namespace Core.DataModels
{
    public interface IComponent
    {

        string Name { get; set; }
        uint Version { get; set; }

        Icon Icon { get; set; }

    }
}
