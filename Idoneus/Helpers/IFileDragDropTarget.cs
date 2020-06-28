using Domain.Data;

namespace Idoneus.Helpers
{
    public interface IFileDragDropTarget
    {
        void OnFileDrop(string[] filePaths);
        void OnInnerFileDrop(IData source, IData destinationComponent);
    }
}
