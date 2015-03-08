using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// Sql片断组装
    /// </summary>
    public abstract class SqlAssemble
    {
        /// <summary>
        /// 数据库提供者（不同数据库的特性）
        /// </summary>
        protected IQuery QueryProvider { get; private set; }

        protected SqlAssemble(IQuery queryProvider)
        {
            QueryProvider = queryProvider;
        }
    }
}
