using Domain.Models.Tasks;
using System.Collections.Generic;

namespace Domain.Models.Base
{
    public interface IUpdatableProgress
    {
        double GetProgress();
    }
}
