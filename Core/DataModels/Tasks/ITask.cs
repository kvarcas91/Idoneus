using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.DataModels
{
    public interface ITask : IInteractiveElement, IOrderable
    {

        bool IsCompleted { get; set; }

        IList<IPerson> Contributors { get; }

        ObservableCollection<ISubTask> SubTasks { get; }

    }
}
