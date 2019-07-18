using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dal.DatabaseSQLGenerator
{
    public abstract class MADataSQLGenerator_POSTGRESQL : MADataSQLGenerator
    {
        #region Variables
        /// <summary>
        /// AllColumns = "*"
        /// <para></para>
        /// Default SELECT all column parameter
        /// </summary>
        public static readonly string[] AllColumns = new string[] { "*" };
        /// <summary>
        /// AllCount = "COUNT(*)"
        /// <para></para>
        /// Default SELECT COUNT parameter
        /// </summary>
        public const string AllCount = "COUNT(*)";
        public const string NULL = "NULL";

        #region Data Types
        public const string Byte = "tinyint";
        public const string Int16 = "smallint";
        public const string Int32 = "int";
        public const string Int64 = "bigint";
        public const string Decimal = "decimal";
        public const string String = "varchar(max)";
        public const string Date = "date";
        public const string DateTime = "datetime";
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// return "\"" + name + "\""
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ObjectName(string name)
        {
            return "\"" + name + "\"";
        }

        /// <summary>
        /// return "@" + name
        /// </summary>
        /// <param name="name">Parameter Name</param>
        /// <returns></returns>
        public static string Parameter(string name)
        {
            return "@" + name;
        }
        /// <summary>
        /// return "(" + string.Join(" AND ", whereCommands) + ")"
        /// </summary>
        /// <param name="whereCommands">Where Commands</param>
        /// <returns></returns>
        public static string WhereAnd(params string[] whereCommands)
        {
            if (whereCommands.Length == 0)
                return "";
            return "(" + string.Join(" AND ", whereCommands) + ")";
        }
        /// <summary>
        /// return "(" + string.Join(" OR ", whereCommands) + ")"
        /// </summary>
        /// <param name="whereCommands">Where Commands</param>
        /// <returns></returns>
        public static string WhereOr(params string[] whereCommands)
        {
            if (whereCommands.Length == 0)
                return "";
            return "(" + string.Join(" OR ", whereCommands) + ")";
        }
        /// <summary>
        /// return name1 + "&lt;" + name2
        /// </summary>
        /// <param name="name1">Left Value</param>
        /// <param name="name2">Right Value</param>
        /// <returns></returns>
        public static string LessThan(string name1, string name2)
        {
            return name1 + "<" + name2;
        }
        /// <summary>
        /// return name1 + "&lt;=" + name2
        /// </summary>
        /// <param name="name1">Left Value</param>
        /// <param name="name2">Right Value</param>
        /// <returns></returns>
        public static string LessThanOrEqual(string name1, string name2)
        {
            return name1 + "<=" + name2;
        }
        /// <summary>
        /// return name1 + "&gt;" + name2
        /// </summary>
        /// <param name="name1">Left Value</param>
        /// <param name="name2">Right Value</param>
        /// <returns></returns>
        public static string GreaterThan(string name1, string name2)
        {
            return name1 + ">" + name2;
        }
        /// <summary>
        /// return name1 + "&gt;=" + name2
        /// </summary>
        /// <param name="name1">Left Value</param>
        /// <param name="name2">Right Value</param>
        /// <returns></returns>
        public static string GreaterThanOrEqual(string name1, string name2)
        {
            return name1 + ">=" + name2;
        }
        /// <summary>
        /// return name1 + " IS NULL"
        /// </summary>
        /// <param name="name">Null Value</param>
        /// <returns></returns>
        public static string IsNull(string name)
        {
            return name + " IS NULL";
        }
        /// <summary>
        /// return name + " IS NOT NULL"
        /// </summary>
        /// <param name="name">Null Value</param>
        /// <returns></returns>
        public static string IsNotNull(string name)
        {
            return name + " IS NOT NULL";
        }
        /// <summary>
        /// return name1 + "=" + name2
        /// </summary>
        /// <param name="name1">Left Value</param>
        /// <param name="name2">Right Value</param>
        /// <returns></returns>
        public static string Equal(string name1, string name2)
        {
            return name1 + "=" + name2;
        }
        /// <summary>
        /// return name1 + "&lt;&gt;" + name2
        /// </summary>
        /// <param name="name1">Left Value</param>
        /// <param name="name2">Right Value</param>
        /// <returns></returns>
        public static string NotEqual(string name1, string name2)
        {
            return name1 + "<>" + name2;
        }
        /// <summary>
        /// return "CAST(" + data + " AS " + type + ")"
        /// </summary>
        /// <param name="data">Value</param>
        /// <param name="type">Target Type</param>
        /// <returns></returns>
        public static string Cast(string data, string type)
        {
            return "CAST(" + data + " AS " + type + ")";
        }
        /// <summary>
        /// return "COUNT(" + name + ")"
        /// </summary>
        /// <param name="name">Value</param>
        /// <returns></returns>
        public static string Count(string name)
        {
            return "COUNT(" + name + ")";
        }
        /// <summary>
        /// return "UPPER(" + data + ")"
        /// </summary>
        /// <param name="data">Value</param>
        /// <returns></returns>
        public static string Upper(string data)
        {
            return "UPPER(" + data + ")";
        }
        /// <summary>
        /// return "LOWER(" + data + ")"
        /// </summary>
        /// <param name="data">Value</param>
        /// <returns></returns>
        public static string Lower(string data)
        {
            return "LOWER(" + data + ")";
        }
        /// <summary>
        /// return "TRIM(from " + data + ")"
        /// </summary>
        /// <param name="data">Value</param>
        /// <returns></returns>
        public static string Trim(string data)
        {
            return "TRIM(from " + data + ")";
        }
        /// <summary>
        /// return "SUM(" + name + ")"
        /// </summary>
        /// <param name="name">Value</param>
        /// <returns></returns>
        public static string Sum(string name)
        {
            return "SUM(" + name + ")";
        }
        /// <summary>
        /// return "MAX(" + name + ")"
        /// </summary>
        /// <param name="name">Value</param>
        /// <returns></returns>
        public static string Max(string name)
        {
            return "MAX(" + name + ")";
        }
        /// <summary>
        /// return "MIN(" + name + ")"
        /// </summary>
        /// <param name="name">Value</param>
        /// <returns></returns>
        public static string Min(string name)
        {
            return "MIN(" + name + ")";
        }
        /// <summary>
        /// return "length(" + name + ")"
        /// </summary>
        /// <param name="name">Value</param>
        /// <returns></returns>
        public static string Length(string name)
        {
            return "length(" + name + ")";
        }
        /// <summary>
        /// return "date_part('year', " + name + ")"
        /// </summary>
        /// <param name="name">Value</param>
        /// <returns></returns>
        public static string Year(string name)
        {
            return "date_part('year', " + name + ")";
        }
        /// <summary>
        /// return "date_part('month', " + name + ")"
        /// </summary>
        /// <param name="name">Value</param>
        /// <returns></returns>
        public static string Month(string name)
        {
            return "date_part('month', " + name + ")";
        }
        /// <summary>
        /// return "date_part('day', " + name + ")"
        /// </summary>
        /// <param name="name">Value</param>
        /// <returns></returns>
        public static string Day(string name)
        {
            return "date_part('day', " + name + ")";
        }
        /// <summary>
        /// return name1 + " LIKE " + name2
        /// </summary>
        /// <param name="name1">Left Value</param>
        /// <param name="name2">Right Value</param>
        /// <returns></returns>
        public static string Like(string name1, string name2)
        {
            return name1 + " LIKE " + name2;
        }
        /// <summary>
        /// return name1 + " NOT LIKE " + name2
        /// </summary>
        /// <param name="name1">Left Value</param>
        /// <param name="name2">Right Value</param>
        /// <returns></returns>
        public static string NotLike(string name1, string name2)
        {
            return name1 + " NOT LIKE " + name2;
        }
        /// <summary>
        /// return string.Join("||", names)
        /// </summary>
        /// <param name="names">Combine Names</param>
        /// <returns></returns>
        public static string ConcatString(params string[] names)
        {
            return "(" + string.Join("||", names) + ")";
        }
        /// <summary>
        /// return string.Join("/", names)
        /// </summary>
        /// <param name="values">Combine Names</param>
        /// <returns></returns>
        public static string DivideValues(params string[] values)
        {
            return "(" + string.Join("/", values) + ")";
        }
        /// <summary>
        /// return string.Join("*", names)
        /// </summary>
        /// <param name="values">Combine Names</param>
        /// <returns></returns>
        public static string MultiplyValues(params string[] values)
        {
            return "(" + string.Join("*", values) + ")";
        }
        /// <summary>
        /// return string.Join("*", names)
        /// </summary>
        /// <param name="values">Combine Names</param>
        /// <returns></returns>
        public static string Modulo(params string[] values)
        {
            return "(" + string.Join("%", values) + ")";
        }
        /// <summary>
        /// return string.Join("-", names)
        /// </summary>
        /// <param name="values">Combine Names</param>
        /// <returns></returns>
        public static string SubtractValues(params string[] values)
        {
            return "(" + string.Join("-", values) + ")";
        }
        /// <summary>
        /// return "(CASE WHEN " + where + " THEN " + ifTrue + " ELSE " + ifFalse + " END)"
        /// </summary>
        /// <param name="where">Where Query</param>
        /// <param name="ifTrue">If True - Result</param>
        /// <param name="ifFalse">If False - Result</param>
        /// <returns></returns>
        public static string Case(string where, string ifTrue, string ifFalse)
        {
            return "(CASE WHEN " + where + " THEN " + ifTrue + " ELSE " + ifFalse + " END)";
        }
        /// <summary>
        /// return name + "(" + string.Join(",", parameters) + ")"
        /// </summary>
        /// <param name="name">Function Name</param>
        /// <param name="parameters">Function Parameter Values</param>
        /// <returns></returns>
        public static string Function(string name, string[] parameters)
        {
            return name + "(" + string.Join(",", parameters) + ")";
        }
        /// <summary>
        /// return string.Join(",", parameters.Select(parameter => parameter.Name + " " + (parameter.Asc ? "ASC" : "DESC")).ToArray())
        /// </summary>
        /// <param name="parameters">Order Name And Is Ascending</param>
        /// <returns></returns>
        public static string OrderByCommand(params NameAndOrder[] parameters)
        {
            return string.Join(",", parameters.Select(parameter => parameter.Name + " " + (parameter.Asc ? "ASC" : "DESC")).ToArray());
        }
        /// <summary>
        /// return "SELECT " + name + "(" + string.Join(",", parameters) + ")"
        /// </summary>
        /// <param name="name">Procedure Name</param>
        /// <param name="parameters">Procedures Parameter Values</param>
        /// <returns></returns>
        public static string ExecProcedure(string name, string[] parameters)
        {
            return "SELECT " + name + "(" + string.Join(",", parameters) + ")";
        }
        /// <summary>
        /// Selected Row by Row Index Range
        /// </summary>
        /// <param name="selectQuery">SELECT query or Table, View or Function Name</param>
        /// <param name="orderByCommand">Which is ordered by human</param>
        /// <param name="startRowIndex">Start Row Index(1, 2, ...)</param>
        /// <param name="endRowIndex">End Row Index(1, 2, ...)</param>
        /// <returns></returns>
        public static string SelectPage(string selectQuery, string orderByCommand, int? startRowIndex, int? endRowIndex)
        {
            List<string> whereQueries = new List<string>();
            if (startRowIndex != null)
                whereQueries.Add("ROW_NEXT >= " + startRowIndex);
            if (endRowIndex != null)
                whereQueries.Add("ROW_NEXT <= " + endRowIndex);

            return
                "SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY " + orderByCommand + ") AS ROW_NEXT " +
                "FROM (" + selectQuery + @") AS rtWithRowNext) AS rtResult" +
                (whereQueries.Count > 0 ? " WHERE " + string.Join(" AND ", whereQueries.ToArray()) : "") +
                " ORDER BY rtResult.ROW_NEXT";
        }
        /// <summary>
        /// Create SELECT query
        /// </summary>
        /// <param name="tableOrViewOrFunctionName">Table, View Or Function Name</param>
        /// <param name="columns">Column Names</param>
        /// <param name="whereCommand">
        /// Where Query
        /// <para></para>
        /// Example : YEAR(birth_date) = 2018
        /// </param>
        /// <param name="groupByColumns">
        /// Group Column Names
        /// <para></para>
        /// Example : name, surname
        /// </param>
        /// <param name="havingCommand">
        /// Having Column Names
        /// <para></para>
        /// Example : name = 'MUHAMMED'
        /// </param>
        /// <param name="orderByCommand">
        /// Order Column and ascending or descending
        /// <para></para>
        /// Example : name ASC, surname DESC
        /// </param>
        /// <param name="distinct">
        /// DISTINCT Enable = True | False
        /// </param>
        /// <param name="joins">
        /// INNER JOIN ..., LEFT JOIN ...
        /// </param>
        /// <returns></returns>
        public static string Select(
            string tableOrViewOrFunctionName,
            string[] columns,
            string whereCommand = null,
            string[] groupByColumns = null,
            string havingCommand = null,
            string orderByCommand = null,
            bool distinct = false,
            string[] joins = null)
        {
            string sql = "SELECT " + (distinct ? "DISTINCT " : "") + string.Join(",", columns) + " ";
            sql += "FROM " + tableOrViewOrFunctionName;
            if (joins != null && joins.Length > 0)
                sql += " " + string.Join(" ", joins);
            if (!string.IsNullOrEmpty(whereCommand))
                sql += " WHERE " + whereCommand;
            if (groupByColumns != null && groupByColumns.Length > 0)
                sql += " GROUP BY " + string.Join(",", groupByColumns);
            if (!string.IsNullOrEmpty(havingCommand))
                sql += " HAVING " + havingCommand;
            if (!string.IsNullOrEmpty(orderByCommand))
                sql += " ORDER BY " + orderByCommand;
            return sql;
        }
        /// <summary>
        /// return joinType + " JOIN " + tableOrViewOrFunctionName + " ON " + joinCode;
        /// </summary>
        /// <param name="joinType"></param>
        /// <param name="tableOrViewOrFunctionName"></param>
        /// <param name="joinCode"></param>
        /// <returns></returns>
        public static string Join(JoinType joinType, string tableOrViewOrFunctionName, string joinCode)
        {
            string type = "";
            switch (joinType)
            {
                case JoinType.Left:
                    type = "LEFT";
                    break;
                case JoinType.Right:
                    type = "RIGHT";
                    break;
                case JoinType.Inner:
                    type = "INNER";
                    break;
                case JoinType.Full:
                    type = "FULL";
                    break;
                default:
                    break;
            }

            return type + " JOIN " + tableOrViewOrFunctionName + " ON " + joinCode;
        }        
        /// <summary>
        /// return "SELECT " + objectName
        /// </summary>
        /// <param name="objectName">Object Name</param>
        /// <returns></returns>
        public static string SelectData(string objectName)
        {
            return "SELECT " + objectName;
        }
        /// <summary>
        /// INSERT table query
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="columns">Column Names</param>
        /// <param name="values">Column Values</param>
        /// <returns></returns>
        public static string Insert(string tableName, string[] columns, string[] values)
        {
            string sql = "INSERT INTO " + tableName + " ";
            if (columns != null && columns.Length > 0)
                sql += "(" + string.Join(",", columns) + ") ";
            sql += "VALUES (" + string.Join(",", values) + ")";
            return sql;
        }
        /// <summary>
        /// UPDATE table query
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="columnNameAndValue">Changeable Column Name and Value</param>
        /// <param name="whereCommand">
        /// Where Query
        /// <para></para>
        /// Example : YEAR(user_id) = '1234'
        /// </param>
        /// <returns></returns>
        public static string Update(string tableName, NameAndValue[] columnNameAndValue, string whereCommand)
        {
            string sql = "UPDATE " + tableName + " ";
            sql += "SET " + string.Join(",", columnNameAndValue.Select(item => item.Name + "=" + item.Value).ToArray());
            if (!string.IsNullOrEmpty(whereCommand))
                sql += " WHERE " + whereCommand;
            return sql;
        }
        /// <summary>
        /// DELETE table query
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="whereCommand">
        /// Where Query
        /// <para></para>
        /// Example : YEAR(user_id) = '1234'
        /// </param>
        /// <returns></returns>
        public static string Delete(string tableName, string whereCommand)
        {
            string sql = "DELETE FROM " + tableName;
            if (!string.IsNullOrEmpty(whereCommand))
                sql += " WHERE " + whereCommand;
            return sql;
        }
        #endregion
    }
}
