using System.Data;
using HelperLibrary;
using Microsoft.Data.SqlClient;

namespace MusicBrainzDataAcessLibrary
{
    public class DBAccessBase
    {
        protected string _connectionString = ConfigHelper.GetConnectionString();

        //protected IEnumerable<object[]> GetQueryResult(string sql)
        //{
        //    List<object []> records = new();

        //    using var connection = new SqlConnection(_connectionString);

        //    connection.Open();

        //    using SqlCommand cmd = new SqlCommand();

        //    cmd.Connection = connection;

        //    cmd.CommandText = sql;

        //    DataTable dataTable = new DataTable();
        //    SqlDataReader? reader = cmd.ExecuteReader();
        //    dataTable.Load(reader);


        //    var i = 0;
        //    while (reader.Read())
        //    {
        //        object [] row = new object [reader.FieldCount];
        //        reader.GetValues(row);


        //        records.Add(row);

        //    }


        //    return records;
        //}
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
