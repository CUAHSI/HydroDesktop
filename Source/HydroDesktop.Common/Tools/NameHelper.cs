using System;
using System.Linq.Expressions;

namespace HydroDesktop.Common.Tools
{
    /// <summary>
    /// Helper class to get properties names.
    /// </summary>
    public static class NameHelper
    {
        /// <summary>
        /// Get name of property in current class.
        /// </summary>
        public static string Name<TProp>(Expression<Func<TProp>> expression, bool isDeep = false)
        {
            return GetMemberName(expression.Body, isDeep);
        }

        internal static string GetMemberName(Expression expression, bool isDeep)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var memberName = memberExpression.Member.Name;
                    if (!isDeep)
                        return memberName;
                    var superPath = GetMemberName(memberExpression.Expression, true);
                    return !string.IsNullOrEmpty(superPath) ? superPath + "." + memberName : memberName;
                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return callExpression.Method.Name;
                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return GetMemberName(unaryExpression.Operand, isDeep);
                case ExpressionType.Parameter:
                case ExpressionType.Constant: //Change
                    return string.Empty;
                default:
                    throw new ArgumentException("The expression is not a member access or method call expression");
            }
        }
    }

    /// <summary>
    /// Helper class to get property name in the given class.
    /// </summary>
    /// <typeparam name="TSource">Class with properties.</typeparam>
    public static class NameHelper<TSource>
    {
        /// <summary>
        /// Get name of property in the given class.
        /// </summary>
        public static string Name<TProp>(Expression<Func<TSource, TProp>> expression, bool isDeep = false)
        {
            return NameHelper.GetMemberName(expression.Body, isDeep);
        }
    }
}
