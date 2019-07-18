using MA.Dal;
using MA.Dao.Entity.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity
{
    public class MAEntity : IDisposable
    {
        #region Constructs
        public MAEntity(string connectionString) : this(connectionString, false)
        { }

        public MAEntity(string connectionString, bool enableTransaction)
        {
            this.Connection = new MAData.Connection(connectionString);
            this.EnableTransactionInternal = enableTransaction;
        }
        #endregion

        #region Variables
        public MAData.Connection Connection { get; private set; }
        public MAData.Transaction Transaction { get; private set; }
        internal bool EnableTransactionInternal = false;
        public bool EnableTransaction
        {
            get
            {
                return EnableTransactionInternal;
            }
        }
        #endregion

        #region Methods
        private void openConnection()
        {
            if (this.Connection.State != System.Data.ConnectionState.Open)
            {
                this.Connection.Open();
                if (this.EnableTransactionInternal)
                    this.Transaction = this.Connection.BeginTransaction();
            }
        }

        public T Run<T>(Func<T> func)
        {
            openConnection();
            return func();
        }
        public void Run(Action action)
        {
            Run<bool>(() =>
            {
                action();
                return true;
            });
        }

        #region Dao
        #region Table
        public int Insert<T>(MADaoTableEntity<T> table) where T : MADaoTableEntity<T>
        {
            return table.Insert(this);
        }

        public int Update<T>(MADaoTableEntity<T> table) where T : MADaoTableEntity<T>
        {
            return table.Update(this);
        }

        public int Delete<T>(MADaoTableEntity<T> table) where T : MADaoTableEntity<T>
        {
            return table.Delete(this);
        }

        public int Delete<T>(object[] keys) where T : MADaoTableEntity<T>
        {
            var obj = Activator.CreateInstance<T>();
            var index = 0;
            foreach (var prop in MADaoTableEntity<T>.Properties)
            {
                if (prop.AttributeKey != null)
                {
                    prop.Property.SetValue(obj, keys[index]);
                    index++;
                    if (index == keys.Length)
                        break;
                }
            }
            return obj.Delete(this);
        }
        public int Delete<T>(object key) where T : MADaoTableEntity<T>
        {
            return Delete<T>(
                new object[]
                {
                    key
                }
                );
        }

        public List<T> Select<T>(
            string command,
            params MAEntityParameter[] parameters) where T : MADaoTableEntity<T>
        {
            return MADaoTableEntity<T>.Select(this, command, parameters);
        }

        public static MAEntityLinqQueryFromName<T> Query<T>(
            Expression<Func<object, T>> expression,
            string command,
            params MAEntityParameter[] parameters)
        {
            int indexObject = 0;
            var safeObjectName = MAData.Sql.ObjectName("ma_before_query_first_sub");
            return new MAEntityLinqQueryFromName<T>(
                "(" + MAData.Sql.Select(
                    "(" + command + ") AS " + MAData.Sql.ObjectName("ma_before_query_first"),
                    expression.GetColumns(new List<MAEntityParameter>(), ref indexObject, onlyAliasName: true)
                    ) + ") AS " + safeObjectName,
                MAData.Sql.ObjectName("ma_before_query_first_sub")
                )
            {
                ParametersDefault = parameters
            };
        }
        #endregion

        #region Procedure
        public TResult Exec<TModel, TResult>(MADaoProcedureEntity<TModel, TResult> procedure) where TModel : MADaoProcedureEntity<TModel, TResult>
        {
            return procedure.Exec(this);
        }
        #endregion

        public virtual void Rollback()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Rollback();
                this.Transaction.Dispose();
                this.Transaction = this.Connection.BeginTransaction();
            }
        }

        public virtual void SaveChanges()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Commit();
                this.Transaction.Dispose();
                this.Transaction = this.Connection.BeginTransaction();
            }
        }
        #endregion

        #region MAData.Command

        #region CommandExec
        private T firstRowColumn<T>(string commandText, MAEntityParameter[] parameters, Func<MAData.Command, T> func)
        {
            return this.Run(
                () =>
                {
                    var command = new MAData.Command(this.Connection, commandText, parameters.Select(parameter => (MAData.Parameter)parameter).ToArray());
                    if (this.Transaction != null)
                        command.Transaction = this.Transaction;
                    return func(
                        command
                        );
                }
                );
        }

        public Nullable<bool> FirstBool(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn<Nullable<bool>>(commandText, parameters, (command) => command.GetFirstRowColumnBool());
        }

        public Nullable<char> FirstChar(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnChar());
        }

        public Nullable<byte> FirstByte(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnByte());
        }
        public Nullable<sbyte> FirstSByte(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnSByte());
        }

        public Nullable<short> FirstShort(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnShort());
        }
        public Nullable<ushort> FirstUShort(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnUShort());
        }

        public Nullable<int> FirstInt(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnInt());
        }
        public Nullable<uint> FirstUInt(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnUInt());
        }

        public Nullable<long> FirstLong(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnLong());
        }
        public Nullable<ulong> FirstULong(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnULong());
        }

        public Nullable<float> FirstFloat(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnFloat());
        }
        public Nullable<double> FirstDouble(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnDouble());
        }
        public Nullable<decimal> FirstDecimal(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnDecimal());
        }

        public Nullable<DateTime> FirstDateTime(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn(commandText, parameters, (command) => command.GetFirstRowColumnDateTime());
        }

        public T FirstEnum<T>(string commandText, params MAEntityParameter[] parameters)
        {
            return firstRowColumn<T>(commandText, parameters, (command) => command.GetFirstRowColumnEnum<T>());
        }

        internal static T First<T>(MAData.Command command)
        {
            object obj = null;
            var type = typeof(T);
            if (type.IsEnum)
            {
                obj = command.GetFirstRowColumnEnum<T>();
                return (T)obj;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    obj = command.GetFirstRowColumnBool();
                    break;
                case TypeCode.Char:
                    obj = command.GetFirstRowColumnChar();
                    break;
                case TypeCode.SByte:
                    obj = command.GetFirstRowColumnSByte();
                    break;
                case TypeCode.Byte:
                    obj = command.GetFirstRowColumnByte();
                    break;
                case TypeCode.Int16:
                    obj = command.GetFirstRowColumnShort();
                    break;
                case TypeCode.UInt16:
                    obj = command.GetFirstRowColumnUShort();
                    break;
                case TypeCode.Int32:
                    obj = command.GetFirstRowColumnInt();
                    break;
                case TypeCode.UInt32:
                    obj = command.GetFirstRowColumnUInt();
                    break;
                case TypeCode.Int64:
                    obj = command.GetFirstRowColumnLong();
                    break;
                case TypeCode.UInt64:
                    obj = command.GetFirstRowColumnULong();
                    break;
                case TypeCode.Single:
                    obj = command.GetFirstRowColumnFloat();
                    break;
                case TypeCode.Double:
                    obj = command.GetFirstRowColumnDouble();
                    break;
                case TypeCode.Decimal:
                    obj = command.GetFirstRowColumnDecimal();
                    break;
                case TypeCode.DateTime:
                    obj = command.GetFirstRowColumnDateTime();
                    break;
                case TypeCode.String:
                    obj = command.GetFirstRowColumnString();
                    break;
                default:
                    break;
            }

            return (T)obj;
        }
        #endregion

        public List<T> Reader<T>(
            string commandText,
            MAEntityParameter[] parameters
            )
        {
            return this.Run(
                () =>
                {
                    var command = new MAData.Command(this.Connection, commandText, parameters.ToMAData());
                    if (this.Transaction != null)
                        command.Transaction = this.Transaction;
                    return MADaoBase.SelectForResult<T>(
                        command,
                        (model) => { },
                        (model) => { }
                        );
                }
                );
        }
        public int ExecuteNonQuery(
            string commandText,
            MAEntityParameter[] parameters
            )
        {
            return this.Run(
                () =>
                {
                    var command = new MAData.Command(this.Connection, commandText, parameters.ToMAData());
                    if (this.Transaction != null)
                        command.Transaction = this.Transaction;
                    return command.ExecuteNonQuery();
                }
                );
        }

        #endregion

        public virtual void Dispose()
        {
            this.Connection.Dispose();
            if (this.Transaction != null)
                this.Transaction.Dispose();
        }
        #endregion
    }

    public static class MAEntityExtension
    {
        public static MAData.Parameter[] ToMAData(this IEnumerable<MAEntityParameter> parameters)
        {
            return parameters.Select(parameter => (MAData.Parameter)parameter).ToArray();
        }
        public static MAData.Sql.NameAndOrder[] ToMAData(this IEnumerable<MAEntityNameAndOrder> parameters)
        {
            return parameters.Select(parameter => (MAData.Sql.NameAndOrder)parameter).ToArray();
        }
    }

    public class MAEntityParameter : IDbDataParameter
    {
        #region Constructs
        public MAEntityParameter()
        {
            this._parameter = new MAData.Parameter();
        }

        public MAEntityParameter(string parameterName, object value)
        {
            this._parameter = new MAData.Parameter(parameterName, value);
        }
        #endregion

        #region Variables
        private MAData.Parameter _parameter { get; set; }
        #endregion

        #region Variables
        public byte Precision
        {
            get
            {
                return this._parameter.Precision;
            }
            set
            {
                this._parameter.Precision = value;
            }
        }
        public byte Scale
        {
            get
            {
                return this._parameter.Scale;
            }
            set
            {
                this._parameter.Scale = value;
            }
        }
        public int Size
        {
            get
            {
                return this._parameter.Size;
            }
            set
            {
                this._parameter.Size = value;
            }
        }
        public DbType DbType
        {
            get
            {
                return this._parameter.DbType;
            }
            set
            {
                this._parameter.DbType = value;
            }
        }
        public ParameterDirection Direction
        {
            get
            {
                return this._parameter.Direction;
            }
            set
            {
                this._parameter.Direction = value;
            }
        }

        public bool IsNullable
        {
            get
            {
                return this._parameter.IsNullable;
            }
        }

        public string ParameterName
        {
            get
            {
                return this._parameter.ParameterName;
            }
            set
            {
                this._parameter.ParameterName = value;
            }
        }
        public string SourceColumn
        {
            get
            {
                return this._parameter.SourceColumn;
            }
            set
            {
                this._parameter.SourceColumn = value;
            }
        }
        public DataRowVersion SourceVersion
        {
            get
            {
                return this._parameter.SourceVersion;
            }
            set
            {
                this._parameter.SourceVersion = value;
            }
        }
        public object Value
        {
            get
            {
                return this._parameter.Value;
            }
            set
            {
                this._parameter.Value = value ?? DBNull.Value;
            }
        }
        #endregion

        #region Methods
        #region Implicit
        public static implicit operator MAData.Parameter(MAEntityParameter parameter)
        {
            return new MAData.Parameter(parameter.ParameterName, parameter.Value);
        }
        #endregion
        #endregion
    }

    public class MAEntityNameAndOrder
    {
        #region Constructs
        public MAEntityNameAndOrder(string name, bool asc)
        {
            this._nameAndOrder = new MAData.Sql.NameAndOrder(name, asc);
        }
        #endregion

        #region Variables
        private MAData.Sql.NameAndOrder _nameAndOrder { get; set; }

        public string Name
        {
            get
            {
                return this._nameAndOrder.Name;
            }
            set
            {
                this._nameAndOrder.Name = value;
            }
        }
        public bool Asc
        {
            get
            {
                return this._nameAndOrder.Asc;
            }
            set
            {
                this._nameAndOrder.Asc = value;
            }
        }
        #endregion

        #region Methods
        #region Implicit
        public static implicit operator MAData.Sql.NameAndOrder(MAEntityNameAndOrder nameAndOrder)
        {
            return new MAData.Sql.NameAndOrder(nameAndOrder._nameAndOrder.Name, nameAndOrder._nameAndOrder.Asc);
        }
        #endregion
        #endregion
    }
}
