using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.DataModels
{
    public interface ITask : IInteractiveElement, IOrderable
    {

        bool IsCompleted { get; set; }

        ObservableCollection<IContributor> Contributors { get; set; }

        ObservableCollection<ISubTask> SubTasks { get; }

    }
}
