using System.Data;
using HelperLibrary;
using Microsoft.Data.SqlClient;

namespace MusicBrainzDataAcessLibrary
{
    public class DBAccessBase
    {
        protected string _connectionString = ConfigHelper.GetConnectionString();

        protected DataTable GetQueryResult(string sql)
        {
            DataTable output = new();


            using var connection = new SqlConnection(_connectionString);

            connection.Open();


            using SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = sql;


            SqlDataReader? reader = cmd.ExecuteReader();

            output.Load(reader);

            return output;
        }

    }
}
