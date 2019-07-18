using MA.Dao.Generate.VSIX.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Generate
{
    public class ConvertSQL
    {
        #region Enums
        public enum DatabaseType
        {
            MSSQL = 0,
            POSTGRESQL = 1,
            MYSQL = 2
        }
        #endregion

        #region Classes
        public class SplitData
        {
            #region Constructs
            public SplitData(string[] divide, int index, bool combineOther, StringSplitOptions option = StringSplitOptions.None)
            {
                this.Divide = divide;
                this.Index = index;
                this.CombineOther = combineOther;
                this.Option = option;
            }
            #endregion

            #region Variables
            public string[] Divide { get; private set; }
            public int Index { get; private set; }
            public bool CombineOther { get; private set; }
            public StringSplitOptions Option { get; set; }
            #endregion

            #region Methods
            public string Get(string data)
            {
                var array = data.Split(this.Divide, this.Option);
                return this.Index < 0 ?
                    (this.CombineOther ? (string.Join(this.Divide[0], array.Take(array.Length + this.Index).ToArray()) + this.Divide[0]) : "") + array[array.Length + this.Index] :
                    array[this.Index] + (this.CombineOther ? (this.Divide[0] + string.Join(this.Divide[0], array.Skip(this.Index + 1).ToArray())) : "");
            }

            public static void ScriptData(DatabaseType dbtype, string column, out string name, out string type, out int length)
            {
                switch (dbtype)
                {
                    case DatabaseType.MSSQL:
                        break;
                    case DatabaseType.POSTGRESQL:
                        {
                            column = column
                                .Replace(" bigint", " int8")
                                .Replace(" bigserial", " serial8")
                                .Replace(" bit varying", " varbit")

                                .Replace(" boolean", " bool")
                                .Replace(" double precision", " float8")
                                .Replace(" integer", " int")

                                .Replace(" numeric", " decimal")
                                .Replace(" real", " float4")
                                .Replace(" smallint", " int2")

                                .Replace(" smallserial", " serial2")
                                .Replace(" serial", " serial4")
                                .Replace(" character varying", " varchar")

                                .Replace(" character", " char");
                        }
                        break;
                    case DatabaseType.MYSQL:
                        break;
                    default:
                        break;
                }
                var split = column.Replace(", ", ",").Replace(" ,", ",").Split(' ', '=');
                if (split.Length == 1)
                {
                    split = new string[]
                        {
                        "",
                        split[0]
                        };
                }
                name = split[0];
                type = split[1];
                length = 0;
                if (type.Contains('('))
                {
                    var divideType = type.Split('(', ')');
                    type = divideType[0];
                    if (!divideType[1].Contains(','))
                        length = Convert.ToInt32(divideType[1]);
                    else
                    {
                        int.TryParse(divideType[1], out length);
                    }
                }
            }
            public static string Get(string data, SplitData[] splitDatas)
            {
                data = UpdateSplit(data);
                foreach (var splitData in splitDatas)
                    data = splitData.Get(data);
                return data.Trim();
            }
            public static string[] GetItems(string dataText, SplitData[] splitDatas, params string[] itemSplitString)
            {
                var data = Get(dataText, splitDatas);
                var items = data.Split(itemSplitString, StringSplitOptions.None);
                string[] results = new string[items.Length];
                for (int i = 0; i < items.Length; i++)
                    results[i] = items[i].Trim();
                return results;
            }
            #endregion
        }
        public class ActionParameter
        {
            #region Constructs
            public ActionParameter(string name, string type, int length)
            {
                this.Name = name.Trim().Replace("@", "").Replace("[", "").Replace("]", "").Replace("\"", "").Replace("'", "").Replace("`", "");
                this.Type = type.Trim().Replace("@", "").Replace("[", "").Replace("]", "").Replace("\"", "").Replace("'", "").Replace("`", "").ToLower().Replace("ı", "i");
                this.Length = length;
            }
            #endregion

            #region Variables
            public string Name { get; private set; }
            public string Type { get; private set; }
            public int Length { get; private set; }
            #endregion

            #region Methods
            #endregion
        }
        public class TableColumn
        {
            #region Constructs
            public TableColumn(string name, string type, int length, bool notNull, bool primaryKey = false)
            {
                this.Name = name.Trim().Replace("[", "").Replace("]", "").Replace("\"", "").Replace("'", "").Replace("`", "");
                this.Type = type.Trim().Replace("[", "").Replace("]", "").Replace("\"", "").Replace("'", "").Replace("`", "").ToLower().Replace("ı", "i");
                this.Length = length;
                this.NotNull = notNull;
                this.PrimaryKey = primaryKey;
            }
            #endregion

            #region Variables
            public string Name { get; private set; }
            public string Type { get; private set; }
            public int Length { get; private set; }
            public bool NotNull { get; private set; }
            public bool PrimaryKey { get; set; }
            #endregion
        }
        public class Table
        {
            #region Constructs
            public Table(string name, string safeName, TableColumn[] tableColumns, DatabaseType dbtype)
            {
                this.Name = name.Replace("\"", "\\\"");
                this.SafeName = safeName;
                this.TableColumns = tableColumns;
                this.Type = dbtype;
            }
            #endregion

            #region Variables
            public string Name { get; private set; }
            public string SafeName { get; private set; }
            public TableColumn[] TableColumns { get; set; }
            public DatabaseType Type { get; private set; }
            #endregion

            #region Methods
            public string GetCSharp()
            {
                string code = Resources.Table;

                switch (this.Type)
                {
                    case DatabaseType.MSSQL:
                    case DatabaseType.POSTGRESQL:
                    case DatabaseType.MYSQL:
                        {
                            code = code
                                .Replace("table_database_name", this.Name)
                                .Replace("table_name", this.SafeName)
                                .Replace("/*table_properties*/",
                                        string.Join(Environment.NewLine,
                                            this.TableColumns.Select(column =>
                                            {
                                                var propType = PropertyType(this.Type, column.Type, column.NotNull);
                                                return
                                                    Environment.NewLine +
                                                    propType + " _" + column.Name + ";" +
                                                    Environment.NewLine +
                                                    SqlAttributeToCsharpCode(this.Type, column.Type, propType, column.Length, column.PrimaryKey) +
                                                    Environment.NewLine +
                                                    "public " + propType + " " + column.Name + " { get { return _" + column.Name + "; } set { PropertSetAndActiveEvent(" + this.SafeName + ".PropertyName(o => o." + column.Name + "), ref _" + column.Name + ", value); } }";
                                            }).ToArray()));
                        }
                        break;
                    default:
                        break;
                }

                return code;
            }
            #endregion
        }
        public class Procedure
        {
            #region Constructs
            public Procedure(string name, string safeName, ActionParameter[] parameters, DatabaseType dbtype)
            {
                this.Name = name.Replace("\"", "\\\"");
                this.SafeName = safeName;
                this.Parameters = parameters;
                this.Type = dbtype;
            }
            #endregion

            #region Variables
            public string Name { get; private set; }
            public string SafeName { get; private set; }
            public ActionParameter[] Parameters { get; set; }
            public DatabaseType Type { get; private set; }
            #endregion

            #region Methods
            public string GetCSharp()
            {
                string code = Resources.Procedure;

                switch (this.Type)
                {
                    case DatabaseType.MSSQL:
                    case DatabaseType.MYSQL:
                        {
                            code = code
                                .Replace("procedure_database_name", this.Name)
                                .Replace("procedure_name", this.SafeName)
                                .Replace("/*procedure_properties*/",
                                        string.Join(Environment.NewLine,
                                            this.Parameters.Select(parameter =>
                                            {
                                                var propType = PropertyType(this.Type, parameter.Type, false);
                                                return
                                                    Environment.NewLine +
                                                    SqlAttributeToCsharpCode(this.Type, parameter.Type, propType, parameter.Length, false) +
                                                    Environment.NewLine +
                                                    "public " + propType + " " + parameter.Name + " { get; set; }";
                                            }).ToArray()));
                        }
                        break;
                    default:
                        break;
                }

                return code;
            }
            #endregion
        }
        public class Function
        {
            #region Constructs
            public Function(string name, string safeName, ActionParameter[] parameters, DatabaseType dbtype)
            {
                this.Name = name.Replace("\"", "\\\"");
                this.SafeName = safeName;
                this.Parameters = parameters;
                this.Type = dbtype;
            }
            #endregion

            #region Variables
            public string Name { get; private set; }
            public string SafeName { get; private set; }
            public ActionParameter[] Parameters { get; set; }
            public DatabaseType Type { get; private set; }
            #endregion
        }
        public class FunctionScalar : Function
        {
            #region Constructs
            public FunctionScalar(string name, string safeName, ActionParameter[] parameters, ActionParameter returnType, DatabaseType dbtype) : base(name, safeName, parameters, dbtype)
            {
                this.ReturnType = returnType;
            }
            #endregion

            #region Variables
            public ActionParameter ReturnType { get; set; }
            #endregion

            #region Methods
            public string GetCSharp()
            {
                string code = Resources.FunctionScalar;

                switch (this.Type)
                {
                    case DatabaseType.MSSQL:
                    case DatabaseType.POSTGRESQL:
                    case DatabaseType.MYSQL:
                        {
                            var databaseName_attrs = new string[this.Parameters.Length];
                            for (int i = 0; i < this.Parameters.Length; i++)
                                databaseName_attrs[i] = i == 0 ? "{parameter}" : "{argument" + (i - 1) + "}";
                            code = code
                                .Replace("function_database_name__attr", this.Name + "(" + string.Join(",", databaseName_attrs) + ")")
                                .Replace("function_database_name", this.Name + "(" + string.Join(",", this.Parameters.Select(parameter => "@" + parameter.Name)) + ")")
                                .Replace("function_name", this.SafeName)
                                .Replace("function_type", PropertyType(this.Type, this.ReturnType.Type, false))
                                .Replace("/*function_parameters*/",
                                    this.Parameters.Length == 0 ? "" : ", " + string.Join(", ",
                                            this.Parameters.Select(parameter =>
                                            {
                                                var propType = PropertyType(this.Type, parameter.Type, false);
                                                return propType + " " + parameter.Name;
                                            }).ToArray()))
                                .Replace("/*function_parameters_linq*/",
                                    string.Join(", ",
                                            this.Parameters.Select(parameter =>
                                            {
                                                var propType = PropertyType(this.Type, parameter.Type, false);
                                                return propType + " " + parameter.Name;
                                            }).ToArray()))
                                .Replace("/*function_sql_parameters*/",
                                    this.Parameters.Length == 0 ? "" : ", " + string.Join(", ",
                                            this.Parameters.Select(parameter =>
                                            {
                                                return "new MAEntityParameter(\"@" + parameter.Name + "\", " + parameter.Name + ")";
                                            }).ToArray()));
                        }
                        break;
                    default:
                        break;
                }

                return code;
            }
            #endregion
        }
        public class FunctionTable : Function
        {
            #region Constructs
            public FunctionTable(string name, string safeName, ActionParameter[] parameters, TableColumn[] tableColumns, DatabaseType dbtype) : base(name, safeName, parameters, dbtype)
            {
                this.TableColumns = tableColumns;
            }
            #endregion

            #region Variables
            public TableColumn[] TableColumns { get; set; }
            #endregion

            #region Methods
            public string GetCSharp()
            {
                string code = Resources.FunctionTable;

                switch (this.Type)
                {
                    case DatabaseType.MSSQL:
                    case DatabaseType.POSTGRESQL:
                    case DatabaseType.MYSQL:
                        {
                            var parameters = this.Parameters.Length == 0 ? "" : string.Join(", ",
                                            this.Parameters.Select(parameter =>
                                            {
                                                var propType = PropertyType(this.Type, parameter.Type, false);
                                                return propType + " " + parameter.Name;
                                            }).ToArray());
                            var sqlParameters = this.Parameters.Length == 0 ? "" : string.Join(", ",
                                            this.Parameters.Select(parameter =>
                                            {
                                                return "new MAEntityParameter(\"@" + parameter.Name + "\", " + parameter.Name + ")";
                                            }).ToArray());
                            code = code
                                .Replace("function_database_name", this.Name + "(" + string.Join(",", this.Parameters.Select(parameter => "@" + parameter.Name)) + ")")
                                .Replace("function_name", this.SafeName)
                                .Replace("/*function_parameters_sub_string*/", parameters)
                                .Replace("/*function_parameters*/",
                                    this.Parameters.Length == 0 ? "" : ", " + parameters)
                                .Replace("/*function_sql_parameters_sub_string*/", sqlParameters)
                                .Replace("/*function_sql_parameters*/",
                                    this.Parameters.Length == 0 ? "" : ", " + sqlParameters)
                                .Replace("/*function_properties*/",
                                        string.Join(Environment.NewLine,
                                            this.TableColumns.Select(column =>
                                            {
                                                var propType = PropertyType(this.Type, column.Type, column.NotNull);
                                                return
                                                    Environment.NewLine +
                                                    "[MADataColumn]" + Environment.NewLine +
                                                    "public " + propType + " " + column.Name + " { get; set; }";
                                            }).ToArray()));
                        }
                        break;
                    default:
                        break;
                }

                return code;
            }
            #endregion
        }
        public class View
        {
            #region Constructs
            public View(string name, string safeName, TableColumn[] tableColumns, DatabaseType dbtype)
            {
                this.TableColumns = tableColumns;
                this.Name = name.Replace("\"", "\\\"");
                this.SafeName = safeName;
                this.Type = dbtype;
            }
            #endregion

            #region Variables
            public string Name { get; private set; }
            public string SafeName { get; private set; }
            public TableColumn[] TableColumns { get; set; }
            public DatabaseType Type { get; private set; }
            #endregion

            #region Methods
            public string GetCSharp()
            {
                string code = Resources.FunctionTable;

                switch (this.Type)
                {
                    case DatabaseType.MSSQL:
                    case DatabaseType.POSTGRESQL:
                    case DatabaseType.MYSQL:
                        {
                            code = code
                                .Replace("parameter_values,", "")
                                .Replace("namespace MA.Database.Functions.Table", "namespace MA.Database.Views")
                                .Replace("function_database_name", this.Name)
                                .Replace("function_name", this.SafeName)
                                .Replace("/*function_parameters*/", "")
                                .Replace("/*function_sql_parameters*/", "")
                                .Replace("/*function_properties*/",
                                        string.Join(Environment.NewLine,
                                            this.TableColumns.Select(column =>
                                            {
                                                var propType = PropertyType(this.Type, column.Type, column.NotNull);
                                                return
                                                    Environment.NewLine +
                                                    "[MADataColumn]" + Environment.NewLine +
                                                    "public " + propType + " " + column.Name + " { get; set; }";
                                            }).ToArray()));
                        }
                        break;
                    default:
                        break;
                }

                return code;
            }
            #endregion
        }
        #endregion

        #region Variables
        public static string[] DivideParameters
        {
            get
            {
                var list = new string[]
                        {
                            "\n,",
                            "\r,",
                            "\n\r,",
                            "\r\n,",
                            ",\n",
                            ",\r",
                            ",\n\r",
                            ",\r\n",
                            "   ,",
                            "    ,",
                            "  ,"
                        };

                return list;
            }
        }
        #endregion

        #region Methods
        private static bool contraintControlColumn(string column)
        {
            return column.ToLower().Contains("constraint") || column.ToLower().Contains("constraınt") || column.ToLower().Contains("primary") || column.ToLower().Contains("prımary");
        }

        public static string UpdateSplit(string result)
        {
            string letters = "ABCDEFGHIJKLMNOPRSTUVYZWQXabcdefghijklmnoprstuvyzwqx";
            foreach (var letter in letters)
            {
                result = result.Replace(", " + letter, "," + Environment.NewLine + letter);
                result = result.Replace("," + letter, "," + Environment.NewLine + letter);
            }
            return result;
        }
        public static string UpdateVariableType(DatabaseType dbtype, string row)
        {
            switch (dbtype)
            {
                case DatabaseType.MSSQL:
                    break;
                case DatabaseType.POSTGRESQL:
                    {
                        row = row
                            .Replace("bigint", "int8")
                            .Replace("bigserial", "serial8")
                            .Replace("bit varying", "varbit")

                            .Replace("boolean", "bool")
                            .Replace("double precision", "float8")
                            .Replace("integer", "int")

                            .Replace("numeric", "decimal")
                            .Replace("real", "float4")
                            .Replace("smallint", "int2")

                            .Replace("smallserial", "serial2")
                            .Replace("serial", "serial4")
                            .Replace("character varying", "varchar")

                            .Replace("character", "char");
                    }
                    break;
                case DatabaseType.MYSQL:
                    break;
                default:
                    break;
            }
            return row;
        }
        public static string PropertyType(DatabaseType dbtype, string sqlType, bool isNotNull)
        {
            sqlType = UpdateVariableType(dbtype, sqlType);
            var propType = "";
            switch (dbtype)
            {
                case DatabaseType.MSSQL:
                    {
                        switch (sqlType)
                        {
                            case "char":
                            case "nchar":
                            case "varchar":
                            case "nvarchar":
                            case "text":
                            case "ntext":
                                propType = "string";
                                break;
                            case "bit":
                                propType = "bool";
                                break;
                            case "binary":
                            case "varbinary":
                                propType = "byte[]";
                                break;
                            case "tinyint":
                                propType = "byte";
                                break;
                            case "smallint":
                                propType = "short";
                                break;
                            case "int":
                                propType = "int";
                                break;
                            case "bigint":
                                propType = "long";
                                break;
                            case "float":
                            case "real":
                                propType = "float";
                                break;
                            case "money":
                            case "smallmoney":
                            case "decimal":
                                propType = "decimal";
                                break;
                            case "date":
                            case "datetime":
                            case "datetime2":
                            case "smalldatetime":
                                propType = "DateTime";
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DatabaseType.POSTGRESQL:
                    {
                        switch (sqlType)
                        {
                            case "bit":
                            case "varbit":
                            case "char":
                            case "varchar":
                            case "json":
                            case "text":
                            case "tsquery":
                            case "tsvector":
                            case "xml":
                                propType = "string";
                                break;
                            case "bool":
                                propType = "bool";
                                break;
                            case "tinyint":
                                propType = "byte";
                                break;
                            case "int2":
                                propType = "short";
                                break;
                            case "serial2":
                                propType = "ushort";
                                break;
                            case "int":
                            case "int4":
                                propType = "int";
                                break;
                            case "serial4":
                                propType = "uint";
                                break;
                            case "int8":
                                propType = "long";
                                break;
                            case "serial8":
                                propType = "ulong";
                                break;
                            case "float4":
                                propType = "float";
                                break;
                            case "float8":
                                propType = "double";
                                break;
                            case "money":
                            case "decimal":
                                propType = "decimal";
                                break;
                            case "date":
                            case "time":
                            case "timetz":
                            case "timestamp":
                            case "timestamptz":
                                propType = "DateTime";
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DatabaseType.MYSQL:
                    {
                        switch (sqlType)
                        {
                            case "char":
                            case "varchar":
                            case "text":
                            case "blob":
                            case "tinytext":
                            case "tinyblob":
                            case "mediumtext":
                            case "mediumblob":
                            case "longtext":
                            case "longblob":
                            case "enum":
                                propType = "string";
                                break;
                            case "bool":
                            case "boolean":
                                propType = "bool";
                                break;
                            case "bit":
                            case "tinyint":
                                propType = "byte";
                                break;
                            case "smallint":
                                propType = "short";
                                break;
                            case "mediumint":
                            case "int":
                            case "integer":
                            case "year":
                                propType = "int";
                                break;
                            case "bigint":
                                propType = "long";
                                break;
                            case "float":
                                propType = "float";
                                break;
                            case "double":
                            case "real":
                                propType = "double";
                                break;
                            case "decimal":
                            case "dec":
                            case "numeric":
                                propType = "decimal";
                                break;
                            case "date":
                            case "datetime":
                            case "timestamp":
                            case "time":
                                propType = "DateTime";
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (!isNotNull && propType != "string" && propType != "byte[]")
                propType += "?";

            return propType;
        }
        public static string SqlAttributeToCsharpCode(DatabaseType dbtype, string sqlType, string propertyType, int length, bool primaryKey)
        {
            bool fixedSize = false;
            switch (dbtype)
            {
                case DatabaseType.MSSQL:
                    {
                        switch (sqlType)
                        {
                            case "char":
                            case "nchar":
                                fixedSize = true;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DatabaseType.POSTGRESQL:
                    {
                        switch (sqlType)
                        {
                            case "char":
                            case "bit":
                                fixedSize = true;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case DatabaseType.MYSQL:
                    {
                        switch (sqlType)
                        {
                            case "char":
                                fixedSize = true;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
            string attrs = "";
            if (primaryKey)
                attrs += "[MADataKey()]";
            switch (propertyType)
            {
                case "string":
                    if (length > 0)
                    {
                        attrs += "[MaxLength(" + length + ", ErrorMessage = \"\")]";
                        if (fixedSize)
                            attrs += "[MinLength(" + length + ", ErrorMessage = \"\")]";
                    }
                    break;
                default:
                    break;
            }
            return attrs + "[MADataColumn]";
        }
        #endregion
    }
}
