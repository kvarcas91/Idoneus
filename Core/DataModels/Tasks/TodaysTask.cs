using Dapper.Contrib.Extensions;
using System;

namespace Core.DataModels
{
    [Table("td_tasks")]
    public class TodaysTask : IElement, IDateable
    {
        [Key]
        public long ID { get; set; }
        public bool IsCompleted { get; set; }
        public string Content { get; set; }
        public DateTime SubmitionDate { get; set; }

        [Computed]
        public DateTime DueDate { get; set; }
    }
}
