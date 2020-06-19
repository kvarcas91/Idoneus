using Dapper;
using Dapper.Contrib.Extensions;
using Domain.Models;
using Domain.Models.Base;
using Domain.Repository.Helpers;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Domain.Repository.Base
{
    public abstract class BaseRepository
    {

        protected IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity, new()
        {
            using var connection = GetConnection();
            var output = connection.GetAll<TEntity>();
            return output;
        }

        protected IEnumerable<TEntity> GetAll<TEntity>(string query) where TEntity : class, IEntity, new()
        {
            using var connection = GetConnection();
            var output = connection.Query<TEntity>(query);
            return output;
        }

        protected IEnumerable<TEntity> GetAll<TEntity>((dynamic, dynamic) param) where TEntity : class, IEntity, new()
        {
            using var connection = GetConnection();
            
            var output = connection.Query<TEntity>("");
            return output;
        }

        public string GetTableName<TObject>()
        {

                var output = ((TableAttribute)typeof(TObject).GetCustomAttributes(true)[0]).Name;
                return output;
        }




        //public IEnumerable<Project> GetAll() 
        //{
        //    using var connection = new SqliteConnection(GetConnectionString());
        //    var query = "SELECT * FROM projects";
        //    var output = connection.Query<Project>(query);
        //    if (output == null) return output;

        //    foreach (var item in output)
        //    {
        //        var tasksQuery = "SELECT * " +
        //        "FROM tasks t " +
        //        "INNER JOIN project_tasks p ON p.taskID = t.ID " +
        //        $"INNER JOIN projects pr on pr.ID = p.projectID WHERE pr.ID = {item.ID} order by t.OrderNumber ASC, t.DueDate ASC";
        //        var tasks = connection.Query<ProjectTask>(tasksQuery);
        //        if (tasks == null) continue;

        //        foreach (var task in tasks)
        //        {
        //            var subtaskQuery = "SELECT s.ID, s.Content, s.IsCompleted, s.Priority, s.DueDate, s.OrderNumber " +
        //                               "FROM subtasks s " +
        //                               "INNER JOIN task_subtasks p ON p.subtaskID = s.ID " +
        //                               $"INNER JOIN tasks ts on ts.ID = p.taskID WHERE ts.ID = {task.ID} order by s.IsCompleted, s.DueDate, s.Priority DESC";
        //            var subtasks = connection.Query<SubTask>(subtaskQuery);
        //            task.SubTasks = new ObservableCollection<SubTask>(subtasks);
        //        }
        //        item.Tasks = new ObservableCollection<ProjectTask>(tasks);
        //    }

        //    return output;
        //}


        public T Get<T>(string field, string param)
        {
            using var connection = new SqliteConnection(GetConnectionString());
            var query = $"SELECT * FROM contributors WHERE {field}='{param}'";
            var output = connection.QueryFirstOrDefault<T>(query);
            return output;
        }

        #region Init

        private static string GetConnectionString()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = $".{Path.DirectorySeparatorChar}Database{Path.DirectorySeparatorChar}db.db";
            return connectionStringBuilder.ConnectionString;     
        }

        public Response Initialize()
        {
            return DBHelper.Initialize(GetConnectionString());
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(GetConnectionString());
        }

      

        #endregion // Init
    }
}
