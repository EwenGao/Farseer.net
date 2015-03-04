using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using FS.Extend;

namespace FS.Utils.Common
{
    /// <summary>
    ///     对象比较的实现
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class InfoComparer<TInfo, T> : IEqualityComparer<TInfo> where TInfo : class
    {
        private readonly Func<TInfo, T> keySelect;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="keySelect"></param>
        public InfoComparer(Func<TInfo, T> keySelect)
        {
            this.keySelect = keySelect;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(TInfo x, TInfo y)
        {
            return EqualityComparer<T>.Default.Equals(keySelect(x), keySelect(y));
        }

        /// <summary>
        /// HashCode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(TInfo obj)
        {
            return EqualityComparer<T>.Default.GetHashCode(keySelect(obj));
        }
    }

    /// <summary>
    /// DataGridView排序
    /// zgke@sina.com
    /// qq:116149
    /// </summary>
    public class DataGridViewComparer : IComparer
    {
        private readonly DataGridViewColumn column;

        /// <summary>
        /// dataGridView1.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.Descending; 根据这个进行排序列
        /// </summary>
        /// <param name="column"></param>
        public DataGridViewComparer(DataGridViewColumn column)
        {
            this.column = column;
        }

        int IComparer.Compare(Object x, Object y)
        {
            if (column == null) return -1;
            var _X = ((DataGridViewRow)x).Cells[column.Name].Value.ConvertType(0m);
            var _Y = ((DataGridViewRow)y).Cells[column.Name].Value.ConvertType(0m);

            var compareValue = _X.CompareTo(_Y);
            if (column.HeaderCell.SortGlyphDirection == SortOrder.Descending) return compareValue * -1;
            return compareValue;
        }
    }
}