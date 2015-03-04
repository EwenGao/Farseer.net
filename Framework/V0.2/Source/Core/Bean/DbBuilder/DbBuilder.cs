using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using FS.Core.Model;
using FS.Extend;
using FS.Mapping.Table;
using FS.Utils.Common;

namespace FS.Core.Bean
{
    /// <summary>
    ///     数据库Sql生成
    /// </summary>
    /// <typeparam name="TInfo"></typeparam>
    internal abstract class DbBuilder<TInfo> where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     实体类映射
        /// </summary>
        protected TableMap Map = typeof(TInfo);

        /// <summary>
        ///     条件参数
        /// </summary>
        internal List<DbParameter> ParamsList = new List<DbParameter>();

        /// <summary>
        ///     字段选择Sql
        /// </summary>
        protected StringBuilder SelectString = new StringBuilder();

        /// <summary>
        ///     排序Sql
        /// </summary>
        protected StringBuilder SortString = new StringBuilder();

        /// <summary>
        ///     条件Sql
        /// </summary>
        protected StringBuilder WhereString = new StringBuilder();

        /// <summary>
        ///     数据库提供者
        /// </summary>
        protected DbProvider dbProvider = DbFactory.CreateDbProvider<TInfo>();

        /// <summary>
        ///     数据库表达式树解析器
        /// </summary>
        protected DbVisit<TInfo> dbVisit = DbFactory.CreateDbVisit<TInfo>();

        /// <summary>
        ///     数据库Sql生成
        /// </summary>
        /// <param name="tableName">表名称</param>
        internal DbBuilder(string tableName = "")
        {
            TableName = tableName.IsNullOrEmpty() ? Map.ClassInfo.Name : tableName;
        }

        /// <summary>
        ///     数据库表名
        /// </summary>
        protected string TableName { get; set; }

        /// <summary>
        ///     获取该实体类的参数
        /// </summary>
        /// <param name="info">实体类</param>
        protected List<DbParameter> GetParameter(TInfo info)
        {
            var lst = new List<DbParameter>();

            foreach (var kic in Map.ModelList.Where(o => o.Value.IsDbField))
            {
                var obj = kic.Key.GetValue(info, null);
                if (obj == null) { continue; }

                #region List处理

                var type = obj.GetType();
                if (type.IsGenericType)
                {
                    if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                    {
                        obj = ((IList)obj).ToString(",");
                    }
                    else
                    {
                        if (type.GetGenericArguments()[0] == typeof(int))
                        {
                            obj = ((List<int?>)obj).ToString(",");
                        }
                    }
                }

                #endregion

                lst.Add(dbProvider.CreateDbParam(kic.Value.Column.Name, obj));
            }

            return lst;
        }

        internal void Where(Expression<Func<TInfo, bool>> exeWhere)
        {
            if (exeWhere == null) { return; }

            WhereString.Append(WhereString.Length == 0 ? "Where " : " AND ");

            List<DbParameter> lstParams;
            WhereString.Append(dbVisit.WhereTranslator(exeWhere, out lstParams));
            ParamsList.AddRange(lstParams);

            lstParams.Clear();
        }

        internal void WhereOr(Expression<Func<TInfo, bool>> exeWhere)
        {
            if (exeWhere == null) { return; }

            WhereString.Append(WhereString.Length == 0 ? "Where " : " OR ");

            List<DbParameter> lstParams;
            WhereString.Append(dbVisit.WhereTranslator(exeWhere, out lstParams));
            ParamsList.AddRange(lstParams);

            lstParams.Clear();
        }

        internal void Select(Expression select)
        {
            if (select == null) { return; }

            if (SelectString.Length > 0) { SelectString.Append(","); }
            SelectString.Append(dbVisit.SelectTranslator(select).ToString(",").DelEndOf(","));
        }

        internal void Asc(Expression sort)
        {
            if (sort == null) { return; }

            SortString.Append(SortString.Length == 0 ? "Order By " : ",");

            SortString.Append((dbVisit.OrderTranslator(sort).ToString(" ASC,") + " ASC").DelEndOf(","));
        }

        internal void Desc(Expression sort)
        {
            if (sort == null) { return; }

            SortString.Append(SortString.Length == 0 ? "Order By " : ",");

            SortString.Append((dbVisit.OrderTranslator(sort).ToString(" DESC,") + " DESC").DelEndOf(","));
        }

        public virtual string GetFields()
        {
            if (SelectString.Length > 0) { return SelectString.ToString(); }

            var str = new StrPlus();
            foreach (var item in Map.ModelList.Where(o => o.Value.IsDbField))
            {
                if (!dbProvider.IsField(item.Value.Column.Name))
                {
                    str += item.Value.Column.Name + " as " + item.Key.Name + ",";
                    continue;
                }
                str += dbProvider.CreateTableAegis(item.Value.Column.Name) + ",";
            }
            return str.Value.IsNullOrEmpty() ? "*" : str.DelLastChar(",");
        }

        public virtual string Insert(TInfo info)
        {
            ParamsList = GetParameter(info);
            if (ParamsList.Count == 0)
            {
                return string.Empty;
            }

            //要插入的表
            var sql = new StringBuilder();

            sql.AppendFormat("INSERT INTO {0} ", TableName);

            #region 要插入的值

            var strFields = new StringBuilder("(");
            var strValues = new StringBuilder("(");

            foreach (var param in ParamsList)
            {
                strFields.AppendFormat("{0},", dbProvider.CreateTableAegis(param.ParameterName.Substring(1)));
                strValues.AppendFormat("{0},", param.ParameterName);
            }

            sql.AppendFormat(strFields.ToString().DelEndOf(",") + ") VALUES " + strValues.ToString().DelEndOf(",") + ")");

            #endregion

            return sql.ToString() + ";";
        }

        public virtual string LastIdentity()
        {
            return "SELECT @@IDENTITY;";
        }

        public virtual string Count()
        {
            return string.Format("SELECT Count(0) FROM {0} {1};", TableName, WhereString);
        }

        public virtual string Delete()
        {
            return string.Format("DELETE FROM {0} {1};", TableName, WhereString);
        }

        public virtual string ToInfo()
        {
            return string.Format("select top 1 {0} from {1} {2} {3};", GetFields(), TableName, WhereString, SortString);
        }

        public virtual string ToTable(int pageSize, int pageIndex)
        {
            // 不分页
            if (pageIndex == 1)
            {
                return ToTable(pageSize);
            }

            if (SortString.Length == 0)
            {
                SortString.AppendFormat("ORDER BY {0} ASC", Map.IndexName);
            }
            var sort2 = SortString.ToString().Replace(" DESC", " [倒序]").Replace("ASC", "DESC").Replace("[倒序]", "ASC");

            return string.Format("SELECT TOP {1} {0} FROM (SELECT TOP {2} {0} FROM {3} {4} {5}) a  {6};",
                                 GetFields(), pageSize, pageSize * pageIndex, TableName, WhereString, SortString, sort2);
        }

        public virtual string ToTable(int top = 0)
        {
            var topString = top > 0 ? string.Format("TOP {0}", top) : string.Empty;
            return string.Format("SELECT {0} {1} FROM {2} {3} {4};", topString, GetFields(), TableName, WhereString,
                                 SortString);
        }

        public virtual string ToTableByRand(int top = 0)
        {
            var topString = top > 0 ? string.Format("TOP {0}", top) : string.Empty;
            return string.Format("SELECT {0} {1} FROM {2} {3} ORDER BY NEWID();", topString, GetFields(), TableName,
                                 WhereString);
        }

        public virtual string Update(TInfo info)
        {
            var param = GetParameter(info);
            if (param.Count == 0) return string.Empty;

            // 要更新的表
            var sql = new StringBuilder();
            sql.AppendFormat("UPDATE {0} SET ", TableName);

            // 要更新的字段
            foreach (var parm in param)
            {
                sql.AppendFormat("{0} = {1} ,", dbProvider.CreateTableAegis(parm.ParameterName.Substring(1)),
                                 parm.ParameterName);
            }

            ParamsList.AddRange(param);

            return sql.ToString().DelEndOf(",") + WhereString + ";";
        }

        public virtual string UpdateValue<T>(T fieldValue)
        {
            var lst = SelectString.ToString().ToList("");
            for (var i = 0; i < lst.Count; i++) { lst[i] = string.Format("{0} = {0} + {1},", lst[i], fieldValue); }
            return string.Format("UPDATE {0} SET {1} {2};", TableName, lst.ToString("").DelEndOf(","), WhereString);
        }

        public virtual string GetSum()
        {
            return string.Format("SELECT SUM({0}) FROM {1} {2};", SelectString, TableName, WhereString);
        }

        public virtual string GetMax()
        {
            return string.Format("SELECT MAX({0}) FROM {1} {2};", SelectString, TableName, WhereString);
        }

        public virtual string GetMin()
        {
            return string.Format("SELECT MIN({0}) FROM {1} {2};", SelectString, TableName, WhereString);
        }

        public virtual string GetValue()
        {
            return string.Format("SELECT TOP 1 {0} FROM {1} {2};", SelectString, TableName, WhereString);
        }

        public virtual string ResetIdentity()
        {
            throw new Exception("该数据库不支持此方法");
        }

        public void Clear()
        {
            ParamsList.Clear();
            SelectString.Clear();
            SortString.Clear();
            WhereString.Clear();
        }

        public override string ToString()
        {
            var whereStr = WhereString.ToString();
            whereStr = ParamsList.Aggregate(whereStr, (current, item) => current.Replace(item.ParameterName, item.Value.ToString()));
            return "TableName：" + TableName + "； Where：" + whereStr + "； Sort：" + SortString + "； Select：" + SelectString;
        }
    }
}