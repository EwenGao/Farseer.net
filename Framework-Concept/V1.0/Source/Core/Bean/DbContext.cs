using FS.Configs;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Extend;
using FS.Core.Data;
using FS.Core.Model;
using System.Data.Entity;
using FS.ORM;

namespace FS.Core.Bean
{
    public class DbContext<TInfo> : System.Data.Entity.DbContext where TInfo : BaseInfo, new()
    {
        private DbContext(DbConnection existingConnection, bool contextOwnsConnection, int commandTimeout) : base(existingConnection, contextOwnsConnection) { base.Database.CommandTimeout = commandTimeout; }
        
        public DbSet<TInfo> Info { get; set; }

        public static DbContext<TInfo> CreateInstance()
        {
            var dbIndex = ModelCache.GetInfo(typeof(TInfo)).ClassInfo.DbIndex;
            DbInfo dbInfo = dbIndex;
            var Factory = DbProviderFactories.GetFactory(dbInfo.DataType.GetName());
            var conn = Factory.CreateConnection();
            conn.ConnectionString = DbFactory.CreateConnString(dbIndex);

            return new DbContext<TInfo>(conn, true, dbInfo.CommandTimeout);
        }
    }

}
