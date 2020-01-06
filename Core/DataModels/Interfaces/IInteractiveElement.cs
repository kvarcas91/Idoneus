using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataModels
{
    public interface IInteractiveElement : IElement, IDateable, IPrioritizable, IProgressable
    {

        bool AddPerson(IPerson person);

        bool AddPersons(IList<IPerson> person);

        bool RemovePerson(IPerson person);

        bool UpdatePerson(IPerson person);

        bool AddElement(IElement element);

        bool AddElements(IList<IElement> elements);

        bool RemoveElement(IElement element);

        bool UpdateElement(IElement element);
    }
}
