using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{

    [Table("contributors")]
    public class Contributor : IEntity
    {

        [Key]
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"ID: {ID}; FirstName: {FirstName}; LastName: {LastName};";
        }
    }
}
