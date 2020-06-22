using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;

namespace Domain.Models.Tasks
{


    [Table("td_tasks")]
    public class TodaysTask : IEntity
    {

        public TodaysTask()
        {

        }

        public TodaysTask(RepetetiveTask task)
        {
            ID = Guid.NewGuid().ToString();
            RepetetiveTaskID = task.ID;
            Content = task.Content;
        }

        [Key]
        public string ID { get; set; }

        public string RepetetiveTaskID { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Content { get; set; }
        public DateTime SubmitionDate { get; set; } = DateTime.Now;
    }
}
