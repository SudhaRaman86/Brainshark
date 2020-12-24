using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Infrastructure
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    public class Database
    {
        private readonly SqlConnection _connection;

        public Database()
        {
            // var connectionString = "Data Source=LOCALHOST;Initial Catalog=BrainWare;Integrated Security=SSPI";
            var mdf = @"C:\Brainshark\interview\BrainWare\Web\App_Data\BrainWare.mdf";
            var connectionString = $"Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=BrainWAre;Integrated Security=SSPI;AttachDBFilename={mdf}";

            _connection = new SqlConnection(connectionString);

        }

        public class QueryParameters
        {
            public string parameterName { get; set; }
            public string parameterType { get; set; }
            public string parameterValue { get; set; }
        }

        public DbDataReader ExecuteReader(string query, List<SqlParameter> paralist)
        {            
            var sqlQuery = new SqlCommand(query, _connection);
            if (paralist != null)
            {
                foreach (var p in paralist)
                {
                    sqlQuery.Parameters.AddWithValue(p.ParameterName, p.Value);
                }
            }
            try
            {
                _connection.Open();
                return sqlQuery.ExecuteReader();
            }
            catch (SqlException ex)
            {
                var message =  ex.Message != null? ex.Message.ToString()  :  ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString();
                //Log into Dblogger table
            }
            return null;
        }

        public int ExecuteNonQuery(string query)
        {
            var sqlQuery = new SqlCommand(query, _connection);
            _connection.Open();
            return sqlQuery.ExecuteNonQuery();
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

    }
}