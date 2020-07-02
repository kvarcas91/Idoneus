using Common;
using Dapper.Contrib.Extensions;
using Prism.Mvvm;
using System;

namespace Domain.Models.Tasks
{

    [Table("subtasks")]
    [Serializable]
    public class SubTask : BindableBase, ITask
    {

        public SubTask()
        {

        }

        public SubTask(SubTask subTask)
        {
            ID = subTask.ID;
            ParentID = subTask.ParentID;
            Content = subTask.Content;
            DueDate = subTask.DueDate;
            Priority = subTask.Priority;
            Status = subTask.Status;
            OrderNumber = subTask.OrderNumber;
        }

        [Key]
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Content { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public Status Status { get; set; } = Status.InProgress;
        public int OrderNumber { get; set; }

        public bool HasString(string param)
        {
            if (Content.ToLower().Contains(param) ||
                Status.ToString().ToLower().Contains(param)) return true;

            return false;
        }

        public bool HasString(string param, int viewType)
        {
            if ((Content.ToLower().Contains(param) && (int)Status == viewType) ||
                (Status.ToString().ToLower().Contains(param) && (int)Status == viewType)) return true;

            return false;
        }
    }
}
