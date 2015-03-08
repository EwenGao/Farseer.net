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
            using (var context = new TableContext<UserPO>())
            {
                var info = context.TableSet.Where(o => o.ID > 0).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToInfo();
                info.PassWord = "123456";

                context.TableSet.Where(o => o.ID == 1).Update(info);

                context.TableSet.Insert(info);


                var lst = context.TableSet.Where(o => o.ID > 0).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToList();

                context.SaveChanges();
            }
        }
    }
}
