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
    public abstract class MADaoViewOrFunc<TModel> : MADaoBase<TModel> where TModel : MADaoViewOrFunc<TModel>
    {
        #region Constructs
        #endregion

        #region Variables

        public static string ViewOrFunctionName
        {
            get
            {
                return TypeObjectName;
            }
        }
        #endregion

        #region Methods
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
        public static List<TModel> SelectProtected(
            MAData.Command command
            )
        {
            return SelectForModel(
                command,
                (model) => { },
                (model) => { }
                );
        }
        #endregion
    }
}
