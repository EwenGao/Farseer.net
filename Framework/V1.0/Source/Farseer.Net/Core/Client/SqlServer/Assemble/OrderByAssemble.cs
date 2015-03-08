using System.Linq.Expressions;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.Assemble
{
    public class OrderByAssemble  : SqlAssemble
    {
        public OrderByAssemble(IQuery queryProvider) : base(queryProvider) { }

        public string Execute(Expression exp)
        {
            return "";
        }
    }
}
