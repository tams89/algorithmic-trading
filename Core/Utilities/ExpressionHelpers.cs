using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Utilities
{
    public static class ExpressionHelpers<TEntity>
    {
        /// <summary>
        /// Check to see if the memberExpressions specified properties type matches that of the member value.
        /// </summary>
        /// <param name="memberExpression">The expression</param>
        /// <param name="memberValue">The value</param>
        public static void CheckMemberExpression(Expression<Func<TEntity, object>> memberExpression, dynamic memberValue)
        {
            var memberBody = memberExpression.Body as MemberExpression;
            var unaryBody = memberExpression.Body as UnaryExpression;
            if (memberBody == null && unaryBody == null)
                throw new ArgumentNullException("memberExpression");

            if (memberBody != null)
            {
                if (((PropertyInfo)memberBody.Member).PropertyType != memberValue.GetType())
                    throw new ArgumentException("memberValue type does not equal memberExpression type");
            }

            if (unaryBody != null)
            {
                if (unaryBody.Operand.Type != memberValue.GetType())
                    throw new ArgumentException("memberValue type does not equal memberExpression type");
            }
        }
    }
}
