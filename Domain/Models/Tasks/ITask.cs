using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Tasks
{
    public interface ITask : IStatus, ISearchable, IEntity
    {
        string ParentID { get; set; }
    }
}
