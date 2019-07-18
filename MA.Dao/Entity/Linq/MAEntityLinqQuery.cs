using MA.Dal;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity.Linq
{
    public class MAEntityLinqQuery<T> : MAEntityLinqQueryFromName<T> where T : MADaoBase<T>
    {
        #region Constructs
        internal MAEntityLinqQuery() : base(MADaoBase<T>.TypeObjectName)
        {
        }
        #endregion

        #region Methods
        #endregion
    }

    public class MAEntityLinqQuery<T, TBefore> : IMAEntityLinqQuery<T>
    {
        #region Constructs
        internal MAEntityLinqQuery()
        {
        }

        internal MAEntityLinqQuery(IMAEntityLinqQuery<TBefore> beforeQuery) : this()
        {
            this.BeforeQuery = beforeQuery;
        }
        static MAEntityLinqQuery()
        {
            _Stype = typeof(T);
            var constructs = _Stype.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            _ObjectConstructor = MAEntityLinqReflection.CreateParameterizedConstructor(constructs[0]);
            _StypeCode = Type.GetTypeCode(SType);
            _Properties = SType.GetProperties();
            _BeforeTypeS = typeof(TBefore);
        }
        #endregion

        #region Variables
        #region Cache
        private static MAEntityLinqReflection.ObjectConstructor<object> _ObjectConstructor = null;
        internal static MAEntityLinqReflection.ObjectConstructor<object> ObjectConstructor
        {
            get
            {
                return _ObjectConstructor;
            }
        }
        #endregion

        public IMAEntityLinqQuery<TBefore> BeforeQuery { get; private set; }

        private static TypeCode? _StypeCode = null;
        public static TypeCode STypeCode
        {
            get
            {
                return _StypeCode.Value;
            }
        }

        private static Type _Stype = null;
        public static Type SType
        {
            get
            {
                return _Stype;
            }
        }

        private static PropertyInfo[] _Properties = null;
        public static PropertyInfo[] Properties
        {
            get
            {
                return _Properties;
            }
        }
        public Type Type
        {
            get
            {
                return SType;
            }
        }

        private static Type _BeforeTypeS = null;
        public static Type BeforeTypeS
        {
            get
            {
                return _BeforeTypeS;
            }
        }
        public Type BeforeType
        {
            get
            {
                return BeforeTypeS;
            }
        }

        public MAEntityParameter[] ParametersDefault { get; set; }

        private List<Expression<Func<T, object>>> _GroupByQueries = new List<Expression<Func<T, object>>>();
        public List<Expression<Func<T, object>>> GroupByQueries
        {
            get
            {
                return _GroupByQueries;
            }
        }

        private List<Expression<Func<T, bool>>> _HavingQueries = new List<Expression<Func<T, bool>>>();
        public List<Expression<Func<T, bool>>> HavingQueries
        {
            get
            {
                return _HavingQueries;
            }
        }

        private List<MAEntityLinqOrder<T>> _OrderQueries = new List<MAEntityLinqOrder<T>>();
        public List<MAEntityLinqOrder<T>> OrderQueries
        {
            get
            {
                return _OrderQueries;
            }
        }

        private List<Expression<Func<T, bool>>> _WhereQueries = new List<Expression<Func<T, bool>>>();
        public List<Expression<Func<T, bool>>> WhereQueries
        {
            get
            {
                return _WhereQueries;
            }
        }

        #region Native

        private List<string> _GroupByColumnsNative = new List<string>();
        public List<string> GroupByColumnsNative
        {
            get
            {
                return _GroupByColumnsNative;
            }
        }

        private List<MAEntityLinqSql> _HavingQueriesNative = new List<MAEntityLinqSql>();
        public List<MAEntityLinqSql> HavingQueriesNative
        {
            get
            {
                return _HavingQueriesNative;
            }
        }

        private List<MAEntityNameAndOrder> _OrderByColumnsNative = new List<MAEntityNameAndOrder>();
        public List<MAEntityNameAndOrder> OrderByColumnsNative
        {
            get
            {
                return _OrderByColumnsNative;
            }
        }

        private List<MAEntityLinqSql> _WhereQueriesNative = new List<MAEntityLinqSql>();
        public List<MAEntityLinqSql> WhereQueriesNative
        {
            get
            {
                return _WhereQueriesNative;
            }
        }


        #endregion

        internal List<int> skips = new List<int>();
        public List<int> Skips
        {
            get
            {
                return skips;
            }
        }

        internal List<int> takes = new List<int>();
        public List<int> Takes
        {
            get
            {
                return takes;
            }
        }

        public bool DistinctEnable { get; set; }

        internal Expression<Func<TBefore, T>> SelectQuery { get; set; }


        internal string[] beforeGroupColumns = null;
        #endregion

        #region Methods

        #region Add
        public MAEntityLinqQuery<T, TBefore> OrderBy(Expression<Func<T, object>> query)
        {
            this._OrderQueries.Add(
                    new MAEntityLinqOrder<T>(
                            query, true
                        )
                );
            return this;
        }

        public MAEntityLinqQuery<T, TBefore> OrderByDescending(Expression<Func<T, object>> query)
        {
            this.OrderQueries.Add(
                    new MAEntityLinqOrder<T>(
                            query, false
                        )
                );
            return this;
        }

        public MAEntityLinqQuery<T, TBefore> GroupBy(Expression<Func<T, object>> query)
        {
            this.GroupByQueries.Add(query);
            return this;
        }

        public MAEntityLinqQuery<T, TBefore> Having(Expression<Func<T, bool>> query)
        {
            this._HavingQueries.Add(query);
            return this;
        }

        public MAEntityLinqQuery<T, TBefore> Where(Expression<Func<T, bool>> query)
        {
            this._WhereQueries.Add(query);
            return this;
        }

        #region Native
        public MAEntityLinqQuery<T, TBefore> Native_OrderBy(string name)
        {
            this._OrderByColumnsNative.Add(new MAEntityNameAndOrder(name, true));
            return this;
        }

        public MAEntityLinqQuery<T, TBefore> Native_OrderByDescending(string name)
        {
            this._OrderByColumnsNative.Add(new MAEntityNameAndOrder(name, false));
            return this;
        }

        public MAEntityLinqQuery<T, TBefore> Native_GroupBy(params string[] columns)
        {
            this._GroupByColumnsNative.AddRange(columns);
            return this;
        }

        public MAEntityLinqQuery<T, TBefore> Native_Having(string query, params MAEntityParameter[] parameters)
        {
            this._HavingQueriesNative.Add(new MAEntityLinqSql(query, parameters));
            return this;
        }

        public MAEntityLinqQuery<T, TBefore> Native_Where(string query, params MAEntityParameter[] parameters)
        {
            this._WhereQueriesNative.Add(new MAEntityLinqSql(query, parameters));
            return this;
        }
        #endregion

        public MAEntityLinqQuery<T, TBefore> Skip(int skip)
        {
            this.skips.Add(skip);
            return this;
        }

        public MAEntityLinqQuery<T, TBefore> Take(int take)
        {
            this.takes.Add(take);
            return this;
        }

        public MAEntityLinqQuery<TNew, T> Select<TNew>(Expression<Func<T, TNew>> query)
        {
            return new MAEntityLinqQuery<TNew, T>(this)
            {
                SelectQuery = query
            };
        }

        public MAEntityLinqQuery<T, T> New()
        {
            return new MAEntityLinqQuery<T, T>(this)
            {
                SelectQuery = null
            };
        }

        public MAEntityLinqQuery<T, TBefore> Distinct()
        {
            this.DistinctEnable = true;
            return this;
        }
        #endregion

        #region Result
        internal string[] Get_GroupByQueries(List<MAEntityParameter> parameters, ref int indexObject)
        {
            List<string> groups = new List<string>();
            foreach (var item in this.GroupByQueries)
            {
                string[] objectNames = new string[0];
                item.ToValues(parameters, ref indexObject, ref objectNames);
                foreach (var objectName in objectNames)
                {
                    groups.Add(objectName);
                }
            }
            groups.AddRange(this._GroupByColumnsNative);

            return groups.ToArray();
        }

        internal string Get_HavingQueries(List<MAEntityParameter> parameters, ref int indexObject)
        {
            List<string> havingQueries = new List<string>();

            foreach (var item in this._HavingQueries)
            {
                item.Body.ToIfOneOnly(havingQueries, parameters, ref indexObject);
            }

            foreach (var item in this._HavingQueriesNative)
            {
                parameters.AddRange(item.Parameters);
                havingQueries.Add(item.Query);
            }

            return MAData.Sql.WhereAnd(havingQueries.ToArray());
        }

        internal string Get_OrderQueries(List<MAEntityParameter> parameters, ref int indexObject)
        {
            List<MAEntityNameAndOrder> orders = new List<MAEntityNameAndOrder>();
            foreach (var item in this.OrderQueries)
            {
                string[] objectNames = new string[0];
                item.Query.ToValues(parameters, ref indexObject, ref objectNames);
                foreach (var objectName in objectNames)
                {
                    orders.Add(
                      new MAEntityNameAndOrder(objectName, item.Asc)
                      );
                }
            }
            orders.AddRange(_OrderByColumnsNative);
            return MAData.Sql.OrderByCommand(orders.ToMAData());
        }

        internal string Get_WhereQueries(List<MAEntityParameter> parameters, ref int indexObject)
        {
            List<string> whereQueries = new List<string>();

            foreach (var item in this._WhereQueries)
            {
                item.Body.ToIfOneOnly(whereQueries, parameters, ref indexObject);
            }

            foreach (var item in this._WhereQueriesNative)
            {
                parameters.AddRange(item.Parameters);
                whereQueries.Add(item.Query);
            }

            return MAData.Sql.WhereAnd(whereQueries.ToArray());
        }

        internal int Get_Skips()
        {
            int skip = 0;
            foreach (var item in this.skips)
                skip += item;
            return skip;
        }

        internal int Get_Takes()
        {
            int takes = 0;
            foreach (var item in this.takes)
                takes += item;
            return takes;
        }

        public MAEntityLinqSql ToSql()
        {
            int indexObject = 0;
            return ToSql(ref indexObject);
        }

        public MAEntityLinqSql ToSql(ref int indexObject, bool orderEnable = true)
        {
            return toSqlWithColumnName(orderEnable, ref indexObject, new string[0]);
        }

        private MAEntityLinqSql toSqlWithColumnName(bool orderEnable, ref int indexObject, string[] columns)
        {
            string query = "";
            List<MAEntityParameter> parameters = new List<MAEntityParameter>();
            beforeGroupColumns = this.Get_GroupByQueries(parameters, ref indexObject);

            string selectObjectOrQuery = "";
            if (BeforeQuery != null)
            {
                var beforeQuery_Sql = this.BeforeQuery.ToSql(ref indexObject, false);
                parameters.AddRange(beforeQuery_Sql.Parameters);

                indexObject++;
                selectObjectOrQuery = "(" + beforeQuery_Sql.Query + ") AS " + MAData.Sql.ObjectName("ma_before_query" + indexObject);
            }
            else
            {
                selectObjectOrQuery = ((IMAEntityLinqQueryFirst)this).GetObjectName();
            }

            if (this.ParametersDefault != null)
                parameters.AddRange(this.ParametersDefault);

            var orderQuery = this.Get_OrderQueries(parameters, ref indexObject);

            bool existsSkipOrTake = this.skips.Count > 0 || this.takes.Count > 0;

            query = MAData.Sql.Select(
                selectObjectOrQuery,
                columns.Length > 0 ? columns : this.GetColumns(parameters, ref indexObject),
                this.Get_WhereQueries(parameters, ref indexObject),
                beforeGroupColumns,
                this.Get_HavingQueries(parameters, ref indexObject),
                existsSkipOrTake || !orderEnable ? null : orderQuery,
                this.DistinctEnable);

            if (existsSkipOrTake)
            {
                int? skip = this.Get_Skips();
                int? take = this.Get_Takes();

                skip = skip > 0 ? skip + 1 : null;
                take = (take > 0 ? (skip == null ? take : take - 1) : null);

                var startRowIndex = skip;
                var endRowIndex = (skip ?? 0) + take;

                indexObject++;
                query = MAData.Sql.SelectPage(query, orderQuery, startRowIndex, endRowIndex, indexObject, orderEnable);
            }

            return new MAEntityLinqSql(query, parameters.ToArray());
        }

        public virtual object Clone()
        {
            return ((IMAEntityLinqQuery<T>)this).Clone();
        }

        IMAEntityLinqQuery<T> IMAEntityLinqQuery<T>.Clone()
        {
            return MAEntityLinqExtensions.Clone(this);
        }

        public int Count(MAEntity entity)
        {
            return Count<int>(entity);
        }

        public TResult Count<TResult>(MAEntity entity)
        {
            return entity.Run(() =>
            {
                int indexObject = 0;
                var toSql = this.toSqlWithColumnName(false, ref indexObject, new string[] { MAData.Sql.AllCount });
                using (MAData.Command command = new MAData.Command(entity.Connection, toSql.Query, toSql.Parameters.ToMAData()))
                {
                    if (entity.Transaction != null)
                        command.Transaction = entity.Transaction;
                    using (MAData.DataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            return (TResult)reader.GetValue(0);
                    }
                }

                return default(TResult);
            });
        }

        private TResult oneColumnProcess<TResult>(MAEntity entity, Expression<Func<T, TResult>> prop, Func<string, string> getColName)
        {
            return entity.Run(() =>
            {
                int indexObject = 0;
                var propColumn = prop.GetColumns(new List<MAEntityParameter>(), ref indexObject, false)[0];
                var column = this.GetColumns(new List<MAEntityParameter>(), ref indexObject, false, propColumn)[0];

                var toSql = this.toSqlWithColumnName(false, ref indexObject, new string[] { getColName(column) });

                using (MAData.Command command = new MAData.Command(entity.Connection, toSql.Query, toSql.Parameters.ToMAData()))
                {
                    if (entity.Transaction != null)
                        command.Transaction = entity.Transaction;
                    using (MAData.DataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            return (TResult)reader.GetValue(0);
                    }
                }

                return default(TResult);
            });
        }

        public TResult Sum<TResult>(MAEntity entity, Expression<Func<T, TResult>> prop)
        {
            return oneColumnProcess(entity, prop, (column) => MAData.Sql.Sum(column));
        }

        public TResult Max<TResult>(MAEntity entity, Expression<Func<T, TResult>> prop)
        {
            return oneColumnProcess(entity, prop, (column) => MAData.Sql.Max(column));
        }

        public TResult Min<TResult>(MAEntity entity, Expression<Func<T, TResult>> prop)
        {
            return oneColumnProcess(entity, prop, (column) => MAData.Sql.Min(column));
        }

        public virtual List<T> ToList(MAEntity entity)
        {
            return entity.Run(() =>
            {
                List<T> rows = new List<T>();

                MAData.DataReader reader = null;
                Action actionCreateType = () =>
                {
                    object[] parametersValue = new object[Properties.Length];
                    for (int i = 0; i < Properties.Length; i++)
                    {
                        if (reader[i] == null || reader[i] == DBNull.Value)
                            parametersValue[i] = null;
                        else
                            parametersValue[i] = reader[i];
                    }

                    rows.Add((T)ObjectConstructor(parametersValue));
                };
                Action actionDotNETType = () =>
                {
                    rows.Add((T)reader.GetValue(0));
                };

                Action activeAction = null;
                switch (STypeCode)
                {
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                    case TypeCode.String:
                        activeAction = actionDotNETType;
                        break;
                    case TypeCode.Object:
                    default:
                        activeAction = actionCreateType;
                        break;
                }
                var toSql = this.ToSql();
                using (MAData.Command command = new MAData.Command(entity.Connection, toSql.Query, toSql.Parameters.ToMAData()))
                {
                    if (entity.Transaction != null)
                        command.Transaction = entity.Transaction;
                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            activeAction();
                        }
                    }
                }

                return rows;
            });
        }

        public T FirstOrDefault(MAEntity entity)
        {
            return this.ToList(entity).FirstOrDefault();
        }
        /// <summary>
        /// this.ToList(entity).FirstOrDefault()
        /// </summary>
        /// <param name="entity">Database Connection</param>
        /// <returns></returns>
        public T FirstOrDefault(MAEntity entity, Expression<Func<T, bool>> query)
        {
            this.Where(query);
            return this.ToList(entity).FirstOrDefault();
        }

        /// <summary>
        /// return this.ToList(entity)[0]
        /// </summary>
        /// <param name="entity">Database Connection</param>
        /// <returns></returns>
        public T First(MAEntity entity)
        {
            return this.ToList(entity)[0];
        }
        public T First(MAEntity entity, Expression<Func<T, bool>> query)
        {
            this.Where(query);
            return this.ToList(entity)[0];
        }
        #endregion
        #endregion
    }
}