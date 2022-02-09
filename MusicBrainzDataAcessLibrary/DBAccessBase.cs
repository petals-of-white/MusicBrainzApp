using System.Data;
using HelperLibrary;
using HelperLibrary.Logging;
using Microsoft.Data.SqlClient;

namespace MusicBrainzDataAcessLibrary
{
    public class DBAccessBase
    {
        protected LoggerBase _logger = new FileLoggerFactory("musicbrainz.log").CreateLogger();

        protected string _connectionString = ConfigHelper.GetConnectionString();

        protected DataTable GetQueryResult(string sql)
        {
            DataTable output = new();

            try
            {

                using var connection = new SqlConnection(_connectionString);

                connection.Open();


                using SqlCommand cmd = new SqlCommand();

                cmd.Connection = connection;
                cmd.CommandText = sql;


                SqlDataReader? reader = cmd.ExecuteReader();

                output.Load(reader);

                return output;
            }

            catch (ArgumentException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }

            catch (SqlException ex)
            {
                _logger.Log(ex.ToString());
                throw;
            }
            return output;

        }

    }