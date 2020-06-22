using Dapper.Contrib.Extensions;
using System;

namespace Domain.Models.Comments
{
    [Table("links")]
    public class Link : IComment
    {
        [Key]
        public string ID { get; set; }
        public string ProjectID { get; set; }
        public string Content { get; set; }
        public DateTime SubmitionDate { get; set; }
    }
}
