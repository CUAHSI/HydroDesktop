using System;
using System.Linq.Expressions;

namespace HydroDesktop.Common.Tools
{
    /// <summary>
    /// Helper to work with properties and names
    /// </summary>
    public static class NameHelper
    {
        private static string GetMemberName(Expression expression, bool isSuperClass = false)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var supername = GetMemberName(memberExpression.Expression);
                    if (String.IsNullOrEmpty(supername))
                        return memberExpression.Member.Name;
                    return isSuperClass ? String.Concat(supername, '.', memberExpression.Member.Name) : memberExpression.Member.Name;
                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return callExpression.Method.Name;
                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return GetMemberName(unaryExpression.Operand);
                case ExpressionType.Parameter:
                case ExpressionType.Constant: //Change
                    return String.Empty;
                default:
                    throw new ArgumentException("The expression is not a member access or method call expression");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Name<T, T2>(Expression<Func<T, T2>> expression, bool isSuperClass = false)
        {
            return GetMemberName(expression.Body, isSuperClass);
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Name<T>(Expression<Func<T>> expression, bool isSuperClass = false)
        {
            return GetMemberName(expression.Body, isSuperClass);
        }
    }
}
