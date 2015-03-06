using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FS.Core.Infrastructure
{
    /// <summary>
    /// 生成Where、Select、OrderBy、Assign的部份SQL
    /// </summary>
    public interface IAssemble
    {
        string Execute(Expression exp);
    }
}
