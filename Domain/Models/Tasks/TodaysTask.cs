using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;

namespace Domain.Models.Tasks
{


    [Table("td_tasks")]
    public class TodaysTask : IEntity
    {

        [Key]
        public string ID { get; set; }

        public bool IsCompleted { get; set; }
        public string Content { get; set; }
        public DateTime SubmitionDate { get; set; }

        [Computed]
        public DateTime DueDate { get; set; }
    }
}
