using System;
using Demo.PO.Table.Members;
using FS.Core.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.Context
{
    [TestClass]
    public class TableSetTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            for (var i = 0; i < 100; i++)
            {
                TableContext<UserPO>.Data.Select(o => o.ID).Where(o => o.ID > 0).ToList();
            }
        }
    }
}
