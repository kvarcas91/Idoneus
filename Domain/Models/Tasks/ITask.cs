using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Tasks
{
    public interface ITask
    {
        string ParentID { get; set; }
    }
}
