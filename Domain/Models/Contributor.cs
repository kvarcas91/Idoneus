using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{

    [Table("contributors")]
    public class Contributor
    {

        [Key]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }

        public override string ToString()
        {
            return $"ID: {ID}; FirstName: {FirstName}; LastName: {LastName}; Login: {Login}";
        }
    }
}
