using MA.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity.Linq
{
    public interface IMAEntityLinqQuery<T> : ICloneable
    {
        Type Type { get; }
        Type BeforeType { get; }
        MAEntityParameter[] ParametersDefault { get; set; }
        List<MAEntityLinqOrder<T>> OrderQueries { get; }
        List<Expression<Func<T, bool>>> WhereQueries { get; }
        List<int> Skips { get; }
        List<int> Takes { get; }

        List<string> GroupByColumnsNative { get; }
        List<MAEntityLinqSql> HavingQueriesNative { get; }
        List<MAEntityNameAndOrder> OrderByColumnsNative { get; }
        List<MAEntityLinqSql> WhereQueriesNative { get; }

        MAEntityLinqSql ToSql();
        MAEntityLinqSql ToSql(ref int indexObject, bool orderEnable);
        IMAEntityLinqQuery<T> Clone();
    }
}
