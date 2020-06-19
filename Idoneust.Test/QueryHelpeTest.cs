using Domain.Helpers;
using Domain.Models.Project;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Idoneust.Test
{
    public class QueryHelpeTest
    {

        [Fact]
        public void SearchQuery ()
        {
            var expected = "SELECT ID,Header,Content,SubmitionDate,DueDate,Priority,Status,OrderNumber,Progress FROM projects WHERE Status = '2' AND";
            var project = new Project
            {
                Status = Common.Status.InProgress,
                Header = "test"
            };

            //var actual = QueryHelper.BuildSearchQuery(project, "projects", Common.ViewType.InProgress, "test");
            //actual[0].Should().Be(expected);
        }

    }
}
