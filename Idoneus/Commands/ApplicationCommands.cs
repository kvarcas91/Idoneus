using Prism.Commands;

namespace Idoneus.Commands
{
    public interface IApplicationCommands
    {
        CompositeCommand UpdateProjectProgress { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        public CompositeCommand UpdateProjectProgress { get; } = new CompositeCommand(true);
    }
}
