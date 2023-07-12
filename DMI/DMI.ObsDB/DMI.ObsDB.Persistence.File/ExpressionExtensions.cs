using System;
using System.Linq.Expressions;

namespace DMI.ObsDB.Persistence.File
{
    public static class ExpressionExtensions
    {
        public static object Analyze(
            this Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    throw new NotImplementedException();
                    //var add = expression as BinaryExpression;
                    //return add.Left.ToMSSqlString() + " + " + add.Right.ToMSSqlString();
                case ExpressionType.Parameter:
                    throw new NotImplementedException();
                    //var parameterExpression = expression as ParameterExpression;
                    //return parameterExpression.Name;
                case ExpressionType.Constant:
                    var constant = expression as ConstantExpression;
                    if (constant.Type == typeof(string))
                    {
                        if (constant.Value == null)
                        {
                            return "null";
                        }

                        return constant.Value;
                    }
                    //else if (constant.Type == typeof(Boolean))
                    //{
                    //    // Crap
                    //    return "NoFilter";
                    //}
                    else if (constant.Type == typeof(int))
                    {
                        return constant;
                    }
                    //else if (constant.Type == typeof(int?))
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
                case ExpressionType.Equal:
                    var equal = expression as BinaryExpression;
                    var left = equal.Left.Analyze();
                    var right = equal.Right.Analyze();
                    return $"{left} = {right}";
                case ExpressionType.NotEqual:
                    throw new NotImplementedException();
                    //var notEqual = expression as BinaryExpression;
                    //return notEqual.Left.ToMSSqlString() + " is not " + // men hvad vælter vi så??
                    //        notEqual.Right.ToMSSqlString();
                case ExpressionType.Not:
                    throw new NotImplementedException();
                    //var not = expression as UnaryExpression;
                    //var temp = not.Operand;
                    //return "not " + temp.ToMSSqlString();
                case ExpressionType.LessThan:
                    var lessThan = expression as BinaryExpression;
                    var lessThanLeft = lessThan.Left.Analyze();
                    var lessThanRight = lessThan.Right.Analyze();
                    return $"{lessThanLeft} < {lessThanRight}";
                case ExpressionType.LessThanOrEqual:
                    var lessThanOrEqual = expression as BinaryExpression;
                    var lessThanOrEqualLeft = lessThanOrEqual.Left.Analyze();
                    var lessThanOrEqualRight = lessThanOrEqual.Right.Analyze();
                    return $"{lessThanOrEqualLeft} <= {lessThanOrEqualRight}";
                case ExpressionType.GreaterThan:
                    var greaterThan = expression as BinaryExpression;
                    var greaterThanLeft = greaterThan.Left.Analyze();
                    var greaterThanRight = greaterThan.Right.Analyze();
                    return $"{greaterThanLeft} > {greaterThanRight}";
                case ExpressionType.GreaterThanOrEqual:
                    var greaterThanOrEqual = expression as BinaryExpression;
                    var greaterThanOrEqualLeft = greaterThanOrEqual.Left.Analyze();
                    var greaterThanOrEqualRight = greaterThanOrEqual.Right.Analyze();
                    return $"{greaterThanOrEqualLeft} >= {greaterThanOrEqualRight}";
                case ExpressionType.Lambda:
                    var l = expression as LambdaExpression;
                    return l.Body.Analyze();
                case ExpressionType.MemberAccess:
                    var memberaccess = expression as MemberExpression;
                    var member = memberaccess.Member;
                    var memberName = member.Name;
                    switch (memberName)
                    {
                        case "StatId":
                        case "ParamId":
                        case "Time":
                            return memberName;
                        default:
                            return GetValue(memberaccess);
                    }
                case ExpressionType.Call:
                    throw new NotImplementedException();
                    //var call = expression as MethodCallExpression;
                    //return call.ConvertToMSSqlString();
                case ExpressionType.Convert:
                    throw new NotImplementedException();
                    //var a = expression as UnaryExpression;
                    //return a.Operand.ToMSSqlString();
                case ExpressionType.AndAlso:
                    throw new NotImplementedException();
                    //var logicalAnd = expression as BinaryExpression;
                    //return logicalAnd.Left.ToMSSqlString() + " AND " +
                    //        logicalAnd.Right.ToMSSqlString();
                case ExpressionType.OrElse:
                    throw new NotImplementedException();
                    //var logicalOr = expression as BinaryExpression;
                    //return logicalOr.Left.ToMSSqlString() + " OR " +
                    //        logicalOr.Right.ToMSSqlString();
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
            var result = getter();

            return getter();
        }
    }
}
