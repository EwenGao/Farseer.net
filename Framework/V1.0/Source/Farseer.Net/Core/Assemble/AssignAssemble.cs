using FS.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace FS.Core.Assemble
{
    public class AssignAssemble : IAssemble
    {
        public string Execute<T>(T entity) where T : class
        {
            return "";
        }
    }
}
