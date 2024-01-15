using System;
using System.Collections.Generic;
using System.Linq.Expressions;
//using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.PostgreSQL
{
    public static class ExpressionExtensions
    {
        private static Dictionary<string, string> _columnNameMap = new Dictionary<string, string>
        {
            //{ "ObjectId", "objectid" }, // Table: General
            //{ "GdbToDate", "gdb_to_date" }, // Table: General
            //{ "StationName", "stationname" }, // Table: stationinformation
            //{ "Stationtype", "stationtype" }, // Table: stationinformation
            //{ "StationOwner", "stationowner" }, // Table: stationinformation
            //{ "StationIDDMI", "stationid_dmi" }, // Table: stationinformation
            //{ "Status", "status" }, // Table: General
            //{ "Name", "name" } // contactperson
            { "StatId", "statid" }
        };

        public static string ToMSSqlString(
            this Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    var add = expression as BinaryExpression;
                    return add.Left.ToMSSqlString() + " + " + add.Right.ToMSSqlString();
                case ExpressionType.Parameter:
                    var parameterExpression = expression as ParameterExpression;
                    return parameterExpression.Name;
                case ExpressionType.Constant:
                    var constant = expression as ConstantExpression;
                    if (constant.Type == typeof(string))
                    {
                        if (constant.Value == null)
                        {
                            return "null";
                        }

                        return $"'{constant.Value.ToString()}'";
                    }
                    else if (constant.Type == typeof(Boolean))
                    {
                        // Crap
                        return "NoFilter";
                    }
                    else if (constant.Type == typeof(int))
                    {
                        return $"{constant.ToString()}";
                    }
                    else if (constant.Type == typeof(int?))
                    {
                        if (constant.Value == null)
                        {
                            return "null";
                        }

                        return $"'{constant.Value.ToString()}'";
                    }
                    //else if (constant.Type == typeof(StationType?))
                    //{
                    //    if (constant.Value == null)
                    //    {
                    //        return "null";
                    //    }

                    //    return $"'{constant.Value.ToString()}'";
                    //}
                    //else if (constant.Type == typeof(StationOwner?))
                    //{
                    //    if (constant.Value == null)
                    //    {
                    //        return "null";
                    //    }

                    //    return $"'{constant.Value.ToString()}'";
                    //}
                    //else if (constant.Type == typeof(Status?))
                    //{
                    //    if (constant.Value == null)
                    //    {
                    //        return "null";
                    //    }

                    //    return $"'{constant.Value.ToString()}'";
                    //}
                    else
                    {
                        var b = (int)constant.Value;
                        return b.ToString();
                    }
                    throw new NotSupportedException();
                case ExpressionType.Equal:
                    var equal = expression as BinaryExpression;
                    return equal.Left.ToMSSqlString() + " = " +
                            equal.Right.ToMSSqlString();
                case ExpressionType.NotEqual:
                    var notEqual = expression as BinaryExpression;
                    //return notEqual.Left.ToMSSqlString() + " != " +
                    //        notEqual.Right.ToMSSqlString();
                    return notEqual.Left.ToMSSqlString() + " is not " + // men hvad vælter vi så??
                            notEqual.Right.ToMSSqlString();
                case ExpressionType.Not:
                    var not = expression as UnaryExpression;
                    var temp = not.Operand;
                    return "not " + temp.ToMSSqlString();
                case ExpressionType.Lambda:
                    var l = expression as LambdaExpression;
                    return l.Body.ToMSSqlString();
                case ExpressionType.MemberAccess:
                    var memberaccess = expression as MemberExpression;
                    var str = memberaccess.ToString();
                    if (str.Contains("HasValue"))
                    {
                        var column = _columnNameMap[str.Split('.')[1]];
                        return $"{column} is not null";
                    }
                    var member = memberaccess.Member;
                    var memberName = member.Name;
                    switch (memberName)
                    {
                        //case "StationIDDMI":
                        //case "StationName":
                        //case "Stationtype":
                        //case "StationOwner":
                        //case "Status":
                        //case "GdbToDate":
                        //case "ObjectId":
                        case "StatId":
                            return _columnNameMap[memberName];
                        //case "maxDate":
                        //    return "'9999-12-31 23:59:59'";
                        //case "_nameFilterInUppercase":
                        //    return GetValue(memberaccess) as string;
                        //case "objectIdFilter":
                        //    return GetValue(memberaccess).ToString();
                        //case "_onlyCurrent":
                        //    return GetValue(memberaccess).ToString();
                        case "statId":
                            return GetValue(memberaccess).ToString();
                        default:
                            throw new NotSupportedException();
                    }
                case ExpressionType.Call:
                    throw new NotImplementedException();
                    //var call = expression as MethodCallExpression;
                    //return call.ConvertToMSSqlString();
                case ExpressionType.Convert:
                    var a = expression as UnaryExpression;
                    return a.Operand.ToMSSqlString();
                case ExpressionType.AndAlso:
                    var logicalAnd = expression as BinaryExpression;
                    return logicalAnd.Left.ToMSSqlString() + " AND " +
                            logicalAnd.Right.ToMSSqlString();
                case ExpressionType.OrElse:
                    var logicalOr = expression as BinaryExpression;
                    return logicalOr.Left.ToMSSqlString() + " OR " +
                            logicalOr.Right.ToMSSqlString();
            }

            throw new NotImplementedException(
                expression.GetType().ToString() + " " +
                expression.NodeType.ToString());
        }

        private static object GetValue(
            MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();

            return getter();
        }
    }
}
