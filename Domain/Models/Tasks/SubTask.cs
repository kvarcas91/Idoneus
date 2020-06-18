using Common;
using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;

namespace Domain.Models.Tasks
{

    [Table("subtasks")]
    public class SubTask : IEntity, ITask
    {

        [Key]
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Content { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public Status Status { get; set; } = Status.Default;
        public int OrderNumber { get; set; }
    }
}
