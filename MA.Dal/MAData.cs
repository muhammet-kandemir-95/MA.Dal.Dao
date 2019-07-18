using MA.Dal.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MA.Dal
{
    using MA.Dal.DatabaseSQLGenerator;
    using System.Data.SqlClient;
    using System.Linq.Expressions;

    /*
     * https://www.facebook.com/groups/gameprogramerhacker/
     * MUHAMMED KANDEMİR
     */

    //Edit generic types
    //Example SqlConnection, SqlCommand, SqlParameter, SqlTransaction, SqlDataReader, SqlDataAdapter
    //Example OleDbConnection, OleDbCommand, OleDbParameter, OleDbTransaction, OleDbDataReader, OleDbDataAdapter
    //Example OracleConnection, OracleCommand, OracleParameter, OracleTransaction, OracleDataReader, OracleDataAdapter
    //Example MySqlConnection, MySqlCommand, MySqlParameter, MySqlTransaction, MySqlDataReader, MySqlDataAdapter
    //Example NpgsqlConnection, NpgsqlCommand, NpgsqlParameter, NpgsqlTransaction, NpgsqlDataReader, NpgsqlDataAdapter
    public abstract class MAData : MADataGeneric<SqlConnection, SqlCommand, SqlParameter, SqlTransaction, SqlDataReader, SqlDataAdapter>
    {

        /*
         This class for MA.Dao
         Support databases : MSSQL, MYSQL, POSTGRESQL
         Examples : MADataSQLGenerator_MSSQL, MADataSQLGenerator_MYSQL, MADataSQLGenerator_POSTGRESQL
             */
        public abstract class Sql : MADataSQLGenerator_MSSQL
        {
        }
    }
}
