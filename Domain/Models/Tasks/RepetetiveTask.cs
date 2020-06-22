using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models.Tasks
{

    [Table("repetetive_tasks")]
    public class RepetetiveTask : IEntity
    {

        [Key]
        public string ID { get; set; }
        public string Content { get; set; }

    }
}
