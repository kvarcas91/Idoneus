using Common;
using Dapper.Contrib.Extensions;
using Prism.Mvvm;
using System;
using System.Text.Json.Serialization;

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
            IsSelected = subTask.IsSelected;
        }

        [Key]
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Content { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }

        private Status _status = Status.InProgress;
        public Status Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        [JsonIgnore]
        public int OrderNumber { get; set; }


        private bool _isSelected = false;

        [Computed]
        [JsonIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

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
