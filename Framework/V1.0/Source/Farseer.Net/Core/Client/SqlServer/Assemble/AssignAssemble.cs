using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Core.Context;
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

        public string Execute<TEntity>(TEntity entity, out IEnumerable<DbParameter> param) where TEntity : class,new()
        {
            // 要更新的字段
            param = DbProvider.GetParameter(entity);
            var sb = new StringBuilder();
            foreach (var parm in param) { sb.AppendFormat("{0} = {1} ,", DbProvider.KeywordAegis(parm.ParameterName.Substring(1)), parm.ParameterName); }
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : sb.ToString();
        }
    }
}