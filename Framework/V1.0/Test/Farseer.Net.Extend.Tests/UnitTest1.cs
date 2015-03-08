using System;
using Demo.PO.Table.Members;
using FS.Core.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Extend.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            for (int i = 0; i < 1; i++)
            {
                var lst = TableContext<UserPO>.Data.Where(o => o.ID > 0).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToList();
            }
        }
    }
}
