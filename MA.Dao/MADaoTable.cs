using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using MA.Dao.Attributes;
using MA.Dal;
using System.Linq.Expressions;

namespace MA.Dao
{
    public abstract class MADaoTable<TModel> : MADaoBase<TModel> where TModel : MADaoTable<TModel>
    {
        #region Constructs
        #endregion

        #region Variables
        HashSet<string> changingColumns = new HashSet<string>();
        public HashSet<string> ChangingColumns
        {
            get
            {
                return changingColumns;
            }
            set
            {
                changingColumns = value;
            }
        }

        bool loaded = true;
        public bool Loaded
        {
            get
            {
                return loaded;
            }
            set
            {
                loaded = value;
            }
        }
        
        public static string TableName
        {
            get
            {
                return TypeObjectName;
            }
        }
        #endregion

        #region Methods
        protected void PropertSetAndActiveEvent<TProperty>(string propName, ref TProperty propValue, TProperty value)
        {
            propValue = value;
            if (Loaded)
                this.ChangingColumns.Add(propName);
        }

        protected static List<TModel> SelectProtected(
            MAData.Connection connection,
            MAData.Transaction transaction,
            string commandText,
            params MAData.Parameter[] parameters
            )
        {
            using (MAData.Command command = new MAData.Command(connection, commandText, parameters))
            {
                if (transaction != null)
                    command.Transaction = transaction;
                return SelectProtected(command);
            }
        }
        protected static List<TModel> SelectProtected(
            MAData.Command command
            )
        {
            return SelectForModel(
                command,
                (model) => model.Loaded = false,
                (model) => model.Loaded = true
                );
        }

        protected int InsertProtected(MAData.Connection connection, MAData.Transaction transaction)
        {
            var changeColumns = ChangingColumns.ToArray();
            var columnNames = new string[changeColumns.Length];
            var parameterNames = new string[changeColumns.Length];
            var parameters = new MAData.Parameter[changeColumns.Length];

            for (int parameterIndex = 0; parameterIndex < changeColumns.Length; parameterIndex++)
            {
                var changeColumn = changeColumns[parameterIndex];
                var parameterName = MAData.Sql.Parameter(changeColumn);

                columnNames[parameterIndex] = changeColumn;
                parameterNames[parameterIndex] = parameterName;

                parameters[parameterIndex] =
                    new MAData.Parameter(
                        parameterName,
                        Properties.FirstOrDefault(prop => prop.Property.Name == changeColumn).Property.GetValue(this)
                        );
            }

            var sql = MAData.Sql.Insert(TableName, columnNames, parameterNames);

            MAData.Command command = new MAData.Command(connection, sql, parameters.ToArray());
            if (transaction != null)
                command.Transaction = transaction;
            var result = command.ExecuteNonQuery();
            this.ChangingColumns.Clear();
            return result;
        }

        protected int UpdateProtected(MAData.Connection connection, MAData.Transaction transaction)
        {
            var changeColumns = ChangingColumns.ToArray();
            MAData.Sql.NameAndValue[] updateColumns = new MAData.Sql.NameAndValue[changeColumns.Length];
            var parameters = new List<MAData.Parameter>();

            for (int parameterIndex = 0; parameterIndex < changeColumns.Length; parameterIndex++)
            {
                var changeColumn = changeColumns[parameterIndex];
                var parameterName = MAData.Sql.Parameter(changeColumn);

                updateColumns[parameterIndex] = new MAData.Sql.NameAndValue(changeColumn, parameterName);

                parameters.Add(new MAData.Parameter(
                        parameterName,
                        Properties.FirstOrDefault(prop => prop.Property.Name == changeColumn).Property.GetValue(this)
                        ));
            }

            var keyColumns = Properties.Where(prop => prop.AttributeKey != null).Select(prop => new
            {
                ColumnName = prop.ColumnName,
                ParameterName = MAData.Sql.Parameter(prop.ColumnName),
                Value = prop.Property.GetValue(this)
            }).ToArray();
            if (keyColumns.Length == 0)
                throw new Exception("Not found key columns!");

            var sql = MAData.Sql.Update(TableName, updateColumns, MAData.Sql.WhereAnd(keyColumns.Select(prop => MAData.Sql.Equal(prop.ColumnName, prop.ParameterName)).ToArray()));
            foreach (var keyColumn in keyColumns)
            {
                if (!parameters.Any(parameter => parameter.ParameterName == keyColumn.ParameterName))
                    parameters.Add(
                        new MAData.Parameter(keyColumn.ParameterName, keyColumn.Value)
                        );
            }

            MAData.Command command = new MAData.Command(connection, sql, parameters.ToArray());
            if (transaction != null)
                command.Transaction = transaction;
            var result = command.ExecuteNonQuery();
            this.ChangingColumns.Clear();
            return result;
        }

        protected int DeleteProtected(MAData.Connection connection, MAData.Transaction transaction)
        {
            var keyColumns = Properties.Where(prop => prop.AttributeKey != null).Select(prop => new
            {
                ColumnName = prop.ColumnName,
                ParameterName = MAData.Sql.Parameter(prop.ColumnName),
                Value = prop.Property.GetValue(this)
            }).ToArray();
            if (keyColumns.Length == 0)
                throw new Exception("Not found key columns!");

            var parameters = new MAData.Parameter[keyColumns.Length];
            var sql = MAData.Sql.Delete(TableName, MAData.Sql.WhereAnd(keyColumns.Select(prop => MAData.Sql.Equal(prop.ColumnName, prop.ParameterName)).ToArray()));
            var keyColumnIndex = 0;
            foreach (var keyColumn in keyColumns)
            {
                parameters[keyColumnIndex] = new MAData.Parameter(keyColumn.ParameterName, keyColumn.Value);
                keyColumnIndex++;
            }

            MAData.Command command = new MAData.Command(connection, sql, parameters.ToArray());
            if (transaction != null)
                command.Transaction = transaction;
            var result = command.ExecuteNonQuery();
            this.ChangingColumns.Clear();
            return result;
        }
        #endregion
    }
}
