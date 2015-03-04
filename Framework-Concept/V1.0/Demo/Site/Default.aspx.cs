using System;
using FS.Core.Page;
using FS.Extend;
using FS.Model.Members;
using FS.Utils.Web;
using System.Linq;
using FS.Core.Model;

public partial class _Default : System.Web.UI.Page
{
    string reqOper;
    int reqID;

    protected void Page_Load(object sender, EventArgs e)
    {
        // QS 相当于 Request.QueryString 操作
        reqOper = Req.QS("oper");
        reqID = Req.QS("ID", 0);

        if (!IsPostBack)
        {
            // 绑定 性别 枚举
            hlGenderType.Bind(typeof(UserDB.eumGenderType));
            hl2GenderType.Bind(typeof(UserDB.eumGenderType));

            switch (reqOper)
            {
                case "update":
                    {
                        var user = UserDB.Data.Where(o => o.ID == reqID).ToInfo();
                        //if (user == null) { JavaScript.Alert("用户不存在", "/"); return; }
                        // 数据填充，将从数据库取得的数据（实体类）赋值到表单中。
                        // Fill的第二个参数是表单前缀，即：前缀 + 实体名称
                        // 当然，前缀，你也可以不填，或者使用其它定义的。
                        // 查看下Default.aspx页面中的修改框里的表单控件。会发觉都是采用 hl2 + 实体名称定义的
                        user.Fill(this, "hl2");

                        btnSave.Enabled = true;
                        break;
                    }
                case "del":
                    {
                        //UserDB.Data.Where(o => o.ID == reqID).Delete();
                        //JavaScript.Alert("删除成功", "/");
                        break;
                    }
            }
        }

        btnAdd.Click += btnAdd_Click;
        btnSave.Click += btnSave_Click;

        // 列表绑定
        //rptList.Bind(UserDB.Data.DbContext.Info.Where(o => o.ID > 10).Select(o => new { o.ID }).ToList());
        long total;
        //rptList.Bind(UserDB.Data.GetAllEntity(o => o.ID > 10, o => new { o.ID }, 1, 3, out total, new Bean<UserDB>.OrderModelField { propertyName = "ID", IsDESC = true }));
        rptList.Bind(UserDB.Data.Where(o => o.ID > 10).GetListDataSelect(o => new { o.ID, o.UserName }).ToList());
    }

    void btnSave_Click(object sender, EventArgs e)
    {
        // Form 即从表单提交过来的数据。赋值到实体类中。与Fill操作，正好相反
        //var info = UserDB.Form(null, "hl2");
        // Check 检测实体类赋值情况。根据实体类的特性申明，进行判断
        //if (!info.Check()) { return; }

        //info.Update(reqID);

        //JavaScript.Alert("修改成功！", "/");
    }

    void btnAdd_Click(object sender, EventArgs e)
    {
        // Form 即从表单提交过来的数据。赋值到实体类中。与Fill操作，正好相反
        //var info = UserDB.Form();
        // Check 检测实体类赋值情况。根据实体类的特性申明，进行判断
        //if (!info.Check()) { return; }

        //info.Insert();

        //JavaScript.Alert("注册成功！", "/");
    }
}