using System;

namespace Core.DataModels
{
    public interface IComment : IElement
    {

        DateTime SubmitionDate { get; set; }

    }
}
