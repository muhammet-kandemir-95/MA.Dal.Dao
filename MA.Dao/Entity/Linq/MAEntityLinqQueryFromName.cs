using MA.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity.Linq
{

    public class MAEntityLinqQueryFromName<T> : MAEntityLinqQuery<T, T>, IMAEntityLinqQueryFirst
    {
        #region Constructs
        internal MAEntityLinqQueryFromName(string objectName) : base()
        {
            this.ObjectName = objectName;
            this.SafeObjectName = objectName;
        }
        internal MAEntityLinqQueryFromName(string objectName, string safeObjectName) : base()
        {
            this.ObjectName = objectName;
            this.SafeObjectName = safeObjectName;
        }
        #endregion

        #region Variables
        public string ObjectName { get; internal set; }
        public string SafeObjectName { get; internal set; }
        #endregion

        #region Methods
        public string GetObjectName()
        {
            return ObjectName;
        }
        public string GetSafeObjectName()
        {
            return SafeObjectName;
        }

        #region Joins
        public MAEntityLinqQueryFromName<TResult> InnerJoin<TJoin, TResult>(
            MAEntityLinqQuery<TJoin> query,
            Expression<Func<T, TJoin, bool>> whereEqual,
            Expression<Func<T, TJoin, TResult>> selectResult) where TJoin : MADaoBase<TJoin>
        {
            return this.Join(MAData.Sql.JoinType.Inner, query, whereEqual, selectResult);
        }

        public MAEntityLinqQueryFromName<TResult> LeftJoin<TJoin, TResult>(
            MAEntityLinqQuery<TJoin> query,
            Expression<Func<T, TJoin, bool>> whereEqual,
            Expression<Func<T, TJoin, TResult>> selectResult) where TJoin : MADaoBase<TJoin>
        {
            return this.Join(MAData.Sql.JoinType.Left, query, whereEqual, selectResult);
        }

        public MAEntityLinqQueryFromName<TResult> RightJoin<TJoin, TResult>(
            MAEntityLinqQuery<TJoin> query,
            Expression<Func<T, TJoin, bool>> whereEqual,
            Expression<Func<T, TJoin, TResult>> selectResult) where TJoin : MADaoBase<TJoin>
        {
            return this.Join(MAData.Sql.JoinType.Right, query, whereEqual, selectResult);
        }


        public MAEntityLinqQueryFromName<TResult> FullJoin<TJoin, TResult>(
            MAEntityLinqQuery<TJoin> query,
            Expression<Func<T, TJoin, bool>> whereEqual,
            Expression<Func<T, TJoin, TResult>> selectResult) where TJoin : MADaoBase<TJoin>
        {
            return this.Join(MAData.Sql.JoinType.Full, query, whereEqual, selectResult);
        }

        internal MAEntityLinqQueryFromName<TResult> Join<TJoin, TResult>(
            MAData.Sql.JoinType type,
            MAEntityLinqQuery<TJoin> query,
            Expression<Func<T, TJoin, bool>> whereEqual,
            Expression<Func<T, TJoin, TResult>> selectResult) where TJoin : MADaoBase<TJoin>
        {
            return Join<TJoin, TResult>(type, whereEqual, selectResult);
        }

        internal MAEntityLinqQueryFromName<TResult> Join<TJoin, TResult>(
        MAData.Sql.JoinType type,
        Expression<Func<T, TJoin, bool>> whereEqual,
        Expression<Func<T, TJoin, TResult>> selectResult) where TJoin : MADaoBase<TJoin>
        {
            var joinObjectName = MADaoBase<TJoin>.TypeObjectName;
            var thisObjectName = this.SafeObjectName;

            List<MAEntityParameter> parameters = new List<MAEntityParameter>();
            int indexObject = 100000 + (this.ParametersDefault == null ? 0 : this.ParametersDefault.Length + 2);

            var objectNameTable = MAData.Sql.ObjectName("ma_before_query_join_table");
            indexObject++;
            var objectNameJoinTable = objectNameTable + indexObject;

            Dictionary<Type, string> parametersObjectNameAdd = new Dictionary<Type, string>();
            parametersObjectNameAdd.Add(MADaoBase<TJoin>.Type, objectNameJoinTable);
            parametersObjectNameAdd.Add(SType, thisObjectName);

            var whereQueries = new List<string>();
            whereEqual.ToIfOneOnly(whereQueries, parameters, ref indexObject, parametersObjectNameAdd: parametersObjectNameAdd);
            var selectResultColumns = selectResult.GetColumns(parameters, ref indexObject, parametersObjectNameAdd: parametersObjectNameAdd);

            var objectName = MAData.Sql.ObjectName("ma_before_query_join");
            var query1 = "(" + MAData.Sql.Select(
                    this.ObjectName,
                    selectResultColumns,
                    joins: new string[]
                    {
                        MAData.Sql.Join(type, joinObjectName + " AS " + objectNameJoinTable, MAData.Sql.WhereAnd(whereQueries.ToArray()))
                    }
                    ) + ") AS " + objectName;

            if (this.ParametersDefault != null)
                parameters.AddRange(this.ParametersDefault);

            return new MAEntityLinqQueryFromName<TResult>(
                query1,
                objectName
                )
            {
                ParametersDefault = parameters.ToArray()
            };
        }
        #endregion
        #endregion
    }
}
