﻿using Domain.Helpers;
using Domain.Models;
using Domain.Models.Base;
using Domain.Models.Project;
using Domain.Models.Tasks;
using Domain.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataProcessor.cs
{
    public class WriteData
    {
        public static Response Write(string path, Action<string> setter, IEnumerable<Project> projects)
        {

            var data = new List<string>();
            
            setter("Writing headers...");
            data.Add(GetHeaders());


            using var stream = File.CreateText(path);
            for (int i = 0; i < data.Count; i++)
            {
                stream.WriteLine(data[i]);
            }
            return new Response
            {
                Success = true
            };
        }

        private static string GetRowTemplate(Project project)
        {


            var stringBuilder = new StringBuilder();
            foreach (var task in project.Tasks)
            {
                foreach (var subtask in task.SubTasks)
                {

                }
            }

            return ";";
        }

        public static string GetHeaders()
        {
            var headers = new StringBuilder(string.Empty);
            headers.Append(GetHeader(new Project()));
            headers.Append(GetHeader(new ProjectTask(), "TaskParentID"));
            headers.Append(GetHeader(new SubTask(), "SubTaskParentID"));
            //headers.Append("\n");
            return headers.ToString();
        }


        public static string GetHeader<T>(T obj, string parentID = null)
        {
            var props = PropertyHelper.GetProperties(obj, exportable: true);
            var output = new StringBuilder(string.Empty);
            if (!string.IsNullOrEmpty(parentID)) output.Append($"{parentID}\t");
            for (int i = 0; i < props.Count; i++)
            {
                output.Append($"{typeof(T).Name}{props[i]}");
                output.Append(i + 1 >= props.Count ? "" : "\t");
            }
            return output.ToString();
        }


        //public static string GetHeader<T, P>(T obj, P parentObj, string middleTable = null)  where P : class where T : class, IEntity, new()
        //{
        //    var props = PropertyHelper.GetProperties(obj, exportable: true);
        //    var output = new StringBuilder(string.Empty);
        //    for (int i = 0; i < props.Count; i++)
        //    {
        //        output.Append($"{typeof(T).Name}{props[i]}");
        //        output.Append(i+1 >= props.Count ? (parentObj is IEntity ? GetParentID(new T(), (IEntity)parentObj, obj.ID, middleTable) : "\n") : "\t");
        //    }
        //    return output.ToString();
        //}

        private static string GetParentID<T, P>(T obj, P parent, int ID, string middleTable) where T : class, IEntity where P : class, IEntity
        {
            var repo = new ProjectRepository();
            var parentTable = string.Empty;
            if (parent is Project) parentTable = repo.GetTableName<Project>();
            if (parent is SubTask) parentTable = repo.GetTableName<SubTask>();
           
            var output = repo.GetParentID(ID, obj, parentTable, repo.GetTableName<T>(), middleTable, ("taskID", "ID"), ("ID", "projectID"));
            return "";
        }

       


    }
}