using Demo.PO.Table;
using Demo.PO.Table.Members;
using FS.Core.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace 单元测试
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TableContextMethod()
        {
            // 通过TableContext上下文，动态创建User表的映射
            TableContext<UserPO>.Data.Select(o => new { o.ID }).Where(o => o.ID == 1).Desc(o => new { o.ID, o.UserName }).ToList();
            UserPO.Data.Select(o => new { o.ID }).Where(o => o.ID == 1).ToList();


            using (var db = new TableContext<UserPO>())
            {
                db.TableSet.Where(o => o.ID != 2).Update();
                db.TableSet.Where(o => o.ID != 2).ToList();
                db.TableSet.Where(o => o.ID == 2).Insert();
                db.TableSet.Where(o => o.ID != 2).ToInfo();

                db.SaveChanges();
            }

            // 通过自定义TableContainer（继承TableContext），创建User表的映射
            //TableContainer.Instance.User.Select(o => o.ID).ToList();

            // 仿EF的操作。
            //    using (var db = TableContainer.Instance)
            //    {
            //        db.User.Select(o => o.ID == 1).ToList();
            //        db.User.Select(o => o.ID == 1).Where(o => o.ID > 2).ToInfo();

            //        db.User.Insert(xxx);
            //        db.User.Update(xxx);
            //        db.User.Update(xxx);
            //        db.User.Update(xxx);
            //        db.User.Update(xxx);
            //        db.User.Delete(xxx);

            //        //db.Add
            //        db.SaveChanges();
            //    }

            //    // Farseer.Net V0.1操作方式，UserPO被强继承BaseModel
            //    UserPO.Data.Select(o => o.ID == 1).ToList();
        }
    }
}