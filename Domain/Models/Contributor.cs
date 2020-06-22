using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;

namespace Domain.Models
{

    [Table("contributors")]
    [Serializable]
    public class Contributor : IEntity
    {

        [Key]
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
