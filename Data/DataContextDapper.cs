using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data
{
    public class DataContextDapper
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
            _dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            return _dbConnection.Query<T>(sql);
        }

        public T? LoadDataSingle<T>(string sql)
        {
            return _dbConnection.QuerySingleOrDefault<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            return _dbConnection.Execute(sql) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            return _dbConnection.Execute(sql);
        }

        public bool ExecuteSqlWithParameters(string sql, DynamicParameters parameters)
        {
            return _dbConnection.Execute(sql, parameters) > 0; 
            // SqlCommand commandWithParams = new SqlCommand(sql); // we can use dynamic paramters and remove all the stuff about 
                                                                // and just set return _dbConnection.Execute(sql, parameters) > 0; 
            // foreach (SqlParameter parameter in parameters)
            // {
            //     commandWithParams.Parameters.Add(parameter);
            // }

            // SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            // dbConnection.Open();

            // commandWithParams.Connection = dbConnection;

            // int rowsAffected = commandWithParams.ExecuteNonQuery();

            // dbConnection.Close();

            // return rowsAffected > 0;
        }

        public IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters parameters)
        {
            return _dbConnection.Query<T>(sql, parameters);
        }

        public T? LoadDataSingleWithParameters<T>(string sql, DynamicParameters parameters)
        {
            return _dbConnection.QuerySingleOrDefault<T>(sql, parameters);
        }
    }
}