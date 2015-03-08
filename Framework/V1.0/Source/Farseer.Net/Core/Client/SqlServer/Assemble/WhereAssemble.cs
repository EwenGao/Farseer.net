using System.Linq.Expressions;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.Assemble
{
    public class WhereAssemble  : SqlAssemble
    {
        public WhereAssemble(DbProvider dbProvider) : base(dbProvider) { }

        public string Execute(Expression exp)
        {
            return "";
        }
    }
}
