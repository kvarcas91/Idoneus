using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Tasks
{
    public interface ITask : IStatus, ISearchable
    {
        string ParentID { get; set; }
    }
}
