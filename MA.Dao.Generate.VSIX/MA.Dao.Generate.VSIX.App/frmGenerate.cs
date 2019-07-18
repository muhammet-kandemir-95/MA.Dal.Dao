using MA.Dao.Generate.VSIX.Models;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MA.Dao.Generate.VSIX.App
{
    public partial class frmGenerate : Form
    {
        public frmGenerate()
        {
            InitializeComponent();
        }

        public static string ConnectionStringsPath = "";
        public bool ForceClose = true;

        private void frmGenerate_Load(object sender, EventArgs e)
        {
            ConnectionStringsPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "connectionStrings.txt");
            if (File.Exists(ConnectionStringsPath))
            {
                var result = File.ReadAllText(ConnectionStringsPath).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).GroupBy(o => o).Select(o => o.Key).ToArray();
                tbConnectionString.AutoCompleteCustomSource.AddRange(result);
                File.WriteAllText(ConnectionStringsPath, string.Join(Environment.NewLine, result));
            }
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            try
            {
                var tables = new List<ConvertSQL.Table>();
                var procedures = new List<ConvertSQL.Procedure>();
                var functionsScalar = new List<ConvertSQL.FunctionScalar>();
                var functionsTable = new List<ConvertSQL.FunctionTable>();
                var views = new List<ConvertSQL.View>();

                string connectionString = tbConnectionString.Text;
                File.AppendAllText(ConnectionStringsPath, Environment.NewLine + connectionString);

                if (rbMSSQL.Checked)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();

                        #region Functions Table
                        using (SqlCommand command = new SqlCommand(@"
SELECT 
	SCHEMA_NAME(SO.schema_id) AS [SCHEMA],
	SO.name AS [NAME]
FROM sys.objects AS SO
WHERE SO.[TYPE] IN ('TF')", con))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    functionsTable.Add(
                                        new ConvertSQL.FunctionTable(
                                            "[" + reader["SCHEMA"].ToString() + "].[" + reader["NAME"].ToString() + "]",
                                            reader["NAME"].ToString(),
                                            new ConvertSQL.ActionParameter[0],
                                            new ConvertSQL.TableColumn[0],
                                            ConvertSQL.DatabaseType.MSSQL)
                                        );
                                }
                            }
                        }

                        foreach (var functionTable in functionsTable)
                        {
                            var parameters = new List<ConvertSQL.ActionParameter>();
                            using (SqlCommand command = new SqlCommand(@"
SELECT 
	P.name AS [NAME],
	TYPE_NAME(P.user_type_id) AS [DATA_TYPE],
	P.max_length AS [MAX_VALUE]
FROM sys.objects AS SO
INNER JOIN sys.parameters AS P ON SO.OBJECT_ID = P.OBJECT_ID
WHERE SO.[TYPE] IN ('TF') AND '[' + SCHEMA_NAME(SO.schema_id) + '].[' + SO.name + ']' = @functionName
ORDER BY  P.parameter_id", con))
                            {
                                command.Parameters.Add(new SqlParameter("@functionName", functionTable.Name));
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int length = 0;
                                        int.TryParse(reader["MAX_VALUE"].ToString(), out length);
                                        var parameter = new ConvertSQL.ActionParameter(
                                                reader["NAME"].ToString(),
                                                reader["DATA_TYPE"].ToString(),
                                                length);
                                        parameters.Add(parameter);
                                    }
                                }
                            }

                            var columns = new List<ConvertSQL.TableColumn>();
                            using (SqlCommand command = new SqlCommand(@"
SELECT
	C.name AS [NAME],
	TYPE_NAME(C.user_type_id) AS [DATA_TYPE],
	C.max_length AS [MAX_VALUE],
	C.is_nullable AS [IS_NULLABLE]
FROM sys.columns C
INNER JOIN sys.objects AS SO ON SO.object_id = C.object_id
WHERE '[' + SCHEMA_NAME(SO.schema_id) + '].[' + SO.name + ']' = @functionName
ORDER BY C.column_id", con))
                            {
                                command.Parameters.Add(new SqlParameter("@functionName", functionTable.Name));
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int length = 0;
                                        int.TryParse(reader["MAX_VALUE"].ToString(), out length);

                                        int isNullable = -1;
                                        var ctrlTryParse = int.TryParse(reader["IS_NULLABLE"].ToString(), out isNullable);
                                        if (!ctrlTryParse) isNullable = -1;

                                        columns.Add(
                                            new ConvertSQL.TableColumn(
                                                reader["NAME"].ToString(),
                                                reader["DATA_TYPE"].ToString(),
                                                length,
                                                isNullable == 0 || reader["IS_NULLABLE"].ToString().ToLower() == "false")
                                            );
                                    }
                                }
                            }

                            functionTable.Parameters = parameters.ToArray();
                            functionTable.TableColumns = columns.ToArray();
                        }
                        #endregion

                        #region Functions Scalar
                        using (SqlCommand command = new SqlCommand(@"
SELECT 
	SCHEMA_NAME(SO.schema_id) AS [SCHEMA],
	SO.name AS [NAME]
FROM sys.objects AS SO
WHERE SO.[TYPE] IN ('FN')", con))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    functionsScalar.Add(
                                        new ConvertSQL.FunctionScalar(
                                            "[" + reader["SCHEMA"].ToString() + "].[" + reader["NAME"].ToString() + "]",
                                            reader["NAME"].ToString(),
                                            new ConvertSQL.ActionParameter[0],
                                            new ConvertSQL.ActionParameter("", "", 0),
                                            ConvertSQL.DatabaseType.MSSQL)
                                        );
                                }
                            }
                        }

                        foreach (var functionScalar in functionsScalar)
                        {
                            var parameters = new List<ConvertSQL.ActionParameter>();
                            using (SqlCommand command = new SqlCommand(@"
SELECT 
	P.name AS [NAME],
	TYPE_NAME(P.user_type_id) AS [DATA_TYPE],
	P.max_length AS [MAX_VALUE]
FROM sys.objects AS SO
INNER JOIN sys.parameters AS P ON SO.OBJECT_ID = P.OBJECT_ID
WHERE SO.[TYPE] IN ('FN') AND '[' + SCHEMA_NAME(SO.schema_id) + '].[' + SO.name + ']' = @functionName
ORDER BY  P.parameter_id", con))
                            {
                                command.Parameters.Add(new SqlParameter("@functionName", functionScalar.Name));
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int length = 0;
                                        int.TryParse(reader["MAX_VALUE"].ToString(), out length);
                                        var parameter = new ConvertSQL.ActionParameter(
                                                reader["NAME"].ToString(),
                                                reader["DATA_TYPE"].ToString(),
                                                length);
                                        if (string.IsNullOrEmpty(parameter.Name))
                                            functionScalar.ReturnType = parameter;
                                        else
                                            parameters.Add(parameter);
                                    }
                                }
                            }

                            functionScalar.Parameters = parameters.ToArray();
                        }
                        #endregion

                        #region Procedures
                        using (SqlCommand command = new SqlCommand(@"
SELECT 
	SCHEMA_NAME(SO.schema_id) AS [SCHEMA],
	SO.name AS [NAME]
FROM sys.objects AS SO
WHERE SO.[TYPE] IN ('P')", con))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    procedures.Add(
                                        new ConvertSQL.Procedure(
                                            "[" + reader["SCHEMA"].ToString() + "].[" + reader["NAME"].ToString() + "]",
                                            reader["NAME"].ToString(),
                                            new ConvertSQL.ActionParameter[0],
                                            ConvertSQL.DatabaseType.MSSQL)
                                        );
                                }
                            }
                        }

                        foreach (var procedure in procedures)
                        {
                            var parameters = new List<ConvertSQL.ActionParameter>();
                            using (SqlCommand command = new SqlCommand(@"
SELECT 
	P.name AS [NAME],
	TYPE_NAME(P.user_type_id) AS [DATA_TYPE],
	P.max_length AS [MAX_VALUE]
FROM sys.objects AS SO
INNER JOIN sys.parameters AS P ON SO.OBJECT_ID = P.OBJECT_ID
WHERE SO.[TYPE] IN ('P') AND '[' + SCHEMA_NAME(SO.schema_id) + '].[' + SO.name + ']' = @procedureName
ORDER BY  P.parameter_id", con))
                            {
                                command.Parameters.Add(new SqlParameter("@procedureName", procedure.Name));
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int length = 0;
                                        int.TryParse(reader["MAX_VALUE"].ToString(), out length);
                                        parameters.Add(
                                            new ConvertSQL.ActionParameter(
                                                reader["NAME"].ToString(),
                                                reader["DATA_TYPE"].ToString(),
                                                length)
                                            );
                                    }
                                }
                            }

                            procedure.Parameters = parameters.ToArray();
                        }
                        #endregion

                        #region Tables
                        using (SqlCommand command = new SqlCommand(@"
SELECT 
    T.TABLE_SCHEMA AS [SCHEMA],
    T.TABLE_NAME AS [NAME]
FROM INFORMATION_SCHEMA.TABLES T
WHERE T.TABLE_TYPE = 'BASE TABLE'", con))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    tables.Add(
                                        new ConvertSQL.Table(
                                            "[" + reader["SCHEMA"].ToString() + "].[" + reader["NAME"].ToString() + "]",
                                            reader["NAME"].ToString(),
                                            new ConvertSQL.TableColumn[0],
                                            ConvertSQL.DatabaseType.MSSQL)
                                        );
                                }
                            }
                        }

                        foreach (var table in tables)
                        {
                            var columns = new List<ConvertSQL.TableColumn>();
                            using (SqlCommand command = new SqlCommand(@"
SELECT
	C.name AS [NAME],
	TYPE_NAME(C.user_type_id) AS [DATA_TYPE],
	C.max_length AS [MAX_VALUE],
	C.is_nullable AS [IS_NULLABLE],
	(
		CASE
		WHEN SUM(CONVERT(INT, I.is_primary_key)) IS NULL OR SUM(CONVERT(INT, I.is_primary_key)) = 0 THEN 0
		ELSE 1
		END
	) AS [IS_PRIMARY]
FROM sys.columns C
INNER JOIN sys.objects AS SO ON SO.object_id = C.object_id
LEFT JOIN sys.index_columns AS IC ON IC.column_id = C.column_id AND IC.object_id = C.object_id
LEFT JOIN sys.indexes I ON I.index_id = IC.index_id AND I.object_id = C.object_id
WHERE '[' + SCHEMA_NAME(SO.schema_id) + '].[' + SO.name + ']' = @tableName
GROUP BY C.column_id, C.name, C.user_type_id, C.max_length, C.is_nullable, I.is_primary_key
ORDER BY C.column_id", con))
                            {
                                command.Parameters.Add(new SqlParameter("@tableName", table.Name));
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {

                                        int length = 0;
                                        int.TryParse(reader["MAX_VALUE"].ToString(), out length);

                                        int isNullable = -1;
                                        var ctrlTryParse = int.TryParse(reader["IS_NULLABLE"].ToString(), out isNullable);
                                        if (!ctrlTryParse) isNullable = -1;

                                        int isPrimary = -1;
                                        ctrlTryParse = int.TryParse(reader["IS_PRIMARY"].ToString(), out isPrimary);
                                        if (!ctrlTryParse) isPrimary = -1;
                                        columns.Add(
                                            new ConvertSQL.TableColumn(
                                                reader["NAME"].ToString(),
                                                reader["DATA_TYPE"].ToString(),
                                                length,
                                                isNullable == 0 || reader["IS_NULLABLE"].ToString().ToLower() == "false",
                                                isPrimary == 1 || reader["IS_PRIMARY"].ToString().ToLower() == "true")
                                            );
                                    }
                                }
                            }

                            table.TableColumns = columns.ToArray();
                        }
                        #endregion

                        #region Views
                        using (SqlCommand command = new SqlCommand(@"
SELECT 
    T.TABLE_SCHEMA AS [SCHEMA],
    T.TABLE_NAME AS [NAME]
FROM INFORMATION_SCHEMA.VIEWS T", con))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    views.Add(
                                        new ConvertSQL.View(
                                            "[" + reader["SCHEMA"].ToString() + "].[" + reader["NAME"].ToString() + "]",
                                            reader["NAME"].ToString(),
                                            new ConvertSQL.TableColumn[0],
                                            ConvertSQL.DatabaseType.MSSQL)
                                        );
                                }
                            }
                        }

                        foreach (var view in views)
                        {
                            var columns = new List<ConvertSQL.TableColumn>();
                            using (SqlCommand command = new SqlCommand(@"
SELECT
	C.name AS [NAME],
	TYPE_NAME(C.user_type_id) AS [DATA_TYPE],
	C.max_length AS [MAX_VALUE],
	C.is_nullable AS [IS_NULLABLE]
FROM sys.columns C
INNER JOIN sys.objects AS SO ON SO.object_id = C.object_id
WHERE '[' + SCHEMA_NAME(SO.schema_id) + '].[' + SO.name + ']' = @viewName
ORDER BY C.column_id", con))
                            {
                                command.Parameters.Add(new SqlParameter("@viewName", view.Name));
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {

                                        int length = 0;
                                        int.TryParse(reader["MAX_VALUE"].ToString(), out length);

                                        int isNullable = -1;
                                        var ctrlTryParse = int.TryParse(reader["IS_NULLABLE"].ToString(), out isNullable);
                                        if (!ctrlTryParse) isNullable = -1;
                                        columns.Add(
                                            new ConvertSQL.TableColumn(
                                                reader["NAME"].ToString(),
                                                reader["DATA_TYPE"].ToString(),
                                                length,
                                                isNullable == 0 || reader["IS_NULLABLE"].ToString().ToLower() == "false")
                                            );
                                    }
                                }
                            }

                            view.TableColumns = columns.ToArray();
                        }
                        #endregion
                    }
                }
                else if (rbPOSTGRESQL.Checked)
                {
                    using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
                    {
                        con.Open();

                        #region Functions Table
                        using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT n.nspname AS schema_name
      ,p.proname AS function_name
FROM   (SELECT oid, * FROM pg_proc p WHERE NOT p.proisagg) p
JOIN   pg_namespace n ON n.oid = p.pronamespace
WHERE  n.nspname !~~ 'pg_%'
AND    n.nspname <> 'information_schema'
AND pg_catalog.pg_get_function_result(p.oid) LIKE 'TABLE(%'", con))
                        {
                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    functionsTable.Add(
                                        new ConvertSQL.FunctionTable(
                                            "\"" + reader["schema_name"].ToString() + "\".\"" + reader["function_name"].ToString() + "\"",
                                            reader["function_name"].ToString(),
                                            new ConvertSQL.ActionParameter[0],
                                            new ConvertSQL.TableColumn[0],
                                            ConvertSQL.DatabaseType.POSTGRESQL)
                                        );
                                }
                            }
                        }

                        foreach (var functionTable in functionsTable)
                        {
                            var parameters = new List<ConvertSQL.ActionParameter>();
                            using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT pg_catalog.pg_get_function_identity_arguments(p.oid) AS stmt
FROM   pg_catalog.pg_proc p
JOIN   pg_catalog.pg_namespace n ON n.oid = p.pronamespace
WHERE  '\""' || n.nspname || '\"".\""' || p.proname || '\""' = @functionName
ORDER BY 1;", con))
                            {
                                command.Parameters.Add(new NpgsqlParameter("@functionName", functionTable.Name));
                                using (NpgsqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var divide = reader["stmt"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(o =>
                                        {
                                            string parameterName, parameterType;
                                            int parameterLength;
                                            ConvertSQL.SplitData.ScriptData(ConvertSQL.DatabaseType.POSTGRESQL, o.Replace("'", "").Trim(), out parameterName, out parameterType, out parameterLength);
                                            return new ConvertSQL.ActionParameter(parameterName, parameterType, parameterLength);
                                        });

                                        parameters.AddRange(divide);
                                    }
                                }
                            }

                            var columns = new List<ConvertSQL.TableColumn>();
                            using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT 
  pg_catalog.pg_get_function_result(p.oid) as ""RESULT_DATA""
FROM pg_catalog.pg_proc p
     LEFT JOIN pg_catalog.pg_namespace n ON n.oid = p.pronamespace
WHERE pg_catalog.pg_function_is_visible(p.oid)
      AND n.nspname <> 'pg_catalog'
      AND n.nspname <> 'information_schema'
      AND '\""' || n.nspname || '\"".\""' || p.proname || '\""' = @functionName", con))
                            {
                                command.Parameters.Add(new NpgsqlParameter("@functionName", functionTable.Name));
                                using (NpgsqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var str = reader["RESULT_DATA"].ToString().Replace("TABLE(", "").Trim();
                                        var divide = str.Substring(0, str.Length - 1).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(o =>
                                        {
                                            string parameterName, parameterType;
                                            int parameterLength;
                                            ConvertSQL.SplitData.ScriptData(ConvertSQL.DatabaseType.POSTGRESQL, o.Replace("'", "").Trim(), out parameterName, out parameterType, out parameterLength);
                                            return new ConvertSQL.TableColumn(parameterName, parameterType, parameterLength, false);
                                        });

                                        columns.AddRange(divide);
                                    }
                                }
                            }

                            functionTable.Parameters = parameters.ToArray();
                            functionTable.TableColumns = columns.ToArray();
                        }
                        #endregion

                        #region Functions Scalar
                        using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT n.nspname AS schema_name
      ,p.proname AS function_name
     ,pg_catalog.pg_get_function_result(p.oid) AS return_type_func
FROM   (SELECT oid, * FROM pg_proc p WHERE NOT p.proisagg) p
JOIN   pg_namespace n ON n.oid = p.pronamespace
WHERE  n.nspname !~~ 'pg_%'
AND    n.nspname <> 'information_schema'
AND pg_catalog.pg_get_function_result(p.oid) NOT LIKE 'TABLE(%'", con))
                        {
                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    functionsScalar.Add(
                                        new ConvertSQL.FunctionScalar(
                                            "\"" + reader["schema_name"].ToString() + "\".\"" + reader["function_name"].ToString() + "\"",
                                            reader["function_name"].ToString(),
                                            new ConvertSQL.ActionParameter[0],
                                            new ConvertSQL.ActionParameter("", reader["return_type_func"].ToString(), 0),
                                            ConvertSQL.DatabaseType.POSTGRESQL)
                                        );
                                }
                            }
                        }

                        foreach (var functionScalar in functionsScalar)
                        {
                            var parameters = new List<ConvertSQL.ActionParameter>();
                            using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT pg_catalog.pg_get_function_identity_arguments(p.oid) AS stmt
FROM   pg_catalog.pg_proc p
JOIN   pg_catalog.pg_namespace n ON n.oid = p.pronamespace
WHERE  '\""' || n.nspname || '\"".\""' || p.proname || '\""' = @functionName
ORDER BY 1;", con))
                            {
                                command.Parameters.Add(new NpgsqlParameter("@functionName", functionScalar.Name));
                                using (NpgsqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var divide = reader["stmt"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(o =>
                                        {
                                            string parameterName, parameterType;
                                            int parameterLength;
                                            ConvertSQL.SplitData.ScriptData(ConvertSQL.DatabaseType.POSTGRESQL, o.Replace("'", "").Trim(), out parameterName, out parameterType, out parameterLength);
                                            return new ConvertSQL.ActionParameter(parameterName, parameterType, parameterLength);
                                        });

                                        parameters.AddRange(divide);
                                    }
                                }
                            }

                            functionScalar.Parameters = parameters.ToArray();
                        }
                        #endregion

                        #region Tables
                        using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT 
    T.TABLE_SCHEMA AS SCHEMA,
    T.TABLE_NAME AS NAME
FROM INFORMATION_SCHEMA.TABLES T
WHERE T.TABLE_TYPE = 'BASE TABLE' AND T.TABLE_SCHEMA != 'pg_catalog' AND T.TABLE_SCHEMA != 'information_schema'", con))
                        {
                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    tables.Add(
                                        new ConvertSQL.Table(
                                            "\"" + reader["SCHEMA"].ToString() + "\".\"" + reader["NAME"].ToString() + "\"",
                                            reader["NAME"].ToString(),
                                            new ConvertSQL.TableColumn[0],
                                            ConvertSQL.DatabaseType.POSTGRESQL)
                                        );
                                }
                            }
                        }

                        foreach (var table in tables)
                        {
                            var columns = new List<ConvertSQL.TableColumn>();

                            List<string> primaryKeys = new List<string>();

                            using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT
         kcu.column_name AS NAME
FROM
 information_schema.TABLES t
         LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                 ON tc.table_catalog = t.table_catalog
                 AND tc.table_schema = t.table_schema
                 AND tc.table_name = t.table_name
                 AND tc.constraint_type = 'PRIMARY KEY'
         LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                 ON kcu.table_catalog = tc.table_catalog
                 AND kcu.table_schema = tc.table_schema
                 AND kcu.table_name = tc.table_name
                 AND kcu.constraint_name = tc.constraint_name
WHERE   t.table_schema NOT IN ('pg_catalog', 'information_schema') AND kcu.column_name IS NOT NULL AND 
 '\""' || t.table_schema || '\"".\""' || t.table_name || '\""' = @tableName", con))
                            {
                                command.Parameters.Add(new NpgsqlParameter("@tableName", table.Name));
                                using (NpgsqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        primaryKeys.Add(reader["NAME"].ToString());
                                    }
                                }
                            }

                            using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT
	column_name AS NAME,
	data_type AS DATA_TYPE,
    character_maximum_length AS MAX_VALUE,
    is_nullable AS IS_NULLABLE
FROM
 information_schema.COLUMNS
WHERE
 '\""' || table_schema || '\"".\""' || table_name || '\""' = @tableName
ORDER BY ordinal_position", con))
                            {
                                command.Parameters.Add(new NpgsqlParameter("@tableName", table.Name));
                                using (NpgsqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {

                                        int length = 0;
                                        int.TryParse(reader["MAX_VALUE"].ToString(), out length);

                                        int isNullable = -1;
                                        var ctrlTryParse = int.TryParse(reader["IS_NULLABLE"].ToString(), out isNullable);
                                        if (!ctrlTryParse) isNullable = -1;

                                        var name = reader["NAME"].ToString();

                                        columns.Add(
                                            new ConvertSQL.TableColumn(
                                                name,
                                                reader["DATA_TYPE"].ToString(),
                                                length,
                                                isNullable == 0 || reader["IS_NULLABLE"].ToString().ToLower() == "false",
                                                primaryKeys.Any(o => o == name))
                                            );
                                    }
                                }
                            }

                            table.TableColumns = columns.ToArray();
                        }
                        #endregion

                        #region Views
                        using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT 
    T.TABLE_SCHEMA AS SCHEMA,
    T.TABLE_NAME AS NAME
FROM INFORMATION_SCHEMA.VIEWS T
WHERE T.TABLE_SCHEMA != 'pg_catalog' AND T.TABLE_SCHEMA != 'information_schema'", con))
                        {
                            using (NpgsqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    views.Add(
                                        new ConvertSQL.View(
                                            "\"" + reader["SCHEMA"].ToString() + "\".\"" + reader["NAME"].ToString() + "\"",
                                            reader["NAME"].ToString(),
                                            new ConvertSQL.TableColumn[0],
                                            ConvertSQL.DatabaseType.POSTGRESQL)
                                        );
                                }
                            }
                        }

                        foreach (var view in views)
                        {
                            var columns = new List<ConvertSQL.TableColumn>();
                            using (NpgsqlCommand command = new NpgsqlCommand(@"
SELECT
	column_name AS NAME,
	data_type AS DATA_TYPE,
    character_maximum_length AS MAX_VALUE,
    is_nullable AS IS_NULLABLE
FROM
 information_schema.COLUMNS
WHERE
 '\""' || table_schema || '\"".\""' || table_name || '\""' = @viewName
ORDER BY ordinal_position", con))
                            {
                                command.Parameters.Add(new NpgsqlParameter("@viewName", view.Name));
                                using (NpgsqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {

                                        int length = 0;
                                        int.TryParse(reader["MAX_VALUE"].ToString(), out length);

                                        int isNullable = -1;
                                        var ctrlTryParse = int.TryParse(reader["IS_NULLABLE"].ToString(), out isNullable);
                                        if (!ctrlTryParse) isNullable = -1;
                                        columns.Add(
                                            new ConvertSQL.TableColumn(
                                                reader["NAME"].ToString(),
                                                reader["DATA_TYPE"].ToString(),
                                                length,
                                                isNullable == 0 || reader["IS_NULLABLE"].ToString().ToLower() == "false")
                                            );
                                    }
                                }
                            }

                            view.TableColumns = columns.ToArray();
                        }
                        #endregion
                    }
                }
                else if (rbMYSQL.Checked)
                {
                    using (MySqlConnection con = new MySqlConnection(connectionString))
                    {
                        con.Open();

                        #region Tables
                        using (MySqlCommand command = new MySqlCommand(@"show tables;", con))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    tables.Add(
                                        new ConvertSQL.Table(
                                            "`" + reader[0].ToString() + "`",
                                            reader[0].ToString(),
                                            new ConvertSQL.TableColumn[0],
                                            ConvertSQL.DatabaseType.MYSQL)
                                        );
                                }
                            }
                        }

                        foreach (var table in tables)
                        {
                            var columns = new List<ConvertSQL.TableColumn>();
                            using (MySqlCommand command = new MySqlCommand(@"
SHOW COLUMNS FROM " + table.Name, con))
                            {
                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string parameterName, parameterType;
                                        int parameterLength;
                                        ConvertSQL.SplitData.ScriptData(ConvertSQL.DatabaseType.MYSQL, reader["Type"].ToString().Trim(), out parameterName, out parameterType, out parameterLength);

                                        columns.Add(
                                            new ConvertSQL.TableColumn(
                                                reader["Field"].ToString(),
                                                parameterType,
                                                parameterLength,
                                                reader["Null"].ToString() == "NO",
                                                reader["Key"].ToString() == "PRI")
                                            );
                                    }
                                }
                            }

                            table.TableColumns = columns.ToArray();
                        }
                        #endregion

                        #region Procedures
                        using (MySqlCommand command = new MySqlCommand(@"SHOW PROCEDURE STATUS;", con))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    procedures.Add(
                                        new ConvertSQL.Procedure(
                                            "`" + reader["Name"].ToString() + "`",
                                            reader["Name"].ToString(),
                                            new ConvertSQL.ActionParameter[0],
                                            ConvertSQL.DatabaseType.MYSQL)
                                        );
                                }
                            }
                        }

                        foreach (var procedure in procedures)
                        {
                            var parameters = new List<ConvertSQL.ActionParameter>();
                            using (MySqlCommand command = new MySqlCommand(@"
SELECT * 
FROM information_schema.parameters
WHERE SPECIFIC_NAME = @procedureName
ORDER BY ORDINAL_POSITION;", con))
                            {
                                command.Parameters.Add(new MySqlParameter("@procedureName", procedure.SafeName));
                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int length = 0;
                                        int.TryParse(reader["CHARACTER_MAXIMUM_LENGTH"].ToString(), out length);
                                        parameters.Add(
                                            new ConvertSQL.ActionParameter(
                                                reader["PARAMETER_NAME"].ToString(),
                                                reader["DATA_TYPE"].ToString(),
                                                length)
                                            );
                                    }
                                }
                            }

                            procedure.Parameters = parameters.ToArray();
                        }
                        #endregion

                        #region Functions Scalar
                        using (MySqlCommand command = new MySqlCommand(@"
SELECT * 
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'FUNCTION'", con))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int length = 0;
                                    int.TryParse(reader["CHARACTER_MAXIMUM_LENGTH"].ToString(), out length);

                                    functionsScalar.Add(
                                        new ConvertSQL.FunctionScalar(
                                            "`" + reader["ROUTINE_NAME"].ToString() + "`",
                                            reader["ROUTINE_NAME"].ToString(),
                                            new ConvertSQL.ActionParameter[0],
                                            new ConvertSQL.ActionParameter("", reader["DATA_TYPE"].ToString(), length),
                                            ConvertSQL.DatabaseType.MYSQL)
                                        );
                                }
                            }
                        }

                        foreach (var functionScalar in functionsScalar)
                        {
                            var parameters = new List<ConvertSQL.ActionParameter>();
                            using (MySqlCommand command = new MySqlCommand(@"
SELECT * 
FROM information_schema.parameters
WHERE SPECIFIC_NAME = @functionName AND ORDINAL_POSITION <> 0
ORDER BY ORDINAL_POSITION;", con))
                            {
                                command.Parameters.Add(new MySqlParameter("@functionName", functionScalar.Name));
                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int length = 0;
                                        int.TryParse(reader["MAX_VALUE"].ToString(), out length);
                                        var parameter = new ConvertSQL.ActionParameter(
                                                reader["NAME"].ToString(),
                                                reader["DATA_TYPE"].ToString(),
                                                length);
                                        parameters.Add(parameter);
                                    }
                                }
                            }

                            functionScalar.Parameters = parameters.ToArray();
                        }
                        #endregion

                        #region Views
                        using (MySqlCommand command = new MySqlCommand(@"
SHOW FULL TABLES WHERE TABLE_TYPE LIKE 'VIEW';", con))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    views.Add(
                                        new ConvertSQL.View(
                                            "`" + reader[0].ToString() + "`",
                                            reader[0].ToString(),
                                            new ConvertSQL.TableColumn[0],
                                            ConvertSQL.DatabaseType.MYSQL)
                                        );
                                }
                            }
                        }

                        foreach (var view in views)
                        {
                            var columns = new List<ConvertSQL.TableColumn>();
                            using (MySqlCommand command = new MySqlCommand(@"
SHOW COLUMNS FROM " + view.Name, con))
                            {
                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string parameterName, parameterType;
                                        int parameterLength;
                                        ConvertSQL.SplitData.ScriptData(ConvertSQL.DatabaseType.MYSQL, reader["Type"].ToString().Trim(), out parameterName, out parameterType, out parameterLength);

                                        columns.Add(
                                            new ConvertSQL.TableColumn(
                                                reader["Field"].ToString(),
                                                parameterType,
                                                parameterLength,
                                                reader["Null"].ToString() == "NO",
                                                reader["Key"].ToString() == "PRI")
                                            );
                                    }
                                }
                            }

                            view.TableColumns = columns.ToArray();
                        }
                        #endregion
                    }
                }

                new GenerateSQL()
                {
                    Tables = tables.Select(o => new GenerateSQLItem(o.SafeName, o.GetCSharp())).ToList(),
                    FunctionsScalar = functionsScalar.Select(o => new GenerateSQLItem(o.SafeName, o.GetCSharp())).ToList(),
                    FunctionsTable = functionsTable.Select(o => new GenerateSQLItem(o.SafeName, o.GetCSharp())).ToList(),
                    Procedures = procedures.Select(o => new GenerateSQLItem(o.SafeName, o.GetCSharp())).ToList(),
                    Views = views.Select(o => new GenerateSQLItem(o.SafeName, o.GetCSharp())).ToList()
                }.Save(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "generate.txt"));

                ForceClose = false;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void frmGenerate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ForceClose)
            {
                new GenerateSQL()
                {
                    Tables = new List<GenerateSQLItem>(),
                    FunctionsScalar = new List<GenerateSQLItem>(),
                    FunctionsTable = new List<GenerateSQLItem>(),
                    Procedures = new List<GenerateSQLItem>(),
                    Views = new List<GenerateSQLItem>(),
                }.Save(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "generate.txt"));
            }
        }
    }
}
