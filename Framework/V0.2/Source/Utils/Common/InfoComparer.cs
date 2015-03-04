using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using FS.Core.Model;
using FS.Extend;

namespace FS.Utils.Common
{
    /// <summary>
    ///     对象比较的实现
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class InfoComparer<TInfo, T> : IEqualityComparer<TInfo> where TInfo : ModelInfo, new()
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
        private DataGridViewColumn m_Column;

        /// <summary>
        /// dataGridView1.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.Descending; 根据这个进行排序列
        /// </summary>
        /// <param name="p_Column"></param>
        public DataGridViewComparer(DataGridViewColumn p_Column)
        {
            m_Column = p_Column;
        }

        int IComparer.Compare(Object x, Object y)
        {
            if (m_Column == null) return -1;
            var _X = ((DataGridViewRow)x).Cells[m_Column.Name].Value.ConvertType(0m);
            var _Y = ((DataGridViewRow)y).Cells[m_Column.Name].Value.ConvertType(0m);

            var _CompareValue = _X.CompareTo(_Y);
            if (m_Column.HeaderCell.SortGlyphDirection == SortOrder.Descending) return _CompareValue * -1;
            return _CompareValue;
        }
    }
}