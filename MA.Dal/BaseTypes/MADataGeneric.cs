using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MA.Dal.BaseTypes
{
    public abstract class MADataGeneric<TDbConnection, TDbCommand, TDbParameter, TDbTransaction, TDbDataReader, TDbDataAdapter> where TDbConnection : IDbConnection where TDbCommand : IDbCommand where TDbParameter : IDbDataParameter where TDbTransaction : IDbTransaction where TDbDataReader : IDataReader where TDbDataAdapter : IDbDataAdapter
    {
        public class DataAdapter : IDbDataAdapter, IMAGeneric<TDbDataAdapter>
        {
            #region Constructs
            public DataAdapter()
            {
                this.DataObject = Activator.CreateInstance<TDbDataAdapter>();
            }
            public DataAdapter(Command command) : this()
            {
                this.SelectCommand = command;
            }
            public DataAdapter(Connection connection, string commandText) : this(new Command(connection, commandText))
            {
            }
            #endregion

            #region Variables
            public TDbDataAdapter DataObject { get; private set; }

            IDbCommand IDbDataAdapter.SelectCommand
            {
                get
                {
                    return this.SelectCommand;
                }
                set
                {
                    this.SelectCommand = (Command)value;
                }
            }
            Command selectCommand = null;
            Command SelectCommand
            {
                get
                {
                    return this.selectCommand;
                }
                set
                {
                    this.selectCommand = value;
                    this.DataObject.SelectCommand = this.selectCommand.DataObject;
                }
            }

            IDbCommand IDbDataAdapter.InsertCommand
            {
                get
                {
                    return this.InsertCommand;
                }
                set
                {
                    this.InsertCommand = (Command)value;
                }
            }
            Command insertCommand = null;
            Command InsertCommand
            {
                get
                {
                    return this.insertCommand;
                }
                set
                {
                    this.insertCommand = value;
                    this.DataObject.InsertCommand = this.insertCommand.DataObject;
                }
            }

            IDbCommand IDbDataAdapter.DeleteCommand
            {
                get
                {
                    return this.DeleteCommand;
                }
                set
                {
                    this.DeleteCommand = (Command)value;
                }
            }
            Command deleteCommand = null;
            Command DeleteCommand
            {
                get
                {
                    return this.deleteCommand;
                }
                set
                {
                    this.deleteCommand = value;
                    this.DataObject.DeleteCommand = this.deleteCommand.DataObject;
                }
            }

            IDbCommand IDbDataAdapter.UpdateCommand
            {
                get
                {
                    return this.UpdateCommand;
                }
                set
                {
                    this.UpdateCommand = (Command)value;
                }
            }
            Command updateCommand = null;
            Command UpdateCommand
            {
                get
                {
                    return this.updateCommand;
                }
                set
                {
                    this.updateCommand = value;
                    this.DataObject.UpdateCommand = this.updateCommand.DataObject;
                }
            }

            public MissingMappingAction MissingMappingAction
            {
                get
                {
                    return this.DataObject.MissingMappingAction;
                }
                set
                {
                    this.DataObject.MissingMappingAction = value;
                }
            }
            public MissingSchemaAction MissingSchemaAction
            {
                get
                {
                    return this.DataObject.MissingSchemaAction;
                }
                set
                {
                    this.DataObject.MissingSchemaAction = value;
                }
            }

            ITableMappingCollection IDataAdapter.TableMappings
            {
                get
                {
                    return this.DataObject.TableMappings;
                }
            }
            #endregion

            #region Methods
            public int Fill(DataSet dataSet)
            {
                return this.DataObject.Fill(dataSet);
            }

            public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
            {
                return this.FillSchema(dataSet, schemaType);
            }

            IDataParameter[] IDataAdapter.GetFillParameters()
            {
                return this.GetFillParameters();
            }
            Parameter[] GetFillParameters()
            {
                return this.DataObject.GetFillParameters().Select(parameter => new Parameter(parameter)).ToArray();
            }

            public int Update(DataSet dataSet)
            {
                return this.DataObject.Update(dataSet);
            }
            #endregion
        }

        public class Command : IDbCommand, IMAGeneric<TDbCommand>
        {
            #region Constructs
            public Command()
            {
                this.DataObject = Activator.CreateInstance<TDbCommand>();
                this.Parameters = new ParameterCollection(this);
            }
            public Command(Connection connection) : this()
            {
                this.Connection = connection;
            }
            public Command(Connection connection, string commandText) : this(connection)
            {
                this.CommandText = commandText;
            }
            public Command(Connection connection, string commandText, params Parameter[] parameters) : this(connection, commandText)
            {
                this.Parameters.AddRange(parameters);
            }
            internal Command(Connection connection, IDbCommand dataObject) : this(connection)
            {
                this.DataObject = (TDbCommand)dataObject;
            }
            #endregion

            #region Variables
            public TDbCommand DataObject { get; private set; }
            Connection connection = null;
            public Connection Connection
            {
                get
                {
                    return this.connection;
                }
                set
                {
                    this.connection = value;
                    this.DataObject.Connection = this.connection.DataObject;
                }
            }
            IDbConnection IDbCommand.Connection
            {
                get
                {
                    return (IDbConnection)this.Connection;
                }
                set
                {
                    this.Connection = (Connection)value;
                }
            }
            IDbTransaction IDbCommand.Transaction
            {
                get
                {
                    return this.Transaction;
                }
                set
                {
                    this.Transaction = (Transaction)value;
                }
            }
            Transaction transaction = null;
            public Transaction Transaction
            {
                get
                {
                    return this.transaction;
                }
                set
                {
                    this.transaction = value;
                    this.DataObject.Transaction = this.transaction.DataObject;
                }
            }
            public string CommandText
            {
                get
                {
                    return this.DataObject.CommandText;
                }
                set
                {
                    this.DataObject.CommandText = value;
                }
            }
            public int CommandTimeout
            {
                get
                {
                    return this.DataObject.CommandTimeout;
                }
                set
                {
                    this.DataObject.CommandTimeout = value;
                }
            }
            public CommandType CommandType
            {
                get
                {
                    return this.DataObject.CommandType;
                }
                set
                {
                    this.DataObject.CommandType = value;
                }
            }
            IDataParameterCollection IDbCommand.Parameters
            {
                get
                {
                    return this.DataObject.Parameters;
                }
            }
            ParameterCollection parameters = null;
            public ParameterCollection Parameters
            {
                get
                {
                    return parameters;
                }
                private set
                {
                    parameters = value;
                }
            }
            public UpdateRowSource UpdatedRowSource
            {
                get
                {
                    return this.DataObject.UpdatedRowSource;
                }
                set
                {
                    this.DataObject.UpdatedRowSource = value;
                }
            }
            #endregion

            #region Methods
            public void Cancel()
            {
                this.DataObject.Cancel();
            }

            IDbDataParameter IDbCommand.CreateParameter()
            {
                return this.CreateParameter();
            }
            public Parameter CreateParameter()
            {
                return new Parameter();
            }

            public void Dispose()
            {
                this.DataObject.Dispose();
            }

            public int ExecuteNonQuery()
            {
                return this.DataObject.ExecuteNonQuery();
            }

            IDataReader IDbCommand.ExecuteReader()
            {
                return this.ExecuteReader();
            }
            public DataReader ExecuteReader()
            {
                return new DataReader(this.DataObject.ExecuteReader());
            }

            IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
            {
                return this.ExecuteReader(behavior);
            }
            public DataReader ExecuteReader(CommandBehavior behavior)
            {
                return new DataReader(this.DataObject.ExecuteReader(behavior));
            }

            public object ExecuteScalar()
            {
                return this.DataObject.ExecuteScalar();
            }

            public void Prepare()
            {
                this.DataObject.Prepare();
            }
            #endregion
        }

        public class Connection : IDbConnection, IMAGeneric<TDbConnection>
        {
            #region Constructs
            public Connection(string connectionString)
            {
                this.DataObject = Activator.CreateInstance<TDbConnection>();
                this.ConnectionString = connectionString;
            }
            #endregion

            #region Variables
            public TDbConnection DataObject { get; private set; }

            public string ConnectionString
            {
                get
                {
                    return this.DataObject.ConnectionString;
                }
                set
                {
                    this.DataObject.ConnectionString = value;
                }
            }

            public int ConnectionTimeout
            {
                get
                {
                    return this.DataObject.ConnectionTimeout;
                }
            }

            public string Database
            {
                get
                {
                    return this.DataObject.Database;
                }
            }

            public ConnectionState State
            {
                get
                {
                    return this.DataObject.State;
                }
            }

            #endregion

            #region Methods
            IDbTransaction IDbConnection.BeginTransaction()
            {
                return this.BeginTransaction();
            }
            public Transaction BeginTransaction()
            {
                return new Transaction(this, this.DataObject.BeginTransaction());
            }

            IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
            {
                return this.BeginTransaction(il);
            }
            public Transaction BeginTransaction(IsolationLevel il)
            {
                return new Transaction(this, this.DataObject.BeginTransaction(il));
            }

            public void ChangeDatabase(string databaseName)
            {
                this.DataObject.ChangeDatabase(databaseName);
            }

            public void Close()
            {
                this.DataObject.Close();
            }

            IDbCommand IDbConnection.CreateCommand()
            {
                return this.CreateCommand();
            }
            public Command CreateCommand()
            {
                return new Command(this, this.DataObject.CreateCommand());
            }

            public void Dispose()
            {
                this.DataObject.Dispose();
            }

            public void Open()
            {
                this.DataObject.Open();
            }
            #endregion
        }

        public class Parameter : IDbDataParameter, IMAGeneric<TDbParameter>
        {
            #region Constructs
            public Parameter()
            {
                this.DataObject = Activator.CreateInstance<TDbParameter>();
            }
            public Parameter(string parameterName, object value) : this()
            {
                this.ParameterName = parameterName;
                this.Value = value;
            }

            internal Parameter(IDataParameter dataObject)
            {
                this.DataObject = (TDbParameter)dataObject;
            }
            internal Parameter(IDbDataParameter dataObject)
            {
                this.DataObject = (TDbParameter)dataObject;
            }
            #endregion

            #region Variables
            public TDbParameter DataObject { get; private set; }

            public byte Precision
            {
                get
                {
                    return this.DataObject.Precision;
                }
                set
                {
                    this.DataObject.Precision = value;
                }
            }
            public byte Scale
            {
                get
                {
                    return this.DataObject.Scale;
                }
                set
                {
                    this.DataObject.Scale = value;
                }
            }
            public int Size
            {
                get
                {
                    return this.DataObject.Size;
                }
                set
                {
                    this.DataObject.Size = value;
                }
            }
            public DbType DbType
            {
                get
                {
                    return this.DataObject.DbType;
                }
                set
                {
                    this.DataObject.DbType = value;
                }
            }
            public ParameterDirection Direction
            {
                get
                {
                    return this.DataObject.Direction;
                }
                set
                {
                    this.DataObject.Direction = value;
                }
            }

            public bool IsNullable
            {
                get
                {
                    return this.DataObject.IsNullable;
                }
            }

            public string ParameterName
            {
                get
                {
                    return this.DataObject.ParameterName;
                }
                set
                {
                    this.DataObject.ParameterName = value;
                }
            }
            public string SourceColumn
            {
                get
                {
                    return this.DataObject.SourceColumn;
                }
                set
                {
                    this.DataObject.SourceColumn = value;
                }
            }
            public DataRowVersion SourceVersion
            {
                get
                {
                    return this.DataObject.SourceVersion;
                }
                set
                {
                    this.DataObject.SourceVersion = value;
                }
            }
            public object Value
            {
                get
                {
                    return this.DataObject.Value;
                }
                set
                {
                    this.DataObject.Value = value ?? DBNull.Value;
                }
            }
            #endregion

            #region Methods

            #endregion
        }

        public class ParameterCollection : IDataParameterCollection
        {
            #region Constructs
            internal ParameterCollection(Command command)
            {
                this.Command = command;
            }
            #endregion

            #region Variables
            public Command Command { get; private set; }
            private List<Parameter> items = new List<Parameter>();

            public object this[string parameterName]
            {
                get
                {
                    return items.First(item => item.ParameterName == parameterName);
                }
                set
                {
                    items.First(item => item.ParameterName == parameterName).Value = value;
                }
            }
            public object this[int index]
            {
                get
                {
                    return items[index];
                }
                set
                {
                    items[index].Value = value;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public bool IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public int Count
            {
                get
                {
                    return items.Count;
                }
            }

            public object SyncRoot
            {
                get
                {
                    return null;
                }
            }

            public bool IsSynchronized
            {
                get
                {
                    return false;
                }
            }
            #endregion

            #region Methods
            int IList.Add(object value)
            {
                return this.Add((Parameter)value);
            }
            public int Add(Parameter value)
            {
                this.items.Add(value);
                return this.Command.DataObject.Parameters.Add(value.DataObject);
            }
            public void AddRange(IEnumerable<Parameter> values)
            {
                var arrayValues = values.ToArray();
                this.items.AddRange(arrayValues);
                foreach (var item in arrayValues)
                    this.Command.DataObject.Parameters.Add(item.DataObject);
            }

            public void Clear()
            {
                this.items.Clear();
            }

            public bool Contains(string parameterName)
            {
                return items.Any(parameter => parameter.ParameterName == parameterName);
            }

            public bool Contains(object value)
            {
                return items.Any(parameter => parameter.Value == value);
            }

            public void CopyTo(Array array, int index)
            {
                for (int i = 0; i < this.items.Count; i++)
                {
                    var item = this.items[i];
                    array.SetValue(item, i + index);
                }
            }

            public IEnumerator GetEnumerator()
            {
                return items.GetEnumerator();
            }

            public int IndexOf(string parameterName)
            {
                return items.FindIndex(parameter => parameter.ParameterName == parameterName);
            }

            public int IndexOf(object value)
            {
                return items.FindIndex(parameter => parameter.Value == value);
            }

            void IList.Insert(int index, object value)
            {
                this.Insert(index, (Parameter)value);
            }
            public void Insert(int index, Parameter value)
            {
                this.items.Insert(index, value);
                this.Command.DataObject.Parameters.Insert(index, value.DataObject);
            }

            void IList.Remove(object value)
            {
                this.Remove((Parameter)value);
            }
            public void Remove(Parameter value)
            {
                this.items.Remove(value);
                this.Command.DataObject.Parameters.Remove(value.DataObject);
            }

            public void RemoveAt(string parameterName)
            {
                this.RemoveAt(this.IndexOf(parameterName));
            }

            public void RemoveAt(int index)
            {
                this.items.RemoveAt(index);
                this.Command.DataObject.Parameters.RemoveAt(index);
            }
            #endregion
        }

        public class DataReader : IDataReader, IMAGeneric<TDbDataReader>
        {
            #region Constructs
            internal DataReader()
            { }
            internal DataReader(IDataReader dataObject)
            {
                this.DataObject = (TDbDataReader)dataObject;
            }
            #endregion

            #region Classes
            public class ValueAndIsNull
            {
                #region Constructs
                public ValueAndIsNull(object value, bool isNull)
                {
                    this.Value = value;
                    this.IsNull = isNull;
                }
                #endregion

                #region Variables
                public object Value { get; set; }
                public bool IsNull { get; set; }
                #endregion
            }
            #endregion

            #region Variables
            public TDbDataReader DataObject { get; private set; }

            public object this[int i]
            {
                get
                {
                    return this.DataObject[i];
                }
            }

            public object this[string name]
            {
                get
                {
                    return this.DataObject[name];
                }
            }

            public int Depth
            {
                get
                {
                    return this.DataObject.Depth;
                }
            }

            public bool IsClosed
            {
                get
                {
                    return this.DataObject.IsClosed;
                }
            }

            public int RecordsAffected
            {
                get
                {
                    return this.DataObject.RecordsAffected;
                }
            }

            public int FieldCount
            {
                get
                {
                    return this.DataObject.FieldCount;
                }
            }
            #endregion

            #region Methods

            public void Close()
            {
                this.DataObject.Close();
            }

            public void Dispose()
            {
                this.DataObject.Dispose();
            }

            public bool GetBoolean(int i)
            {
                return this.DataObject.GetBoolean(i);
            }

            public byte GetByte(int i)
            {
                return this.DataObject.GetByte(i);
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                return this.DataObject.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
            }

            public char GetChar(int i)
            {
                return this.DataObject.GetChar(i);
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                return this.DataObject.GetChars(i, fieldoffset, buffer, bufferoffset, length);
            }

            IDataReader IDataRecord.GetData(int i)
            {
                return this.GetData(i);
            }

            public DataReader GetData(int i)
            {
                return (DataReader)this.DataObject.GetData(i);
            }

            public string GetDataTypeName(int i)
            {
                return this.DataObject.GetDataTypeName(i);
            }

            public DateTime GetDateTime(int i)
            {
                return this.DataObject.GetDateTime(i);
            }

            public decimal GetDecimal(int i)
            {
                return this.DataObject.GetDecimal(i);
            }

            public double GetDouble(int i)
            {
                return this.DataObject.GetDouble(i);
            }

            public Type GetFieldType(int i)
            {
                return this.DataObject.GetFieldType(i);
            }

            public float GetFloat(int i)
            {
                return this.DataObject.GetFloat(i);
            }

            public Guid GetGuid(int i)
            {
                return this.DataObject.GetGuid(i);
            }

            public short GetInt16(int i)
            {
                return this.DataObject.GetInt16(i);
            }

            public int GetInt32(int i)
            {
                return this.DataObject.GetInt32(i);
            }

            public long GetInt64(int i)
            {
                return this.DataObject.GetInt64(i);
            }

            public string GetName(int i)
            {
                return this.DataObject.GetName(i);
            }

            public int GetOrdinal(string name)
            {
                return this.DataObject.GetOrdinal(name);
            }

            public DataTable GetSchemaTable()
            {
                return this.DataObject.GetSchemaTable();
            }

            public string GetString(int i)
            {
                return this.DataObject.GetString(i);
            }

            public object GetValue(int i)
            {
                return this.DataObject.GetValue(i);
            }

            public int GetValues(object[] values)
            {
                return this.DataObject.GetValues(values);
            }

            public bool IsDBNull(int i)
            {
                return this.DataObject.IsDBNull(i);
            }

            public bool NextResult()
            {
                return this.DataObject.NextResult();
            }

            public bool Read()
            {
                return this.DataObject.Read();
            }
            #endregion
        }

        public class Transaction : IDbTransaction, IMAGeneric<IDbTransaction>
        {
            #region Constructs
            internal Transaction()
            { }
            internal Transaction(Connection connection, IDbTransaction dataObject)
            {
                this.Connection = connection;
                this.DataObject = dataObject;
            }
            #endregion

            #region Variables
            public IDbTransaction DataObject { get; private set; }

            public Connection Connection { get; private set; }
            IDbConnection IDbTransaction.Connection
            {
                get
                {
                    return (IDbConnection)this.Connection;
                }
            }

            public IsolationLevel IsolationLevel
            {
                get
                {
                    return this.DataObject.IsolationLevel;
                }
            }
            #endregion

            #region Methods
            public void Commit()
            {
                this.DataObject.Commit();
            }

            public void Dispose()
            {
                this.DataObject.Dispose();
            }

            public void Rollback()
            {
                this.DataObject.Rollback();
            }
            #endregion
        }
    }
}
