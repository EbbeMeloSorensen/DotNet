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
                    else if (constant.Type == typeof(int))
                    {
                        return constant;
                    }
                    else
                    {
                        var b = (int)constant.Value;
                        return b.ToString();
                    }
                case ExpressionType.Equal:
                    var equal = expression as BinaryExpression;
                    var equalLeft = equal.Left.Analyze() as string;
                    var equalRight = equal.Right.Analyze();
                    return new Predicate(equalLeft, Operator.Equal, equalRight);
                case ExpressionType.LessThan:
                    var lessThan = expression as BinaryExpression;
                    var lessThanLeft = lessThan.Left.Analyze() as string;
                    var lessThanRight = lessThan.Right.Analyze();
                    return new Predicate(lessThanLeft, Operator.LessThan, lessThanRight);
                case ExpressionType.LessThanOrEqual:
                    var lessThanOrEqual = expression as BinaryExpression;
                    var lessThanOrEqualLeft = lessThanOrEqual.Left.Analyze() as string;
                    var lessThanOrEqualRight = lessThanOrEqual.Right.Analyze();
                    return new Predicate(lessThanOrEqualLeft, Operator.LessThanOrEqual, lessThanOrEqualRight);
                case ExpressionType.GreaterThan:
                    var greaterThan = expression as BinaryExpression;
                    var greaterThanLeft = greaterThan.Left.Analyze() as string;
                    var greaterThanRight = greaterThan.Right.Analyze();
                    return new Predicate(greaterThanLeft, Operator.GreaterThan, greaterThanRight);
                case ExpressionType.GreaterThanOrEqual:
                    var greaterThanOrEqual = expression as BinaryExpression;
                    var greaterThanOrEqualLeft = greaterThanOrEqual.Left.Analyze() as string;
                    var greaterThanOrEqualRight = greaterThanOrEqual.Right.Analyze();
                    return new Predicate(greaterThanOrEqualLeft, Operator.GreaterThanOrEqual, greaterThanOrEqualRight);
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
                case ExpressionType.NotEqual:
                case ExpressionType.Not:
                case ExpressionType.Call:
                case ExpressionType.Convert:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Add:
                case ExpressionType.Parameter:
                    throw new NotImplementedException();
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
