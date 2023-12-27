using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DMI.StatDB.Persistence.Npgsql
{
    public static class ExpressionExtensions
    {
        private static Dictionary<string, string> _columnNameMap = new Dictionary<string, string>
        {
            { "GdbToDate", "gdb_to_date" },
            { "StationName", "stationname" },
            { "Stationtype", "stationtype" },
            { "StationOwner", "stationowner" },
            { "StationIDDMI", "stationid_dmi" },
            { "Status", "status" },
            { "Country", "country" },
            { "StatID", "station.statid" }
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
                    return notEqual.Left.ToMSSqlString() + " is not " +
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
                        case "StationName":
                        case "Status":
                        case "GdbToDate":
                        case "Country":
                            return _columnNameMap[memberName];
                        case "maxDate":
                            return "'9999-12-31 23:59:59'";
                        case "_nameFilterInUppercase":
                            return GetValue(memberaccess) as string;
                        case "_onlyCurrent":
                            return GetValue(memberaccess).ToString();
                        default:
                            throw new NotSupportedException();
                    }
                case ExpressionType.Call:
                    var call = expression as MethodCallExpression;
                    return call.ConvertToMSSqlString();
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

        private static string ConvertToMSSqlString(
            this MethodCallExpression callExpression)
        {
            var str = callExpression.ToString();

            if (str.Contains(".filter.Contains("))
            {
                var memberaccess = callExpression.Object as MemberExpression;
                var parameter = callExpression.Arguments[0].ToString();
                var property = parameter.Split('.')[1];

                //if (str.Contains("Status"))
                //{
                //    var list = GetValue(memberaccess) as List<Status>;
                //    return $"{_columnNameMap[property]} in ({list.Select(n => $"{(int)n}").Aggregate((c, n) => $"{c},{n}")})";
                //}
                //else if (str.Contains("Stationtype"))
                //{
                //    var list = GetValue(memberaccess) as List<StationType>;
                //    return $"{_columnNameMap[property]} in ({list.Select(n => $"{(int)n}").Aggregate((c, n) => $"{c},{n}")})";
                //}
                //else if (str.Contains("StationOwner"))
                //{
                //    var list = GetValue(memberaccess) as List<StationOwner>;
                //    return $"{_columnNameMap[property]} in ({list.Select(n => $"{(int)n}").Aggregate((c, n) => $"{c},{n}")})";
                //}
            }

            if (str.Contains(".ToUpper().Contains("))
            {
                var parameter = str.Split('.')[1];
                var columnName = _columnNameMap[parameter];

                var expression = callExpression.Arguments[0];
                var memberaccess = expression as MemberExpression;
                var member = memberaccess.Member;
                var memberName = member.Name;
                var value = GetValue(memberaccess);
                return $"{columnName} ILIKE '%{value}%'";
            }

            if (str.Contains(".ToString().Contains("))
            {
                var parameter = str.Split('.')[1];
                var columnName = _columnNameMap[parameter];

                var expression = callExpression.Arguments[0];
                var memberaccess = expression as MemberExpression;
                var member = memberaccess.Member;
                var memberName = member.Name;
                var value = GetValue(memberaccess);
                return $"{columnName}::TEXT LIKE '%{value}%'";
            }

            throw new InvalidOperationException();
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
