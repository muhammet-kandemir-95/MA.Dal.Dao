using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity.Linq
{
    public class MAEntityLinqQueryViewOrFunc<T> : MAEntityLinqQuery<T> where T : MADaoViewOrFuncEntity<T>
    {
        #region Constructs
        public MAEntityLinqQueryViewOrFunc() : base()
        {
        }
        #endregion

        #region Methods
        public override List<T> ToList(MAEntity entity)
        {
            var sql = this.ToSql();
            return MADaoViewOrFuncEntity<T>.Select(entity, sql.Query, sql.Parameters);
        }

        public override object Clone()
        {
            return MAEntityLinqExtensions.Clone(this, new MAEntityLinqQueryViewOrFunc<T>());
        }
        #endregion
    }
}
