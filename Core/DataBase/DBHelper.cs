using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using Dapper;
using Core.DataModels;
using Core.Utils;
using System.Diagnostics;

namespace Core.DataBase
{
	public class DBHelper
	{

        private static readonly object _lock = new object();

        #region Projects

        public static IList<IProject> GetProjects(ViewType viewType)
        {
            
            var sql = "select * from projects";
            switch (viewType)
            {
                case ViewType.Ongoing:
                    sql = $"{sql} where IsArchived = '0'";
                    break;
                case ViewType.Archived:
                    sql = $"{sql} where IsArchived = '1'";
                    break;
            }
            sql = $"{sql} order by OrderNumber";


            using IDbConnection connection = new SQLiteConnection(GetConnectionString());

            List<Project> output = connection.Query<Project>(sql).ToList();

            foreach (var project in output)
            {
                //project.Progress = GetProjectProgress(project.ID);
                //project.AddPersons(GetProjectContributors(project.ID));

            }
            connection.Dispose();

            return output.ConvertAll(o => (IProject)o);
        }


        #endregion // Projects

        #region Support DB Methods

        //private static double GetProjectprogress(long projectID)
        //{
        //    var ProjectTasks = GetProjectTasks(projectID);
        //    var totalTaskCount = ProjectTasks.Count;
        //    if (totalTaskCount == 0) return 0;
        //    var completedTaskCount = 0;
        //    foreach (var task in ProjectTasks)
        //    {
        //        if (task.IsCompleted)
        //        {
        //            completedTaskCount++;
        //        }
        //    }
        //    return (completedTaskCount * 100) / totalTaskCount;
        //}

        #endregion // Support DB Methods

        #region Support Methodds

        /// <summary>
        /// Creates tables if database is not found
        /// </summary>
        public static void CreateTablesIfNotExist()
		{
            lock(_lock)
            {
                using IDbConnection connection = new SQLiteConnection(GetConnectionString());

                var query = @"CREATE TABLE projects (
						ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
						SubmitionDate TEXT NOT NULL,
						Content TEXT NOT NULL,
						Path TEXT NOT NULL,
						DueDate TEXT NOT NULL,
						Header TEXT NOT NULL,
						Priority INTEGER NOT NULL,
						IsArchived INTEGER,
						OrderNumber INTEGER)";

                connection.Execute(query);
            }
			
		}

		private static string GetConnectionString(string id = "Default")
		{
			return ConfigurationManager.ConnectionStrings[id].ConnectionString;
		}

		#endregion // Support Methods
	}
}
