using MA.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao
{
    public abstract class MADaoProcedure<TModel, TResult> : MADaoBase<TModel> where TModel : MADaoProcedure<TModel, TResult>
    {
        #region Constructs
        static MADaoProcedure()
        {
            _TResult_EnumValues = (TResult[])Enum.GetValues(typeof(TResult));
        }
        #endregion

        #region Variables

        public static string ProcedureName
        {
            get
            {
                return TypeObjectName;
            }
        }

        static TResult[] _TResult_EnumValues = null;
        protected static TResult[] TResult_EnumValues
        {
            get
            {
                return _TResult_EnumValues;
            }
        }
        #endregion

        #region Methods
        protected virtual TResult GetResultFromCommand(MAData.Command command)
        {
            return Activator.CreateInstance<TResult>();
        }

        protected TResult Exec(
            MAData.Connection connection,
            MAData.Transaction transaction
            )
        {
            var propAndParameters = Properties.Select(prop => new
            {
                ParameterName = MAData.Sql.Parameter(prop.ColumnName),
                Value = prop.Property.GetValue(this)
            }).ToArray();
            string sql = MAData.Sql.ExecProcedure(ProcedureName, propAndParameters.Select(prop => prop.ParameterName).ToArray());
            using (MAData.Command command = new MAData.Command(connection, sql))
            {
                if (transaction != null)
                    command.Transaction = transaction;
                foreach (var item in propAndParameters)
                    command.Parameters.Add(new MAData.Parameter(item.ParameterName, item.Value));

                return GetResultFromCommand(command);
            }
        }
        #endregion
    }
}
