using Common;
using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Tasks
{

    [Table("subtasks")]
    public class SubTask : IEntity, ITask
    {

        [Key]
        public int ID { get; set; }

        public string Content { get; set; }
        public Priority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int OrderNumber { get; set; }
    }
}
