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

        public bool Delete<T>(IEnumerable<T> obj) where T : class, IEntity, new()
        {
            using var connection = GetConnection();
            var result = connection.Delete(obj);
            return result;
        }

        public bool Insert<T>(T obj) where T : class, IEntity, new()
        {
            using var connection = GetConnection();
            var props = PropertyHelper.GetProperties(obj, includeID: true, searchable: false);
            var query = QueryHelper.BuildInsertQuery(props, GetTableName<T>());
            var result = connection.Query<T>(query);

            return result != null;
        }

        public T Get<T>(string field, string param)
        {
            using var connection = new SqliteConnection(GetConnectionString());
            var query = $"SELECT * FROM contributors WHERE {field}='{param}'";
            var output = connection.QueryFirstOrDefault<T>(query);
            return output;
        }

        public int GetCount<T>()
        {
            using var connection = GetConnection();
            var query = $"SELECT COUNT(*) FROM {GetTableName<T>()}";
            return connection.QuerySingle<int>(query);
        }

        public int GetCount<T>(string condition)
        {
            using var connection = GetConnection();
            var query = $"SELECT COUNT(*) FROM {GetTableName<T>()} WHERE {condition}";
            return connection.QuerySingle<int>(query);

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
