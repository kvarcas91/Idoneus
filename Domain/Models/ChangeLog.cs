using Common.Enums;
using System;

namespace Domain.Models
{
    public class ChangeLog
    {
        public string ContributorID { get; set; }
        public string Content { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ProjectID { get; set; }
        public ChangeAction ChangeAction { get; set; }
    }
}
