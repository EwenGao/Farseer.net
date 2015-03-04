using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using FS.Core.Model;
using FS.ORM;

namespace FS.Extend
{
    /// <summary>
    ///     Cate扩展工具
    /// </summary>
    public static partial class ListExtend
    {
        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="isContainsSub">是否获取子节点</param>
        /// <param name="ID">上级ID</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<int> GetSubIDList<TInfo>(this List<TInfo> lstCate, int? ID, bool isContainsSub = true, bool isAddMySelf = false) where TInfo : BaseCateInfo, new()
        {
            var lst = lstCate.GetSubList(ID, isContainsSub, isAddMySelf);
            return lst == null ? new List<int>() : lst.Select(o => o.ID).ToList();
        }

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isContainsSub">是否获取子节点</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<int> GetSubIDList<TInfo>(this List<TInfo> lstCate, string caption, bool isContainsSub = true, bool isAddMySelf = false) where TInfo : BaseCateInfo, new()
        {
            var lst = lstCate.GetSubList(caption, isContainsSub, isAddMySelf);
            return lst == null ? new List<int>() : lst.Select(o => o.ID).ToList();
        }

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="ID">上级ID</param>
        /// <param name="isContainsSub">是否获取子节点</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<TInfo> GetSubList<TInfo>(this List<TInfo> lstCate, int? ID, bool isContainsSub = true, bool isAddMySelf = false) where TInfo : BaseCateInfo, new()
        {
            var lst = new List<TInfo>();
            if (isAddMySelf)
            {
                var info = lstCate.FirstOrDefault(o => o.ID == ID);
                if (info != null) { lst.Add(info); }
            }

            foreach (var info in lstCate.Where(o => o.ParentID == ID).ToList())
            {
                lst.Add(info);
                if (!isContainsSub) { continue; }
                lst.AddRange(lstCate.GetSubList(info.ID, isContainsSub, false));
            }
            return lst;
        }

        /// <summary>
        ///     获取指定ParentID的ID列表
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isContainsSub">是否获取子节点</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<TInfo> GetSubList<TInfo>(this List<TInfo> lstCate, string caption, bool isContainsSub = true, bool isAddMySelf = false) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.Find(o => o.Caption.IsEquals(caption));
            return info == null ? new List<TInfo>() : lstCate.GetSubList(info.ID, isContainsSub, isAddMySelf);
        }

        /// <summary>
        ///     通过标题，获取分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetID<TInfo>(this List<TInfo> lstCate, string caption) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.Find(o => o.Caption.IsEquals(caption));
            return info == null ? 0 : info.ID;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetFirstID<TInfo>(this List<TInfo> lstCate, int? ID) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.GetFirstInfo(ID);
            return info == null ? 0 : info.ID;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="lstCate">分类列表</param>
        public static TInfo GetFirstInfo<TInfo>(this List<TInfo> lstCate, int? ID) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.FirstOrDefault(o => o.ID == ID);
            if (info == null) { return null; }

            if (lstCate.Count(o => o.ID == info.ParentID) > 0) { info = lstCate.GetFirstInfo(info.ParentID); }

            return info;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetFirstID<TInfo>(this List<TInfo> lstCate, string caption) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.GetFirstInfo(caption);
            return info == null ? 0 : info.ID;
        }

        /// <summary>
        ///     获取根节点分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="lstCate">分类列表</param>
        public static TInfo GetFirstInfo<TInfo>(this List<TInfo> lstCate, string caption) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.Find(o => o.Caption.IsEquals(caption));
            return info == null ? null : lstCate.GetFirstInfo(info.ParentID);
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetParentID<TInfo>(this List<TInfo> lstCate, int? ID) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.GetParentInfo(ID);
            return info == null ? 0 : info.ID;
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="lstCate">分类列表</param>
        public static TInfo GetParentInfo<TInfo>(this List<TInfo> lstCate, int? ID) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.FirstOrDefault(o => o.ID == ID);
            if (info != null) { info = lstCate.FirstOrDefault(o => o.ID == info.ParentID); }
            return info;
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="lstCate">分类列表</param>
        public static int GetParentID<TInfo>(this List<TInfo> lstCate, string caption) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.GetParentInfo(caption);
            return info == null ? 0 : info.ID;
        }

        /// <summary>
        ///     获取上一级分类数据
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="lstCate">分类列表</param>
        public static TInfo GetParentInfo<TInfo>(this List<TInfo> lstCate, string caption) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.Find(o => o.Caption.IsEquals(caption));
            return info == null ? null : lstCate.GetParentInfo(info.ID);
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<int> GetParentIDList<TInfo>(this List<TInfo> lstCate, int? ID, bool isAddMySelf = false) where TInfo : BaseCateInfo, new()
        {
            var lst = lstCate.GetParentList(ID, isAddMySelf);
            return lst == null ? new List<int>() : lst.Select(o => o.ID).ToList();
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="ID">当前分类数据ID</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<TInfo> GetParentList<TInfo>(this List<TInfo> lstCate, int? ID, bool isAddMySelf = false) where TInfo : BaseCateInfo, new()
        {
            var lst = new List<TInfo>();
            var info = lstCate.FirstOrDefault(o => o.ID == ID);
            if (info == null) { return lst; }

            lst.AddRange(lstCate.GetParentList(info.ParentID, true));
            if (isAddMySelf) { lst.Add(info); }
            return lst;
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<int> GetParentIDList<TInfo>(this List<TInfo> lstCate, string caption, bool isAddMySelf = false) where TInfo : BaseCateInfo, new()
        {
            var lst = lstCate.GetParentList(caption, isAddMySelf);
            return lst == null ? new List<int>() : lst.Select(o => o.ID).ToList();
        }

        /// <summary>
        ///     获取所有上级分类数据（从第一级往下排序）
        /// </summary>
        /// <param name="caption">分类标题</param>
        /// <param name="isAddMySelf">是否添加自己</param>
        /// <param name="lstCate">分类列表</param>
        public static List<TInfo> GetParentList<TInfo>(this List<TInfo> lstCate, string caption, bool isAddMySelf = false) where TInfo : BaseCateInfo, new()
        {
            var info = lstCate.Find(o => o.Caption.IsEquals(caption));
            return info == null ? new List<TInfo>() : lstCate.GetParentList(info.ID, isAddMySelf);
        }

        /// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="parentID">所属上级节点</param>
        /// <param name="isUsePrefix">是否需要加上前缀</param>
        /// <param name="lstCate">分类列表</param>
        public static void Bind<TInfo>(this List<TInfo> lstCate, DropDownList ddl, int selectedValue, int parentID, bool isUsePrefix = true) where TInfo : BaseCateInfo, new()
        {
            ddl.Items.Clear();

            lstCate.Bind(ddl, parentID, 0, null, false, isUsePrefix);

            if (selectedValue > 0) { ddl.SelectedItems(selectedValue); }
        }

        /// <summary>
        ///     绑定到DropDownList
        /// </summary>
        /// <param name="ddl">要绑定的ddl控件</param>
        /// <param name="selectedValue">默认选则值</param>
        /// <param name="where">筛选条件</param>
        /// <param name="isContainsSub">筛选条件是否包含子节点</param>
        /// <param name="isUsePrefix">是否需要加上前缀</param>
        /// <param name="lstCate">分类列表</param>
        public static void Bind<TInfo>(this List<TInfo> lstCate, DropDownList ddl, int selectedValue = 0, Func<TInfo, bool> where = null, bool isContainsSub = false, bool isUsePrefix = true) where TInfo : BaseCateInfo, new()
        {
            ddl.Items.Clear();

            lstCate.Bind(ddl, 0, 0, where, isContainsSub, isUsePrefix);

            if (selectedValue > 0) { ddl.SelectedItems(selectedValue); }
        }

        /// <summary>
        ///     递归绑定
        /// </summary>
        private static void Bind<TInfo>(this List<TInfo> lstCate, DropDownList ddl, int parentID, int tagNum, Func<TInfo, bool> where, bool isContainsSub, bool isUsePrefix) where TInfo : BaseCateInfo, new()
        {
            List<TInfo> lst;

            lst = lstCate.FindAll(o => o.ParentID == parentID);
            if (lst == null || lst.Count == 0) { return; }

            if ((parentID == 0 || isContainsSub) && where != null) { lst = lst.Where(where).ToList(); }
            if (lst == null || lst.Count == 0) { return; }

            foreach (var info in lst)
            {
                var text = isUsePrefix ? new string('　', tagNum) + "├─" + info.Caption : info.Caption;

                ddl.Items.Add(new ListItem { Value = info.ID.ToString(), Text = text });
                lstCate.Bind(ddl, info.ID, tagNum + 1, where, isContainsSub, isUsePrefix);
            }
        }
    }
}