using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity.Linq
{
    public class MAEntityLinqQueryTable<T> : MAEntityLinqQuery<T> where T : MADaoTableEntity<T>
    {
        #region Constructs
        public MAEntityLinqQueryTable() : base()
        {
        }
        #endregion

        #region Methods
        public override List<T> ToList(MAEntity entity)
        {
            var sql = this.ToSql();
            return MADaoTableEntity<T>.Select(entity, sql.Query, sql.Parameters);
        }

        public override object Clone()
        {
            return MAEntityLinqExtensions.Clone(this, new MAEntityLinqQueryTable<T>());
        }
        #endregion
    }
}
