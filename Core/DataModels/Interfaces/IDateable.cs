using System;

namespace Core.DataModels
{
    public interface IDateable
    {

        /// <summary>
        /// Submition date. Usually local.now()
        /// </summary>
        DateTime SubmitionDate { get; set; }

        /// <summary>
        /// Deadline
        /// </summary>
        DateTime DueDate { get; set; }

    }
}
