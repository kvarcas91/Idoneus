using System.Collections.Generic;

namespace Core.DataModels
{
    public interface ISubTask : IInteractiveElement, IOrderable
    {

        bool IsCompleted { get; set; }

        IList<IContributor> Contributors { get; }

    }
}
