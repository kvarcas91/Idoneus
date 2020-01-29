using Core.Utils;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    public class Contributor : IContributor
    {
        [Key]
        public long ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Computed]
        public string FullName => $"{FirstName} {LastName}";

        [Computed]
        public string Initials => $"{FirstName.Substring(0, 1)}{LastName.Substring(0, 1)}";

        [Computed]
        public string InitialColor { get; set; }

        [Computed]
        public bool IsSelected { get; set; } = false;

        public Contributor()
        {
            InitialColor = ColorPool.GetColor();
        }

        public Contributor(string fullName) : this()
        {
            var name = fullName.Split(new[] { ' ' }, 2);
            FirstName = name[0];
            LastName = name[1];
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Contributor))
                return false;

            return GetHashCode() == ((Contributor)obj).GetHashCode();
        }

        public override int GetHashCode()
        {
            return (int)ID;
        }
    }
}
