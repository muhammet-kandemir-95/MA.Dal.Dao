using MA.Dal;
using MA.Dao;
using MA.Dao.Entity;
using MA.Dao.Entity.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

public static class MAEntityLinqExtensions
{
    #region Clone
    public static MAEntityLinqQuery<T> Clone<T>(this MAEntityLinqQuery<T> value) where T : MADaoBase<T>
    {
        var objectValue = ((MAEntityLinqQuery<T, T>)value);
        return objectValue.Clone(new MAEntityLinqQuery<T>());
    }

    public static MAEntityLinqQuery<T, TBefore> Clone<T, TBefore>(this MAEntityLinqQuery<T, TBefore> value)
    {
        return value.Clone(new MAEntityLinqQuery<T, TBefore>(value.BeforeQuery.Clone()));
    }

    public static TObject Clone<T, TBefore, TObject>(this MAEntityLinqQuery<T, TBefore> value, TObject clone) where TObject : MAEntityLinqQuery<T, TBefore>
    {
        foreach (var item in value.OrderQueries)
            clone.OrderQueries.Add(item);

        foreach (var item in value.WhereQueries)
            clone.WhereQueries.Add(item);

        foreach (var item in value.Skips)
            clone.Skips.Add(item);

        foreach (var item in value.Takes)
            clone.Takes.Add(item);

        return clone;
    }

    internal static string[] GetColumns<T, TBefore>(this MAEntityLinqQuery<T, TBefore> value, List<MAEntityParameter> parameters, ref int indexObject, bool aliasName = true, string filterColumnName = null)
    {
        if (value.BeforeQuery == null)
            return filterColumnName == null ? (
                value.GroupByQueries.Count == 0 ? MAData.Sql.AllColumns : value.beforeGroupColumns
                ) : new string[] { filterColumnName };
        else if (value.SelectQuery == null)
            return filterColumnName == null ? MAData.Sql.AllColumns : new string[] { filterColumnName };

        return value.SelectQuery.GetColumns(parameters, ref indexObject, aliasName, filterColumnName);
    }

    internal static string[] GetColumns<TDelegate>(this Expression<TDelegate> expression, List<MAEntityParameter> parameters, ref int indexObject, bool alias = true, string filterColumnName = null, bool onlyAliasName = false, Dictionary<Type, string> parametersObjectNameAdd = null)
    {
        if (expression.Body is NewExpression)
        {
            List<string> columns = new List<string>();

            var newExpression = (NewExpression)expression.Body;
            for (int i = 0; i < newExpression.Arguments.Count; i++)
            {
                var argument = newExpression.Arguments[i];
                var member = newExpression.Members[i];

                string objectName = "";
                argument.ToValue(parameters, ref indexObject, ref objectName, true, parametersObjectNameAdd);
                if (filterColumnName == null || member.Name == filterColumnName)
                    columns.Add(onlyAliasName ? member.Name : objectName + (alias ? " AS " + member.Name : ""));
            }

            return columns.ToArray();
        }
        if (expression.Body is MemberExpression)
        {
            return new string[]
            {
                ((MemberExpression)expression.Body).Member.Name
            };
        }
        else if (expression.Body is UnaryExpression)
        {
            var op = ((UnaryExpression)expression.Body).Operand;
            if (op.NodeType == ExpressionType.Parameter)
            {
                return new string[]
                {
                    MAData.Sql.AllColumnsString
                };
            }
            else
            {
                return new string[]
                {
                    ((MemberExpression)op).Member.Name
                };
            }
        }
        else
        {
            string objectName = "";
            expression.Body.ToValue(parameters, ref indexObject, ref objectName, true, parametersObjectNameAdd);
            return new string[]
            {
                objectName
            };
        }
    }
    #endregion

    #region SQL
    public static decimal? Count_MAEntity(this object o)
    {
        return 0;
    }
    public static T Sum_MAEntity<T>(this T o)
    {
        return default(T);
    }
    public static T Max_MAEntity<T>(this T o)
    {
        return default(T);
    }
    public static T Min_MAEntity<T>(this T o)
    {
        return default(T);
    }
    #endregion

    #region Convert
    public static byte ToByte_MAEntity(this object o)
    {
        return Convert.ToByte(o);
    }
    public static short ToInt16_MAEntity(this object o)
    {
        return Convert.ToInt16(o);
    }
    public static int ToInt32_MAEntity(this object o)
    {
        return Convert.ToInt32(o);
    }
    public static long ToInt64_MAEntity(this object o)
    {
        return Convert.ToInt64(o);
    }
    public static decimal ToDecimal_MAEntity(this object o)
    {
        return Convert.ToDecimal(o);
    }
    public static DateTime ToDate_MAEntity(this object o)
    {
        return Convert.ToDateTime(o);
    }
    public static DateTime ToDateTime_MAEntity(this object o)
    {
        return Convert.ToDateTime(o);
    }
    #endregion

    #region Change Type
    public static T ChangeType_MAEntity<T>(this object o)
    {
        return (T)o;
    }
    #endregion
}
