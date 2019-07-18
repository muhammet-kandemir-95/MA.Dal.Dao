using MA.Dal;
using MA.Dao.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MA.Dao
{
    public abstract class MADaoBase<TModel> : MADaoBase where TModel : class
    {
        #region Contructs
        static MADaoBase()
        {
            _Type = typeof(TModel);
            _Properties = _Type.GetProperties().Select(prop => new MADaoProperty(prop)).Where(prop => prop.AttributeColumn != null).ToArray();
            _AttributeName = (MADataNameAttribute)_Type.GetCustomAttributes().FirstOrDefault(attr => attr is MADataNameAttribute);
            _TypeName = _AttributeName == null || string.IsNullOrEmpty(_AttributeName.Name) ? _Type.Name : _AttributeName.Name;
        }
        #endregion

        #region Variables
        static MADaoProperty[] _Properties = null;
        public static MADaoProperty[] Properties
        {
            get
            {
                return _Properties;
            }
        }

        static MADataNameAttribute _AttributeName = null;
        public static MADataNameAttribute AttributeName
        {
            get
            {
                return _AttributeName;
            }
        }

        static Type _Type = null;
        public static Type Type
        {
            get
            {
                return _Type;
            }
        }

        static string _TypeName = null;
        public static string TypeName
        {
            get
            {
                return _TypeName;
            }
        }

        internal static string TypeObjectName
        {
            get
            {
                return MAData.Sql.ObjectName(TypeName);
            }
        }
        #endregion

        #region Methods
        internal static List<TModel> SelectForModel(
            MAData.Command command,
            Action<TModel> editCreateMethod,
            Action<TModel> editBeforeAddMethod
            )
        {
            return SelectForResult<TModel>(command, editCreateMethod, editBeforeAddMethod);
        }
        public static string PropertyName(Expression<Func<TModel, object>> func)
        {
            return PropertyName<TModel, object>(func);
        }
        private static string columnNameOriginal(Expression<Func<TModel, object>> func)
        {
            var propName = PropertyName(func);
            return Properties.FirstOrDefault(prop => prop.Property.Name == propName).ColumnName;
        }
        public static string ColumnName(Expression<Func<TModel, object>> func)
        {
            return MAData.Sql.ObjectName(columnNameOriginal(func));
        }
        public static string ColumnParameterName(Expression<Func<TModel, object>> func)
        {
            return MAData.Sql.Parameter(columnNameOriginal(func));
        }
        public static string ColumnNameOrder(Expression<Func<TModel, object>> func, bool asc = true)
        {
            return MAData.Sql.OrderByCommand(new MAData.Sql.NameAndOrder(columnNameOriginal(func), asc));
        }
        #endregion
    }

    public abstract class MADaoBase
    {
        #region Methods
        public static string PropertyName<TModel, TResult>(Expression<Func<TModel, TResult>> func)
        {
            /*Edit : https://github.com/ArifKibrit*/
            if (func.Body is MemberExpression)
            {
                return ((MemberExpression)func.Body).Member.Name;
            }
            else
            {
                var op = ((UnaryExpression)func.Body).Operand;
                return ((MemberExpression)op).Member.Name;
            }
        }
        internal static List<TResult> SelectForResult<TResult>(
            MAData.Command command,
            Action<TResult> editCreateMethod,
            Action<TResult> editBeforeAddMethod,
            MADaoProperty[] properties = null
        )
        {
            if (properties == null)
                properties = typeof(TResult).GetProperties().Select(prop => new MADaoProperty(prop)).Where(prop => prop.AttributeColumn != null).ToArray();
            List<TResult> results = new List<TResult>();

            using (MAData.DataReader reader = command.ExecuteReader())
            {
                string[] columns = new string[reader.FieldCount];
                for (int i = 0; i < columns.Length; i++)
                    columns[i] = reader.GetName(i);

                while (reader.Read())
                {
                    TResult row = Activator.CreateInstance<TResult>();
                    editCreateMethod(row);

                    int columnIndex = 0;
                    foreach (var column in columns)
                    {
                        var property = properties.FirstOrDefault(prop => prop.ColumnName == column);
                        if (property != null)
                        {
                            if (reader.IsDBNull(columnIndex))
                            {
                                if (property.IsNullable)
                                    property.Property.SetValue(row, null);
                                else
                                    property.Property.SetValue(row, Activator.CreateInstance(property.Property.PropertyType));
                            }
                            else
                                property.Property.SetValue(row, reader[columnIndex]);
                        }
                        columnIndex++;
                    }

                    editBeforeAddMethod(row);
                    results.Add(row);
                }
            }

            return results;
        }
        #endregion
    }
}
