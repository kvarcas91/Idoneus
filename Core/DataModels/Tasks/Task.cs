using Core.Utils;
using Core.ViewModels.Base;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    [Table("tasks")]
    public class Task : ITask
    {
        [Computed]
        public IList<IPerson> Contributors { get; set; }

        [Computed]
        public IList<ISubTask> SubTasks { get; }

        [Key]
        public long ID { get; set; }

        [Computed]
        public DateTime SubmitionDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public decimal Progress { get; set; }
        public uint OrderNumber { get; set; }
        public bool IsCompleted { get; set; }
        public string Content { get; set; }

        public bool AddElement(IElement element)
        {
            throw new NotImplementedException();
        }

        public bool AddElements(IList<IElement> elements)
        {
            throw new NotImplementedException();
        }

        public bool AddPerson(IPerson person)
        {
            throw new NotImplementedException();
        }

        public bool AddPersons(IList<IPerson> person)
        {
            throw new NotImplementedException();
        }

        public bool RemoveElement(IElement element)
        {
            throw new NotImplementedException();
        }

        public bool RemovePerson(IPerson person)
        {
            throw new NotImplementedException();
        }

        public bool UpdateElement(IElement element)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePerson(IPerson person)
        {
            throw new NotImplementedException();
        }
    }
}
