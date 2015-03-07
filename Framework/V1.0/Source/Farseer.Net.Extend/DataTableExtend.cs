using System;
using System.Collections.Generic;
using System.Data;
using FS.Mapping.Table;

namespace FS.Extend
{
    /// <summary>
    ///     DataTable扩展工具
    /// </summary>
    public static class DataTableExtend
    {
        /// <summary>
        ///     对DataTable排序
        /// </summary>
        /// <param name="dt">要排序的表</param>
        /// <param name="sort">要排序的字段</param>
        public static DataTable Sort(this DataTable dt, string sort = "ID DESC")
        {
            var rows = dt.Select("", sort);
            var tmpDt = dt.Clone();

            foreach (var row in rows)
            {
                tmpDt.ImportRow(row);
            }
            return tmpDt;
        }

        /// <summary>
        ///     对DataTable分页
        /// </summary>
        /// <param name="dt">源表</param>
        /// <param name="pageSize">每页显示的记录数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public static DataTable Split(this DataTable dt, int pageSize = 20, int pageIndex = 1)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 1;
            }
            var dtNew = dt.Clone();

            if (dt != null)
            {
                int firstIndex;
                int endIndex;

                #region 计算 开始索引

                if (pageIndex == 1)
                {
                    firstIndex = 0;
                }
                else
                {
                    firstIndex = pageSize * (pageIndex - 1);
                    //索引超出记录总数时，返回空的表格
                    if (firstIndex > dt.Rows.Count)
                    {
                        return dtNew;
                    }
                }

                #endregion

                #region 计算 结束索引

                endIndex = pageSize + firstIndex;
                if (endIndex > dt.Rows.Count)
                {
                    endIndex = dt.Rows.Count;
                }

                #endregion

                for (var i = firstIndex; i < endIndex; i++)
                {
                    dtNew.ImportRow(dt.Rows[i]);
                }
            }
            return dtNew;
        }

        /// <summary>
        ///     DataTable倒序
        /// </summary>
        /// <param name="dt">源DataTable</param>
        public static DataTable Reverse(this DataTable dt)
        {
            var rows = dt.Select("");
            var tmpDt = dt.Clone();

            for (var i = dt.Rows.Count - 1; i >= 0; i--)
            {
                tmpDt.ImportRow(dt.Rows[i]);
            }
            return tmpDt;
        }

        /// <summary>
        ///     DataTable深度复制
        /// </summary>
        /// <param name="dt">要排序的表</param>
        public static DataTable CloneData(this DataTable dt)
        {
            var newTable = dt.Clone();
            dt.Rows.ToRows().ForEach(o => newTable.ImportRow(o));
            return newTable;
        }

        /// <summary>
        ///     DataTable转换为实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="T">实体类</typeparam>
        public static List<TResult> ToList<TResult>(this DataTable dt) where TResult : class,new()
        {
            var list = new List<TResult>();
            var map = TableMapCache.GetMap<TResult>();
            TResult t;
            foreach (DataRow dr in dt.Rows)
            {
                t = new TResult();

                //赋值字段
                foreach (var kic in map.ModelList)
                {
                    if (kic.Key.CanWrite && dr.Table.Columns.Contains(kic.Value.Column.Name))
                    {
                        kic.Key.SetValue(t, dr[kic.Value.Column.Name].ConvertType(kic.Key.PropertyType), null);
                    }
                }
                list.Add(t);
            }
            return list;
        }


        /// <summary>
        ///     DataTable转换为实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="T">实体类</typeparam>
        public static List<T> ToList2<T>(this DataTable dt)
        {
            var list = new List<T>();
            var ht = typeof(T);
            foreach (DataRow dr in dt.Rows)
            {
                var t = (T)Activator.CreateInstance(ht, dr);
                list.Add(t);
            }
            dt.Dispose();
            return list;
        }
    }
}