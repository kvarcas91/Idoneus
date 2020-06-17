using Domain.Models.Project;
using Domain.Models.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Idoneust.Test.DummyData
{
    public static class GetData
    {

        public static Project GetProject(ValueTuple<int, int> taskCount, ValueTuple<int, int> subtaskCount)
        {
            var project = new Project
            {
                ID = 1,
                SubmitionDate = DateTime.Now,
                DueDate = DateTime.MaxValue,
                Header = "test project no 1",
                Content = "this is test project no 1",
                Path = "somewhere in C drive",
                Priority = Common.Priority.High,
                IsArchived = false,
                OrderNumber = 0
            };
            var completedTasks = 0;
            var tasks = new List<ProjectTask>();
            for (int i = 0; i < taskCount.Item1; i++)
            {
                var task = new ProjectTask();
                if (completedTasks < taskCount.Item2)
                {
                    task.IsCompleted = true;
                    completedTasks++;
                }

                var completedSubTasks = 0;
                var subtasks = new List<SubTask>();
                for (int j = 0; j < subtaskCount.Item1; j++)
                {
                    var subTask = new SubTask();
                    if (completedSubTasks < subtaskCount.Item2)
                    {
                        subTask.IsCompleted = true;
                        completedSubTasks++;
                    }
                    subtasks.Add(subTask);

                }

                task.SubTasks = new ObservableCollection<SubTask>(subtasks);
                tasks.Add(task);
            }

            project.Tasks = new ObservableCollection<ProjectTask>(tasks);
            return project;
        }
    }
}
