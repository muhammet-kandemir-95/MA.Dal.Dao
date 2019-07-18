using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity.Linq
{
    public class MAEntityLinqOrder<T>
    {
        #region Constructs
        public MAEntityLinqOrder(Expression<Func<T, object>> query, bool asc)
        {
            this.Query = query;
            this.Asc = asc;
        }
        #endregion

        #region Variables
        public Expression<Func<T, object>> Query { get; internal set; }
        public bool Asc { get; internal set; }
        #endregion
    }
}
