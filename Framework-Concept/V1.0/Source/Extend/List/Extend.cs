using System.Collections.Generic;
using System.Linq;

namespace FS.Extend
{
    /// <summary>
    ///     Data扩展工具
    /// </summary>
    public static partial class ListExtend
    {
        /// <summary>
        ///     判断value是否存在于列表中
        /// </summary>
        /// <param name="lst">数据源</param>
        /// <param name="value">要判断的值</param>
        /// <returns></returns>
        public static bool Contains(this IEnumerator<int> lst, int? value)
        {
            return lst.Contains(value.GetValueOrDefault());
        }

        /// <summary>
        ///     数据分页
        /// </summary>
        /// <typeparam name="TSource">实体</typeparam>
        /// <param name="source">源对像</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        public static IQueryable<TSource> PageSplit<TSource>(this IQueryable<TSource> source, int pageSize = 20,
                                                             int pageIndex = 1)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 20;
            }
            return source.Skip(pageSize*(pageIndex - 1)).Take(pageSize);
        }
    }
}