using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    public class Task
    {

        public string Content { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}
