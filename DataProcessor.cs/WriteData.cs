using Domain.Helpers;
using Domain.Models;
using Domain.Models.Comments;
using Domain.Models.Project;
using Domain.Models.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataProcessor.cs
{
    public class WriteData
    {

        private enum ObjTypes { Project, ProjectTask, SubTask, Link, Contributor, TaskContributor}

        public static Response WriteJson(string path, Action<string> setter, IEnumerable<Project> projects)
        {
            string output = JsonConvert.SerializeObject(projects, Formatting.Indented);
            using var stream = File.CreateText(path);
            stream.WriteLine(output);

            return new Response { Success = true};
        }

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

            setter("Writing projects");
            foreach (var project in projects)
            {
              
                stream.WriteLine(GetDetails(project));

                foreach (var comment in project.Comments)
                {
                    stream.Write(GetDetails(project));
                    stream.WriteLine(GetDetails(comment));
                }

                foreach (var contributor in project.Contributors)
                {
                    stream.Write(GetDetails(project));
                    stream.Write(SkipObject(ObjTypes.Link));
                    stream.WriteLine(GetDetails(contributor));
                }

                foreach (var task in project.Tasks)
                {
                    stream.Write(GetDetails(project));
                    stream.Write(SkipObject(ObjTypes.Link));
                    stream.Write(SkipObject(ObjTypes.Contributor));
                    stream.WriteLine(GetDetails(task));

                    foreach (var subTask in task.SubTasks)
                    {
                        stream.Write(GetDetails(project));
                        stream.Write(SkipObject(ObjTypes.Link));
                        stream.Write(SkipObject(ObjTypes.Contributor));
                        stream.Write(GetDetails(task));
                        stream.WriteLine(GetDetails(subTask));
                    }

                    foreach (var tContributor in task.Contributors)
                    {
                        stream.Write(GetDetails(project));
                        stream.Write(SkipObject(ObjTypes.Link));
                        stream.Write(SkipObject(ObjTypes.Contributor));
                        stream.Write(GetDetails(task));
                        stream.Write(SkipObject(ObjTypes.SubTask));
                        stream.WriteLine(GetDetails(tContributor));
                    }
                }
               
            }

            return new Response
            {
                Success = true
            };
        }

        private static string SkipObject(ObjTypes type)
        {
            return type switch
            {
                ObjTypes.Contributor => ",,,",
                ObjTypes.Project => ",,,,,,,",
                ObjTypes.ProjectTask => ",,,,,,,",
                ObjTypes.SubTask => ",,,,,,,",
                ObjTypes.Link => ",,,,,",
                ObjTypes.TaskContributor => ",,,",
                _ => string.Empty,
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

        private static string GetDetails<T>(T obj)
        {
   
            var props = PropertyHelper.GetProperties(obj, includeID: true, searchable: false);
            var output = new StringBuilder(string.Empty);
            for (int i = 0; i < props.Count; i++)
            {
                try
                {
                    output.Append($"{Verify(props[i].Value.ToString())},");
                }
                catch
                {
                    output.Append($"{string.Empty},");
                }
               
            }
            if (obj is Comment) output.Append($"{string.Empty},");
            return output.ToString();
        }

        private static string Verify(string data)
        {
            if (data.Contains("\""))
            {
                data = data.Replace("\"", "\"\"");
            }

            if (data.Contains(","))
            {
                data = String.Format("\"{0}\"", data);
            }

            if (data.Contains(Environment.NewLine))
            {
                data = String.Format("\"{0}\"", data);
            }

            return data;
        }

        public static string GetHeaders()
        {
            var headers = new StringBuilder(string.Empty);
            headers.Append(GetHeader(new Project()));
            headers.Append(GetHeader(new Link()));
            headers.Append(GetHeader(new Contributor()));
            headers.Append(GetHeader(new ProjectTask()));
            headers.Append(GetHeader(new SubTask()));
            headers.Append(GetHeader(new Contributor(), "Task"));
            return headers.ToString();
        }

        public static string GetHeader<T>(T obj, string prefix = null, string parentID = null)
        {
            var props = PropertyHelper.GetProperties(obj, exportable: true);
            var output = new StringBuilder(string.Empty);
            for (int i = 0; i < props.Count; i++)
            {
                output.Append($"{prefix ?? prefix}{typeof(T).Name}{props[i]},");
            }
            return output.ToString();
        }

    }
}
