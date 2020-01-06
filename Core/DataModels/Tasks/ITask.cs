using System.Collections.Generic;

namespace Core.DataModels
{
    public interface ITask : IInteractiveElement, IOrderable
    {

        bool IsCompleted { get; set; }

        IList<IPerson> Contributors { get; }

        IList<ISubTask> SubTasks { get;}

    }
}
