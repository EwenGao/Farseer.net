using System.Collections.Generic;
using System.Data.Common;
using FS.Core.Model;

namespace FS.Core.Bean
{
    /// <summary>
    ///     返回执行结果
    /// </summary>
    public class DataResult<TInfo> where TInfo : ModelInfo, new()
    {
        /// <summary>
        ///     执行的Sql
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        ///     Sql生成时间
        /// </summary>
        public double SqlBuildTime { get; set; }

        /// <summary>
        ///     操作数据时间
        /// </summary>
        public double OperateDataTime { get; set; }

        /// <summary>
        ///     转到实体用时
        /// </summary>
        public double ToModelTime { get; set; }

        /// <summary>
        ///     参数列表
        /// </summary>
        public List<DbParameter> ParmsList { get; set; }
    }
}