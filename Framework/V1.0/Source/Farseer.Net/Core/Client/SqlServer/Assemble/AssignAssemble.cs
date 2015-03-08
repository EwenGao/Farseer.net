using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using FS.Core.Infrastructure;
using FS.Mapping.Table;

namespace FS.Core.Client.SqlServer.Assemble
{
    /// <summary>
    /// 组装赋值SQL
    /// </summary>
    public class AssignAssemble : SqlAssemble
    {
        public AssignAssemble(DbProvider dbProvider) : base(dbProvider) { }

        public string Execute<TEntity>(TEntity entity, out IList<DbParameter> param) where TEntity : class,new()
        {
            // 要更新的字段
            param = DbProvider.GetParameter(entity);

            #region 如果主键有值，则取消修改主键的SQL
            var map = TableMapCache.GetMap(entity);
            var indexHaveValue = map.GetModelInfo().Key != null && map.GetModelInfo().Key.GetValue(entity, null) != null;
            if (indexHaveValue)
            {
                ((List<DbParameter>)param).RemoveAll(o => o.ParameterName.Equals(DbProvider.ParamsPrefix + map.IndexName));
            }
            #endregion

            var sb = new StringBuilder();
            foreach (var parm in param) { sb.AppendFormat("{0} = {1} ,", DbProvider.KeywordAegis(parm.ParameterName.Substring(1)), parm.ParameterName); }
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
    }
}