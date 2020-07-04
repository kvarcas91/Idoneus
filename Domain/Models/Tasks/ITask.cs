using Common;
using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Tasks
{
    public interface ITask : IStatus, ISearchable, IEntity
    {
        string ParentID { get; set; }
        public string Content { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
    }
}
