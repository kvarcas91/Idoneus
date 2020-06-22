﻿using Domain.Models.Tasks;
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
            var query = $"SELECT * FROM td_tasks";
            return GetAll<RepetetiveTask>(query);
        }

        public (int, int) GetTodaysTaskProgress()
        {
            var count = GetCount<TodaysTask>("td_tasks");
            var completedCount = GetCount<TodaysTask>("IsCompleted = '1'", "td_tasks");
            return (count, completedCount);
        }


    }
}
