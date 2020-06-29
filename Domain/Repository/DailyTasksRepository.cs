using Domain.Models.Tasks;
using Domain.Repository.Base;
using System;
using System.Collections.Generic;

namespace Domain.Repository
{
    public class DailyTasksRepository : BaseRepository
    {
        public IEnumerable<TodaysTask> GetTodaysTasks(int targetDate)
        {
            string dateQuery = targetDate == -2 ? "" : $"WHERE SubmitionDate like '{DateTime.Now.AddDays(targetDate).ToString(dateFormat)}%'";
            var query = $"SELECT * FROM td_tasks {dateQuery} ORDER BY IsCompleted";
            return GetAll<TodaysTask>(query);
        }

        public IEnumerable<TodaysTask> GetMissedTasks()
        {
            var query = $"SELECT * FROM td_tasks WHERE SubmitionDate < DATE('now') AND IsCompleted = '0' ORDER BY IsCompleted";
            return GetAll<TodaysTask>(query);
        }

        public IEnumerable<RepetetiveTask> GetRepetetiveTasks()
        {
            var query = $"SELECT * FROM repetetive_tasks";
            var output = GetAll<RepetetiveTask>(query);
            if (output != null)
            {
                foreach (var item in output)
                {
                    item.SetDayList(true);
                }
            }
            return output;
        }

        public (int, int) GetTodaysTaskProgress()
        {
            var count = GetCount<TodaysTask>("td_tasks");
            var completedCount = GetCount<TodaysTask>("IsCompleted = '1'", "td_tasks");
            return (count, completedCount);
        }


    }
}
