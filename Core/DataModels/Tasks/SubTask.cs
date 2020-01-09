using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Core.Utils;
using Dapper.Contrib.Extensions;
using PropertyChanged;

namespace Core.DataModels
{
    [ImplementPropertyChanged]
    public class SubTask : ISubTask, INotifyPropertyChanged
    {
        public bool IsCompleted { get; set; }

        [Computed]
        public IList<IContributor> Contributors { get; }

        [Key]
        public long ID { get; set; }
        [Computed]
        public int ParentIndex { get; set; }
        public string Content { get; set; }
        [Computed]
        public DateTime SubmitionDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        [Computed]
        public decimal Progress { get; set; }
        public uint OrderNumber { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

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

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
