using System.Linq.Expressions;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.Assemble
{
    public class OrderByAssemble  : SqlAssemble
    {
        public OrderByAssemble(DbProvider dbProvider) : base(dbProvider) { }

        public string Execute(Expression exp)
        {
            return "";
        }
    }
}
