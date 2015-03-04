using System;
using System.Linq.Expressions;
using FS.Core.Model;
using FS.Mapping.Table;

namespace FS.Extend
{
    /// <summary>
    ///     Expression表达式树扩展
    /// </summary>
    public static class ExpressionExtend
    {
        /// <summary>
        ///     And 操作
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="left">左树</param>
        /// <param name="right">右树</param>
        public static Expression<Func<TInfo, bool>> AndAlso<TInfo>(this Expression<Func<TInfo, bool>> left,
                                                                   Expression<Func<TInfo, bool>> right)
            where TInfo : ModelInfo
        {
            if (left == null)
            {
                return right;
            }
            if (right == null)
            {
                return left;
            }

            var param = left.Parameters[0];
            return Expression.Lambda<Func<TInfo, bool>>(ReferenceEquals(param, right.Parameters[0]) ? Expression.AndAlso(left.Body, right.Body) : Expression.AndAlso(left.Body, Expression.Invoke(right, param)), param);
        }

        /// <summary>
        ///     OR 操作
        /// </summary>
        /// <typeparam name="TInfo">实体类</typeparam>
        /// <param name="left">左树</param>
        /// <param name="right">右树</param>
        public static Expression<Func<TInfo, bool>> OrElse<TInfo>(this Expression<Func<TInfo, bool>> left, Expression<Func<TInfo, bool>> right)
            where TInfo : ModelInfo
        {
            if (left == null)
            {
                return right;
            }

            var param = left.Parameters[0];
            return Expression.Lambda<Func<TInfo, bool>>(ReferenceEquals(param, right.Parameters[0]) ? Expression.OrElse(left.Body, right.Body) : Expression.OrElse(left.Body, Expression.Invoke(right, param)), param);
        }

        /// <summary>
        ///     获取字段名称
        /// </summary>
        /// <param name="select">字段名称</param>
        /// <returns></returns>
        public static string GetUsedName<T1, T2>(this Expression<Func<T1, T2>> select) where T1 : ModelInfo
        {
            MemberExpression memberExpression;

            var unary = select.Body as UnaryExpression;
            if (unary != null)
            {
                memberExpression = unary.Operand as MemberExpression;
            }
            else if (select.Body.NodeType == ExpressionType.Call)
            {
                memberExpression = (MemberExpression)((MethodCallExpression)select.Body).Object;
            }
            else
            {
                memberExpression = select.Body as MemberExpression;
            }

            var map = TableMapCache.GetMap<T1>();
            var modelInfo = map.GetModelInfo((memberExpression.Member).Name);

            return modelInfo.Value.Column.Name;
        }
    }
}