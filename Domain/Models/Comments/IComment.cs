using Domain.Models.Base;
using System;

namespace Domain.Models.Comments
{
    public interface IComment : IEntity
    {
        public string ProjectID { get; set; }
        public string Content { get; set; }
        public DateTime SubmitionDate { get; set; }
    }
}