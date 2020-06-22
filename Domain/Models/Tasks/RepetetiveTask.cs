using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;

namespace Domain.Models.Tasks
{

    [Table("repetetive_tasks")]
    [Serializable]
    public class RepetetiveTask : IEntity
    {

        [Key]
        public string ID { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
