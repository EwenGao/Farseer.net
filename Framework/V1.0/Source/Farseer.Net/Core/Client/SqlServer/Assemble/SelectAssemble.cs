﻿using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Core.Infrastructure;

namespace FS.Core.Client.SqlServer.Assemble
{
    public class SelectAssemble   : SqlAssemble
    {
        public SelectAssemble(DbProvider dbProvider) : base(dbProvider) { }

        public string Execute(Expression exp)
        {
            return "";
        }
        public string Execute(List<Expression> exp)
        {
            return "";
        }
    }
}