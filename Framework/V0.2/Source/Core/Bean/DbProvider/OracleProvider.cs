using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using FS.Utils.Common;

namespace FS.Core.Bean
{
    /// <summary>
    ///     Oracle数据库提供者
    /// </summary>
    public class OracleProvider : DbProvider
    {
        /// <summary>
        /// 参数前缀
        /// </summary>
        public override string ParamsPrefix
        {
            get { return ":"; }
        }

        /// <summary>
        /// 获取Oracle数据库提供者
        /// </summary>
        public override DbProviderFactory GetDbProviderFactory
        {
            get { return DbProviderFactories.GetFactory("System.Data.OracleClient"); }
        }

        /// <summary>
        /// 数据库保护符
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public override string CreateTableAegis(string fieldName)
        {
            // 如果不是字段名，则直接返回
            if (!IsField(fieldName)) { return fieldName; }
            return fieldName;
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="valu"></param>
        /// <param name="type"></param>
        /// <param name="len"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override DbParameter CreateDbParam(string name, object valu, DbType type, int len, bool output = false)
        {
            if (valu.GetType() == typeof (byte[]))
            {
                var oracleParam = new OracleParameter(ParamsPrefix + name, OracleType.Blob, 0) {Value = valu};
                return oracleParam;
            }
            else if (type == DbType.String && (Str.Length(valu.ToString())*2) > (1024*4))
            {
                var oracleParam = new OracleParameter(ParamsPrefix + name, OracleType.Clob, valu.ToString().Length)
                                      {
                                          Value = valu
                                      };
                return oracleParam;
            }

            return base.CreateDbParam(name, valu, type, len, output);
        }
    }
}