using Dapper;
using Dapper.Contrib.Extensions;
using Domain.Helpers;
using Domain.Models;
using Domain.Models.Base;
using Domain.Repository.Helpers;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;

namespace Domain.Repository.Base
{
    public abstract class BaseRepository
    {

        protected readonly string dateFormat = "yyyy-MM-dd";

        public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class, IEntity, new()
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

        public bool Update<T>(T obj) where T : class, IEntity, new()
        {
            using var connection = GetConnection();
            var result = connection.Update(obj);
            return result;
        }

        public bool UpdateAll<T>(IEnumerable<T> obj) where T : class, IEntity, new()
        {
            using var connection = GetConnection();
            var result = connection.Update(obj);
            return result;
        }

        public bool Delete<T>(T obj) where T : class, IEntity, new()
        {
            using var connection = GetConnection();
            var result = connection.Delete(obj);
            return result;
        }

        public bool Delete<T>(IEnumerable<T> obj) where T : class, IEntity, new()
        {
            using var connection = GetConnection();
            var result = connection.Delete(obj);
            return result;
        }

        public bool Delete(string query)
        {
            using var connection = GetConnection();
  
            var result = connection.Execute(query);
            return result != 0;
        }

        public bool Insert<T>(T obj, string table) where T : class, IEntity, new()
        {
            using var connection = GetConnection();
            var props = PropertyHelper.GetProperties(obj, includeID: true, searchable: false);
            var query = QueryHelper.BuildInsertQuery(props, table);
            var result = connection.Query<T>(query);

            return result != null;
        }

        public bool Insert(string query)
        {
            using var connection = GetConnection();
            var result = connection.Query(query);

            return result != null;
        }

        public T Get<T>(string field, string param, string table)
        {
            using var connection = new SqliteConnection(GetConnectionString());
            var query = $"SELECT * FROM {table} WHERE {field}='{param}'";
            var output = connection.QueryFirstOrDefault<T>(query);
            return output;
        }

        protected T Get<T>(string query)
        {
            using var connection = new SqliteConnection(GetConnectionString());
            var output = connection.QueryFirstOrDefault<T>(query);
            return output;
        }

        public int GetCount<T>(string table)
        {
            using var connection = GetConnection();
            var query = $"SELECT COUNT(*) FROM {table}";
            return connection.QuerySingle<int>(query);
        }

        public int GetCount<T>(string condition, string table)
        {
            using var connection = GetConnection();
            var query = $"SELECT COUNT(*) FROM {table} WHERE {condition}";
            return connection.QuerySingle<int>(query);

        }

        public dynamic GetScalar<Type>(string query)
        {
            using var connection = GetConnection();
            var output = connection.QueryFirstOrDefault(typeof(Type), query);
            return output;
        }

        #region Init

        private static string GetConnectionString()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = $".{Path.DirectorySeparatorChar}Database{Path.DirectorySeparatorChar}db.db"
            };
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
