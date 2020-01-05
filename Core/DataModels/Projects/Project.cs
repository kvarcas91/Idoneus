using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    public class Project
    {

        public string Header { get; set; }

        public string Content { get; set; }

        public Priority Priority { get; set; } = Priority.Default;

        public double Progress { get; set; } = 0;

    }
}
