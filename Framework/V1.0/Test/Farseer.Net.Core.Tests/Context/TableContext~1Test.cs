using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Demo.PO.Table.Members;
using FS.Core.Context;

namespace Farseer.Net.Core.Tests.Context
{
    [TestClass]
    public class TableContextTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            TableContext<UserPO>.Data.Select(o => o.ID).Where(o => o.ID > 0).ToList();
        }
    }
}
