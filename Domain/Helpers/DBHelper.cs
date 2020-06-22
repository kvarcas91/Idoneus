using Dapper;
using Domain.Helpers;
using Domain.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;

namespace Domain.Repository.Helpers
{
	internal class DBHelper
    {

		private static readonly object _lock = new object();

		public static Response Initialize(string connectionString)
		{

			FileHelper.CreateFolderIfNotExist("./Database");
			if (!FileHelper.FileExists("./Database/db.db"))
			{
				//FileHelper.CreateDB("./Database/db.db");
				var dbCreateResponse = NotifyDBMissing(connectionString);
				if (!dbCreateResponse.Success)
				{
					return dbCreateResponse;
				}
			}

			FileHelper.CreateFolderIfNotExist("./Projects");

			return new Response { Success = true };
		}

		private static Response NotifyDBMissing(string connectionString)
		{
			try
			{
				CreateTablesIfNotExist(connectionString);
				return new Response { Success = true };
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.StackTrace);
				return new Response
				{
					Success = false,
					Message = e.Message
				};
			}
		}

		public static void CreateTablesIfNotExist(string connectionString)
		{
			lock (_lock)
			{
				using var connection = new SqliteConnection(connectionString);
				connection.Open();

				var query = @"CREATE TABLE projects (
								ID TEXT NOT NULL PRIMARY KEY,
								SubmitionDate TEXT NOT NULL,
								Content TEXT NOT NULL,
								DueDate TEXT NOT NULL,
								Header TEXT NOT NULL,
								Priority INTEGER NOT NULL,
								Status INTEGER,
								OrderNumber INTEGER);

							CREATE TABLE comments (
								ID TEXT NOT NULL PRIMARY KEY,
								ProjectID TEXT,
								Content TEXT NOT NULL,
								SubmitionDate TEXT NOT NULL,
								FOREIGN KEY(ProjectID) REFERENCES projects(ID));

							CREATE TABLE links (
								ID TEXT NOT NULL PRIMARY KEY,
								ProjectID TEXT,
								Content TEXT NOT NULL,
								SubmitionDate TEXT NOT NULL,
								FOREIGN KEY(ProjectID) REFERENCES projects(ID));

							CREATE TABLE tasks (
								ID TEXT NOT NULL PRIMARY KEY,
								ParentID TEXT,
								Content TEXT NOT NULL,
								Priority INTEGER NOT NULL,
								DueDate TEXT NOT NULL,
								IsCompleted INTEGER NOT NULL,
								OrderNumber INTEGER,
								FOREIGN KEY(ParentID) REFERENCES projects(ID));

							CREATE TABLE td_tasks (
								ID TEXT NOT NULL PRIMARY KEY,
								Content TEXT NOT NULL,
								IsCompleted INTEGER NOT NULL,
								SubmitionDate TEXT NOT NULL);

							CREATE TABLE repetetive_tasks (
								ID TEXT NOT NULL PRIMARY KEY,
								Content TEXT NOT NULL,
								IsActive INTEGER);


							CREATE TABLE contributors ( 
                              Login TEXT NOT NULL,
                              FirstName TEXT NOT NULL,
                              LastName TEXT NOT NULL);

                            CREATE TABLE project_contributors (
                                projectID TEXT NOT NULL,
	                            contributorID TEXT NOT NULL,
	                            FOREIGN KEY(contributorID) REFERENCES contributors(ID),
	                            FOREIGN KEY(projectID) REFERENCES projects(ID));

							CREATE TABLE subtasks(
								ID TEXT NOT NULL PRIMARY KEY,
								ParentID TEXT,
								Content TEXT NOT NULL,
								Priority INTEGER NOT NULL,
								DueDate TEXT NOT NULL,
								IsCompleted INTEGER NOT NULL,
								OrderNumber INTEGER,
								FOREIGN KEY(ParentID) REFERENCES tasks(ID));

                            CREATE TABLE task_contributors(
                                taskID TEXT NOT NULL,
	                            contributorID TEXT NOT NULL,
	                            FOREIGN KEY(taskID) REFERENCES tasks(ID),
	                            FOREIGN KEY(contributorID) REFERENCES contributors(ID));";




				connection.Execute(query);
			}

		}

	}
}
