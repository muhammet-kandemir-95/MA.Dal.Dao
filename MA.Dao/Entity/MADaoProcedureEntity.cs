using MA.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity
{
    public class MADaoProcedureEntity<TModel, TResult> : MADaoProcedure<TModel, TResult> where TModel : MADaoProcedureEntity<TModel, TResult>
    {
        #region Methods
        protected override TResult GetResultFromCommand(MAData.Command command)
        {
            return MAEntity.First<TResult>(command);
        }

        public TResult Exec(
            MAEntity db
            )
        {
            return db.Run(
                () =>
                {
                    return Exec(db.Connection, db.Transaction);
                }
                );
        }
        #endregion
    }
}
