using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    public class Contributor : IContributor
    {
        public long ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Initials => $"{FirstName.Substring(0, 1)}{LastName.Substring(0, 1)}";

        public string InitialColor { get; set; }

        public Contributor()
        {
            InitialColor = ColorPool.GetColor();
        }
    }
}
