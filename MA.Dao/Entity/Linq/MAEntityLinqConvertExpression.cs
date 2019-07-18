using MA.Dal;
using MA.Dao.Attributes;
using MA.Dao.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

internal static class MAEntityLinqConvertExpressionExtension
{
    #region Methods
    internal static void ToIfOneOnly(this Expression expression, List<string> whereQueries, List<MAEntityParameter> parameters, ref int indexObject, bool not = false, Dictionary<Type, string> parametersObjectNameAdd = null)
    {
        if (expression is BinaryExpression)
        {
            var binaryExpression = (BinaryExpression)expression;
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.AndAlso:
                    {
                        List<string> inlineWhereQueries = new List<string>();
                        binaryExpression.Left.ToIfOneOnly(inlineWhereQueries, parameters, ref indexObject, parametersObjectNameAdd: parametersObjectNameAdd);
                        binaryExpression.Right.ToIfOneOnly(inlineWhereQueries, parameters, ref indexObject, parametersObjectNameAdd: parametersObjectNameAdd);
                        whereQueries.Add(
                            MAData.Sql.WhereAnd(inlineWhereQueries.ToArray())
                            );
                    }
                    break;
                case ExpressionType.OrElse:
                    {
                        List<string> inlineWhereQueries = new List<string>();
                        binaryExpression.Left.ToIfOneOnly(inlineWhereQueries, parameters, ref indexObject, parametersObjectNameAdd: parametersObjectNameAdd);
                        binaryExpression.Right.ToIfOneOnly(inlineWhereQueries, parameters, ref indexObject, parametersObjectNameAdd: parametersObjectNameAdd);
                        whereQueries.Add(
                            MAData.Sql.WhereOr(inlineWhereQueries.ToArray())
                            );
                    }
                    break;
                default:
                    {
                        string leftObjectName = "";
                        string rightObjectName = "";

                        binaryExpression.Left.ToValue(parameters, ref indexObject, ref leftObjectName, false, parametersObjectNameAdd: parametersObjectNameAdd);
                        binaryExpression.Right.ToValue(parameters, ref indexObject, ref rightObjectName, false, parametersObjectNameAdd: parametersObjectNameAdd);

                        switch (binaryExpression.NodeType)
                        {
                            case ExpressionType.Equal:
                                {
                                    if (leftObjectName == null)
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.IsNull(rightObjectName)
                                        );
                                    }
                                    else if (rightObjectName == null)
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.IsNull(leftObjectName)
                                        );
                                    }
                                    else
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.Equal(leftObjectName, rightObjectName)
                                        );
                                    }
                                }
                                break;
                            case ExpressionType.NotEqual:
                                {
                                    if (leftObjectName == null)
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.IsNotNull(rightObjectName)
                                        );
                                    }
                                    else if (rightObjectName == null)
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.IsNotNull(leftObjectName)
                                        );
                                    }
                                    else
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.NotEqual(leftObjectName, rightObjectName)
                                        );
                                    }
                                }
                                break;
                            default:
                                {
                                    leftObjectName = leftObjectName ?? MAData.Sql.NULL;
                                    rightObjectName = rightObjectName ?? MAData.Sql.NULL;
                                    if (binaryExpression.NodeType == ExpressionType.GreaterThan)
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.GreaterThan(leftObjectName, rightObjectName)
                                        );
                                    }
                                    else if (binaryExpression.NodeType == ExpressionType.GreaterThanOrEqual)
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.GreaterThanOrEqual(leftObjectName, rightObjectName)
                                        );
                                    }
                                    else if (binaryExpression.NodeType == ExpressionType.LessThan)
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.LessThan(leftObjectName, rightObjectName)
                                        );
                                    }
                                    else if (binaryExpression.NodeType == ExpressionType.LessThanOrEqual)
                                    {
                                        whereQueries.Add(
                                            MAData.Sql.LessThanOrEqual(leftObjectName, rightObjectName)
                                        );
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }
        else if (expression is MethodCallExpression)
        {
            var methodCallExpression = (MethodCallExpression)expression;
            switch (methodCallExpression.Method.Name)
            {
                case "Contains":
                    {
                        var objectName = "";
                        var containObjectName = "";
                        methodCallExpression.Object.ToValue(parameters, ref indexObject, ref objectName, true, parametersObjectNameAdd: parametersObjectNameAdd);
                        methodCallExpression.Arguments[0].ToValue(parameters, ref indexObject, ref containObjectName, true, parametersObjectNameAdd: parametersObjectNameAdd);
                        containObjectName = MAData.Sql.ConcatString("'%'", containObjectName, "'%'");

                        if (not)
                        {
                            whereQueries.Add(
                                MAData.Sql.NotLike(objectName, containObjectName)
                                );
                        }
                        else
                        {
                            whereQueries.Add(
                                MAData.Sql.Like(objectName, containObjectName)
                                );
                        }
                    }
                    break;
                case "StartsWith":
                    {
                        var objectName = "";
                        var containObjectName = "";
                        methodCallExpression.Object.ToValue(parameters, ref indexObject, ref objectName, true, parametersObjectNameAdd: parametersObjectNameAdd);
                        methodCallExpression.Arguments[0].ToValue(parameters, ref indexObject, ref containObjectName, true, parametersObjectNameAdd: parametersObjectNameAdd);
                        containObjectName = MAData.Sql.ConcatString(containObjectName, "'%'");

                        if (not)
                        {
                            whereQueries.Add(
                                MAData.Sql.NotLike(objectName, containObjectName)
                                );
                        }
                        else
                        {
                            whereQueries.Add(
                                MAData.Sql.Like(objectName, containObjectName)
                                );
                        }
                    }
                    break;
                case "EndsWith":
                    {
                        var objectName = "";
                        var containObjectName = "";
                        methodCallExpression.Object.ToValue(parameters, ref indexObject, ref objectName, true, parametersObjectNameAdd: parametersObjectNameAdd);
                        methodCallExpression.Arguments[0].ToValue(parameters, ref indexObject, ref containObjectName, true, parametersObjectNameAdd: parametersObjectNameAdd);
                        containObjectName = MAData.Sql.ConcatString("'%'", containObjectName);

                        if (not)
                        {
                            whereQueries.Add(
                                MAData.Sql.NotLike(objectName, containObjectName)
                                );
                        }
                        else
                        {
                            whereQueries.Add(
                                MAData.Sql.Like(objectName, containObjectName)
                                );
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        else if (expression is UnaryExpression)
        {
            var unaryExpression = (UnaryExpression)expression;
            unaryExpression.Operand.ToIfOneOnly(whereQueries, parameters, ref indexObject, unaryExpression.NodeType == ExpressionType.Not, parametersObjectNameAdd: parametersObjectNameAdd);
        }
        else if (expression is LambdaExpression)
        {
            var lambdaExpression = (LambdaExpression)expression;
            lambdaExpression.Body.ToIfOneOnly(whereQueries, parameters, ref indexObject, parametersObjectNameAdd: parametersObjectNameAdd);
        }
    }

    internal static object GetMemberValue(MemberExpression expression)
    {
        return Expression.Lambda<Func<object>>(
                            Expression.Convert(expression, typeof(object))
                        ).Compile()();
    }

    internal static void ToValues(this Expression expression, List<MAEntityParameter> parameters, ref int indexObject, ref string[] objectNames)
    {
        if (expression is LambdaExpression && ((LambdaExpression)expression).Body.NodeType == ExpressionType.New)
        {
            var lambdaExpression = (LambdaExpression)expression;
            lambdaExpression.Body.ToValues(parameters, ref indexObject, ref objectNames);
        }
        else if (expression.NodeType == ExpressionType.New)
        {
            var newExpression = (NewExpression)expression;
            var columnNames = new List<string>();
            foreach (var argument in newExpression.Arguments)
            {
                string objectName = "";
                argument.ToValue(parameters, ref indexObject, ref objectName, true);
                columnNames.Add(objectName);
            }
            objectNames = columnNames.ToArray();
        }
        else
        {
            string objectName = "";
            expression.ToValue(parameters, ref indexObject, ref objectName, true);
            objectNames = new string[]
                {
                    objectName
                };
        }
    }

    internal static void ToValue(this Expression expression, List<MAEntityParameter> parameters, ref int indexObject, ref string objectName, bool nullConvert, Dictionary<Type, string> parametersObjectNameAdd = null)
    {
        switch (expression.NodeType)
        {
            case ExpressionType.MemberAccess:
                {
                    var memberExpression = (MemberExpression)expression;
                    if (memberExpression.Expression == null)
                    {
                        objectName = getObjectNameByValue(parameters, ref indexObject, GetMemberValue(memberExpression));
                    }
                    else if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
                    {
                        switch (memberExpression.Member.Name)
                        {
                            case "Length":
                                {
                                    memberExpression.Expression.ToValue(parameters, ref indexObject, ref objectName, nullConvert, parametersObjectNameAdd);
                                    objectName = MAData.Sql.Length(objectName);
                                }
                                break;
                            case "Date":
                                {
                                    memberExpression.Expression.ToValue(parameters, ref indexObject, ref objectName, nullConvert, parametersObjectNameAdd);
                                    objectName = MAData.Sql.Cast(objectName, MAData.Sql.Date);
                                }
                                break;
                            case "Year":
                                {
                                    memberExpression.Expression.ToValue(parameters, ref indexObject, ref objectName, nullConvert, parametersObjectNameAdd);
                                    objectName = MAData.Sql.Year(objectName);
                                }
                                break;
                            case "Month":
                                {
                                    memberExpression.Expression.ToValue(parameters, ref indexObject, ref objectName, nullConvert, parametersObjectNameAdd);
                                    objectName = MAData.Sql.Month(objectName);
                                }
                                break;
                            case "Day":
                                {
                                    memberExpression.Expression.ToValue(parameters, ref indexObject, ref objectName, nullConvert, parametersObjectNameAdd);
                                    objectName = MAData.Sql.Day(objectName);
                                }
                                break;
                            case "Value":
                                {
                                    memberExpression.Expression.ToValue(parameters, ref indexObject, ref objectName, nullConvert, parametersObjectNameAdd);
                                }
                                break;
                            default:
                                {
                                    var attrs = memberExpression.Member.GetCustomAttributes(typeof(MADataNameAttribute), false);
                                    if (attrs.Length > 0)
                                    {
                                        var attr = (MADataNameAttribute)attrs[0];
                                        objectName = getObjectNameByValue(parameters, ref indexObject, GetMemberValue(memberExpression));
                                        objectName = "(" + attr.Name.Replace("{parameter}", objectName) + ")";
                                    }
                                    else
                                    {
                                        objectName = getObjectNameByValue(parameters, ref indexObject, GetMemberValue(memberExpression));
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (parametersObjectNameAdd == null)
                            objectName = memberExpression.Member.Name;
                        else
                            objectName = parametersObjectNameAdd[((ParameterExpression)memberExpression.Expression).Type] + "." + memberExpression.Member.Name;
                    }
                }
                break;
            case ExpressionType.Constant:
                {
                    var constantExpression = (ConstantExpression)expression;
                    objectName = getObjectNameByValue(parameters, ref indexObject, constantExpression.Value);
                }
                break;
            case ExpressionType.Conditional:
                {
                    var conditionalExpression = (ConditionalExpression)expression;
                    //_conditionalExpression
                    List<string> inlineWhereQueries = new List<string>();
                    conditionalExpression.Test.ToIfOneOnly(inlineWhereQueries, parameters, ref indexObject);

                    string trueObjectName = "";
                    string falseObjectName = "";

                    conditionalExpression.IfTrue.ToValue(parameters, ref indexObject, ref trueObjectName, true, parametersObjectNameAdd);
                    conditionalExpression.IfFalse.ToValue(parameters, ref indexObject, ref falseObjectName, true, parametersObjectNameAdd);

                    objectName = MAData.Sql.Case(MAData.Sql.WhereAnd(inlineWhereQueries.ToArray()), trueObjectName, falseObjectName);
                }
                break;
            case ExpressionType.Lambda:
                {
                    var lambdaExpression = (LambdaExpression)expression;
                    lambdaExpression.Body.ToValue(parameters, ref indexObject, ref objectName, nullConvert, parametersObjectNameAdd);
                }
                break;
            case ExpressionType.Add:
                {
                    var binaryExpression = (BinaryExpression)expression;
                    string leftObjectName = "";
                    string rightObjectName = "";

                    binaryExpression.Left.ToValue(parameters, ref indexObject, ref leftObjectName, true, parametersObjectNameAdd);
                    binaryExpression.Right.ToValue(parameters, ref indexObject, ref rightObjectName, true, parametersObjectNameAdd);
                    objectName = MAData.Sql.ConcatString(leftObjectName, rightObjectName);
                }
                break;
            case ExpressionType.Divide:
                {
                    var binaryExpression = (BinaryExpression)expression;
                    string leftObjectName = "";
                    string rightObjectName = "";

                    binaryExpression.Left.ToValue(parameters, ref indexObject, ref leftObjectName, true, parametersObjectNameAdd);
                    binaryExpression.Right.ToValue(parameters, ref indexObject, ref rightObjectName, true, parametersObjectNameAdd);
                    objectName = MAData.Sql.DivideValues(leftObjectName, rightObjectName);
                }
                break;
            case ExpressionType.Multiply:
                {
                    var binaryExpression = (BinaryExpression)expression;
                    string leftObjectName = "";
                    string rightObjectName = "";

                    binaryExpression.Left.ToValue(parameters, ref indexObject, ref leftObjectName, true, parametersObjectNameAdd);
                    binaryExpression.Right.ToValue(parameters, ref indexObject, ref rightObjectName, true, parametersObjectNameAdd);
                    objectName = MAData.Sql.MultiplyValues(leftObjectName, rightObjectName);
                }
                break;
            case ExpressionType.Modulo:
                {
                    var binaryExpression = (BinaryExpression)expression;
                    string leftObjectName = "";
                    string rightObjectName = "";

                    binaryExpression.Left.ToValue(parameters, ref indexObject, ref leftObjectName, true, parametersObjectNameAdd);
                    binaryExpression.Right.ToValue(parameters, ref indexObject, ref rightObjectName, true, parametersObjectNameAdd);
                    objectName = MAData.Sql.Modulo(leftObjectName, rightObjectName);
                }
                break;
            case ExpressionType.Subtract:
                {
                    var binaryExpression = (BinaryExpression)expression;
                    string leftObjectName = "";
                    string rightObjectName = "";

                    binaryExpression.Left.ToValue(parameters, ref indexObject, ref leftObjectName, true, parametersObjectNameAdd);
                    binaryExpression.Right.ToValue(parameters, ref indexObject, ref rightObjectName, true, parametersObjectNameAdd);
                    objectName = MAData.Sql.SubtractValues(leftObjectName, rightObjectName);
                }
                break;
            case ExpressionType.Convert:
                {
                    var unaryExpression = (UnaryExpression)expression;
                    unaryExpression.Operand.ToValue(parameters, ref indexObject, ref objectName, nullConvert, parametersObjectNameAdd);
                }
                break;
            case ExpressionType.Call:
                {
                    var methodCallExpression = (MethodCallExpression)expression;
                    var columnName = "";
                    if (methodCallExpression.Object == null)
                        methodCallExpression.Arguments[0].ToValue(parameters, ref indexObject, ref columnName, nullConvert, parametersObjectNameAdd);
                    else
                        methodCallExpression.Object.ToValue(parameters, ref indexObject, ref columnName, nullConvert, parametersObjectNameAdd);
                    switch (methodCallExpression.Method.Name)
                    {
                        #region SQL
                        case "Count_MAEntity":
                        case "Count":
                            {
                                objectName = MAData.Sql.Count(columnName);
                            }
                            break;
                        case "Sum_MAEntity":
                            {
                                objectName = MAData.Sql.Sum(columnName);
                            }
                            break;
                        case "Max_MAEntity":
                            {
                                objectName = MAData.Sql.Max(columnName);
                            }
                            break;
                        case "Min_MAEntity":
                            {
                                objectName = MAData.Sql.Min(columnName);
                            }
                            break;
                        case "Trim":
                            {
                                objectName = MAData.Sql.Trim(columnName);
                            }
                            break;
                        #endregion
                        #region Convert
                        case "ToUpper":
                            {
                                objectName = MAData.Sql.Upper(columnName);
                            }
                            break;
                        case "ToLower":
                            {
                                objectName = MAData.Sql.Lower(columnName);
                            }
                            break;
                        case "ToByte_MAEntity":
                            {
                                objectName = MAData.Sql.Cast(columnName, MAData.Sql.Byte);
                            }
                            break;
                        case "ToInt16_MAEntity":
                            {
                                objectName = MAData.Sql.Cast(columnName, MAData.Sql.Int16);
                            }
                            break;
                        case "ToInt32_MAEntity":
                            {
                                objectName = MAData.Sql.Cast(columnName, MAData.Sql.Int32);
                            }
                            break;
                        case "ToInt64_MAEntity":
                            {
                                objectName = MAData.Sql.Cast(columnName, MAData.Sql.Int64);
                            }
                            break;
                        case "ToDecimal_MAEntity":
                            {
                                objectName = MAData.Sql.Cast(columnName, MAData.Sql.Decimal);
                            }
                            break;
                        case "ToDate_MAEntity":
                            {
                                objectName = MAData.Sql.Cast(columnName, MAData.Sql.Date);
                            }
                            break;
                        case "ToDateTime_MAEntity":
                            {
                                objectName = MAData.Sql.Cast(columnName, MAData.Sql.DateTime);
                            }
                            break;
                        case "ToString":
                            {
                                objectName = MAData.Sql.Cast(columnName, MAData.Sql.String);
                            }
                            break;
                        #endregion
                        #region Change Result Type
                        case "ChangeType_MAEntity":
                            {
                                objectName = columnName;
                            }
                            break;
                        #endregion
                        default:
                            {
                                var attrs = methodCallExpression.Method.GetCustomAttributes(typeof(MADataNameAttribute), false);
                                if (attrs.Length > 0)
                                {
                                    var attr = (MADataNameAttribute)attrs[0];
                                    objectName = attr.Name.Replace("{parameter}", columnName);
                                    var argumentIndex = 0;
                                    var foreachIndex = 0;
                                    foreach (var item in methodCallExpression.Arguments)
                                    {
                                        foreachIndex++;
                                        if (methodCallExpression.Object == null && foreachIndex == 1)
                                            continue;
                                        var argumentObjectName = "";
                                        item.ToValue(parameters, ref indexObject, ref argumentObjectName, nullConvert, parametersObjectNameAdd);
                                        objectName = objectName.Replace("{argument" + argumentIndex + "}", argumentObjectName);
                                        argumentIndex++;
                                    }
                                    objectName = "(" + objectName + ")";
                                }
                                else
                                    objectName = columnName;
                            }
                            break;
                    }
                }
                break;
            default:
                break;
        }

        if (nullConvert)
            objectName = objectName ?? MAData.Sql.NULL;
    }

    private static string getObjectNameByValue(List<MAEntityParameter> parameters, ref int indexObject, object value)
    {
        string objectName;
        if (value != null)
        {
            indexObject++;
            objectName = MAData.Sql.Parameter("parameter" + indexObject);
            parameters.Add(new MAEntityParameter(objectName, value));
        }
        else
        {
            objectName = null;
        }

        return objectName;
    }
    #endregion
}
