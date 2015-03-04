using System;
using FS.Core.Page;
using FS.Extend;
using FS.Model.Members;
using FS.Utils.Web;
using FS.Core.Data;
using FS.Core.Bean;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using FS.Utils.Common;
using FS.Model;
using FS.Model.LINQ;
using System.Linq;

public partial class Speed : BasePage
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    public const string connString = "User ID=sa;Password=123456;Pooling=true;Data Source=.;Initial Catalog=Farseer;Min Pool Size=16;Max Pool Size=100;Connect Timeout=30;";
    public const int opCount = 100;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 做一次数据库的交互，初始化下
            Run(1);
            btnRun.Text += opCount + " 次测试";
        }

        btnRun.Click += btnRun_Click;
    }

    void btnRun_Click(object sender, EventArgs e)
    {
        Run(opCount);
    }

    void Run(int count)
    {
        litAdoInsert.Text = Ado_Insert(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litDbExecutorInsert.Text = DbExecutor_Insert(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litLINQInsert.Text = LINQ_Insert(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litORMInsert.Text = ORM_Insert(count).Timer.ElapsedMilliseconds.ToString() + " ms";

        litAdoUpdate.Text = Ado_Update(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litDbExecutorUpdate.Text = DbExecutor_Update(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litLINQUpdate.Text = LINQ_Update(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litORMUpdate.Text = ORM_Update(count).Timer.ElapsedMilliseconds.ToString() + " ms";

        litAdoSelect.Text = Ado_Select(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litDbExecutorSelect.Text = DbExecutor_Select(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litORMSelect.Text = ORM_Select(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litLINQSelectList.Text = LINQ_SelectList(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litORMSelectList.Text = ORM_SelectList(count).Timer.ElapsedMilliseconds.ToString() + " ms";

        litAdoDelete.Text = Ado_Delete(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litDbExecutorDelete.Text = DbExecutor_Delete(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litLINQDelete.Text = LINQ_Delete(count).Timer.ElapsedMilliseconds.ToString() + " ms";
        litORMDelete.Text = ORM_Delete(count).Timer.ElapsedMilliseconds.ToString() + " ms";
    }



    /// <summary>
    /// ADO插入
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult Ado_Insert(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                var conn = new SqlConnection(connString);
                var com = conn.CreateCommand();
                com.CommandText = "INSERT INTO [Speed] ([UserName],[PassWord],[GenderType],[LoginCount],[LoginIP],[RoleID]) VALUES ('xxxx','yyyy',0,1,'127.0.0.1',3);";
                conn.Open();
                com.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// DbExecutor插入
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult DbExecutor_Insert(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                using (var db = new DbExecutor(DataBaseType.SqlServer, connString, 60))
                {
                    db.ExecuteNonQuery(CommandType.Text, "INSERT INTO [Speed] ([UserName],[PassWord],[GenderType],[LoginCount],[LoginIP],[RoleID]) VALUES ('xxxx','yyyy',0,1,'127.0.0.1',3);");
                }
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// LINQ插入
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult LINQ_Insert(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                using (var db = new SpeedDataContext())
                {
                    db.Speed.InsertOnSubmit(new FS.Model.LINQ.Speed() { UserName = "xxxx", PassWord = "yyyy", GenderType = 0, LoginCount = 0, LoginIP = "127.0.0.1", RoleID = 3 });
                    db.SubmitChanges();
                }
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// ORM插入
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult ORM_Insert(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                new SpeedDB { UserName = "xxxx", PassWord = "yyyy", GenderType = 0, LoginCount = 0, LoginIP = "127.0.0.1", RoleID = 3 }.Insert();
            }
            return sp.Result;
        }
    }




    /// <summary>
    /// ADO修改
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult Ado_Update(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                var conn = new SqlConnection(connString);
                var com = conn.CreateCommand();
                com.CommandText = "update [Speed] set [UserName] = 'xxxx',[PassWord] = 'yyyy',[GenderType] = 0,[LoginCount] = 1,[LoginIP] = '127.0.0.1',[RoleID] = 3;";
                conn.Open();
                com.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// DbExecutor修改
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult DbExecutor_Update(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                using (var db = new DbExecutor(DataBaseType.SqlServer, connString, 60))
                {
                    db.ExecuteNonQuery(CommandType.Text, "update [Speed] set [UserName] = 'xxxx',[PassWord] = 'yyyy',[GenderType] = 0,[LoginCount] = 1,[LoginIP] = '127.0.0.1',[RoleID] = 3");
                }
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// LINQ修改
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult LINQ_Update(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                using (var db = new SpeedDataContext())
                {
                    foreach (var item in db.Speed.Where(o => o.ID > 1))
                    {
                        item.UserName = "xxxx"; item.PassWord = "yyyy"; item.GenderType = 0; item.LoginCount = 0; item.LoginIP = "127.0.0.1"; item.RoleID = 3;
                    }
                    db.SubmitChanges();
                }
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// ORM修改
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult ORM_Update(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                new SpeedDB { UserName = "xxxx", PassWord = "yyyy", GenderType = 0, LoginCount = 0, LoginIP = "127.0.0.1", RoleID = 3 }.Update();
            }
            return sp.Result;
        }
    }





    /// <summary>
    /// ADO查询
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult Ado_Select(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                var conn = new SqlConnection(connString);
                var com = conn.CreateCommand();
                com.CommandText = "Select * from [Speed] Where ID  > 1;";
                conn.Open();

                SqlDataAdapter ada = new SqlDataAdapter(connString, conn);
                ada.SelectCommand = com;

                var ds = new DataSet();
                ada.Fill(ds);

                conn.Close();
                conn.Dispose();
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// DbExecutor查询
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult DbExecutor_Select(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                using (var db = new DbExecutor(DataBaseType.SqlServer, connString, 60))
                {
                    db.GetDataSet(CommandType.Text, "Select * from [Speed] Where ID  > 1");
                }
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// ORM查询
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult ORM_Select(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                SpeedDB.Data.Where(o => o.ID > 1).ToTable();
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// LINQ查询
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult LINQ_SelectList(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                using (var db = new SpeedDataContext())
                {
                    db.Speed.Where(o => o.ID > 1).ToList();
                }
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// ORM查询
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult ORM_SelectList(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                SpeedDB.Data.Where(o => o.ID > 1).ToList();
            }
            return sp.Result;
        }
    }



    /// <summary>
    /// ADO删除
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult Ado_Delete(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                var conn = new SqlConnection(connString);
                var com = conn.CreateCommand();
                com.CommandText = "Delete from [Speed]  Where ID  > 1;";
                conn.Open();
                com.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// DbExecutor删除
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult DbExecutor_Delete(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                using (var db = new DbExecutor(DataBaseType.SqlServer, connString, 60))
                {
                    db.ExecuteNonQuery(CommandType.Text, "Delete from [Speed]  Where ID  > 1;");
                }
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// LINQ删除
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult LINQ_Delete(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                using (var db = new SpeedDataContext())
                {
                    var speed = db.Speed.Where(o => o.ID > 1);
                    db.Speed.DeleteAllOnSubmit(speed);
                    db.SubmitChanges();
                }
            }
            return sp.Result;
        }
    }

    /// <summary>
    /// ORM删除
    /// </summary>
    /// <returns></returns>
    SpeedTest.SpeedResult ORM_Delete(int count)
    {
        using (var sp = new SpeedTest().Begin())
        {
            // 循环10万次
            for (int i = 0; i < count; i++)
            {
                SpeedDB.Data.Where(o => o.ID > 1).Delete();
            }
            return sp.Result;
        }
    }
}