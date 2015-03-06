using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FS.Core.Infrastructure
{
    public interface IAssemble
    {
        string Execute(Expression exp);
    }
}
