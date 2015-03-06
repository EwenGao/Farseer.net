using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Core.Infrastructure
{
    public interface IQueryQueue
    {
        IQueryQueueExecute List { get; }
    }
}
