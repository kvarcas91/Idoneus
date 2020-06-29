using Common;
using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;

namespace Domain.Models.Tasks
{

    [Table("subtasks")]
    [Serializable]
    public class SubTask : ITask
    {

        [Key]
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Content { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public Status Status { get; set; } = Status.Archived;
        public int OrderNumber { get; set; }

        public bool HasString(string param)
        {
            if (Content.ToLower().Contains(param) ||
                Status.ToString().ToLower().Contains(param)) return true;

            return false;
        }
    }
}
