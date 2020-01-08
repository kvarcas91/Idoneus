using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.DataModels
{
    public interface IProject : IInteractiveElement, IPath
    {

        /// <summary>
        /// Project Title
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// Is project archived
        /// </summary>
        bool IsArchived { get; set; }

        IList<IElement> Comments { get; set; }

        IList<IContributor> Contributors { get; set; } 
        
        ObservableCollection<IElement> Tasks { get; set; }
    }
}
