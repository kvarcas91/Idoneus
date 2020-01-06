using System.Collections.Generic;

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
        
        IList<IElement> Tasks { get; set; }
    }
}
