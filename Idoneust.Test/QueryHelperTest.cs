using Domain.Helpers;
using Domain.Models.Tasks;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Idoneust.Test
{
    public class QueryHelperTest
    {
        [Fact]
        public void ComplexTaskQuery()
        {
            var expectedQuery = "SELECT a.ID, a.Content, a.Priority, a.DueDate, a.IsCompleted, a.OrderNumber FROM tasks a " +
                                "INNER JOIN project_tasks c ON c.taskID = a.ID " +
                                "INNER JOIN projects b ON b.ID = c.projectID WHERE b.ID = 14 ORDER BY a.OrderNumber, a.DueDate ASC";

            var actualQuery = QueryHelper.GetComplexQuery(new ProjectTask(), "tasks", "projects", "project_tasks", ("taskID", "ID"), ("ID", "projectID"), 14, "ASC", "OrderNumber", "DueDate");
            actualQuery.Should().Be(expectedQuery);
        }

        [Fact]
        public void ComplexSubTaskQuery()
        {
            var expectedQuery = "SELECT a.ID, a.Content, a.Priority, a.DueDate, a.IsCompleted, a.OrderNumber FROM subtasks a " +
                                "INNER JOIN task_subtasks c ON c.subtaskID = a.ID " +
                                "INNER JOIN tasks b ON b.ID = c.taskID WHERE b.ID = 36 ORDER BY a.IsCompleted, a.DueDate, a.Priority DESC";

            var actualQuery = QueryHelper.GetComplexQuery(new SubTask(), "subtasks", "tasks", "task_subtasks", ("subtaskID", "ID"), ("ID", "taskID"), 36, "DESC", "IsCompleted", "DueDate", "Priority");
            actualQuery.Should().Be(expectedQuery);
        }

    }
}
