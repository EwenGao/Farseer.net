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

public partial class _Default : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // req的意思是Request的缩写，这样我们很容易判断出这个变量的意思
        // QS全称是：Request.QueryString 派生自：BasePage实现的。其方法实现在：FS.Utils.Web.Req.QS 中。
        // 除此外，还有QF：Request.Form、QA:先执行QS，如果没值，再执行QF操作。
        // 第二个 0 的参数意思是 默认值 ，0 代表是整型。当转换不成功。则默认为0 可以传入任意基本类型，如：bool、string、decimal等等
        var reqID = QS("ID", 0);
        var reqOper = QS("oper");

        if (!IsPostBack)
        {
            // 绑定 性别 枚举 到控件中
            // 通过对枚举的 [Display(Name = "男士")] 设置来显示中文 
            // 对于DropDownList、CheckBoxList、RadioButtonList、Repeater 的绑定操作是完全一样的哦
            // 当然不仅仅是绑定枚举、包括你的List<User> 都可以。对于实体的中文显示，默认是Caption字段
            hlGenderType.Bind(typeof(Users.eumGenderType));
            hl2GenderType.Bind(typeof(Users.eumGenderType));

            switch (reqOper)
            {
                case "update":
                    {
                        // ToInfo() 获取单条记录，返回实体类的实例对象
                        // ToList()、Delete、Update 都是在 Users.Data  下操作的哦
                        // 只要是继承自BaseModel，对数据库的操作都是在：对象名称.Data 下操作的
                        var user = Users.Data.Where(o => o.ID == reqID).ToInfo();
                        if (user == null) { JavaScript.Alert("用户不存在", "/"); return; }
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
                        // Delete() 删除指定Where条件的数据
                        // ToList()、Delete、Update 都是在 Users.Data  下操作的哦
                        // 只要是继承自BaseModel，对数据库的操作都是在：对象名称.Data 下操作的
                        Users.Data.Where(o => o.ID == reqID).Delete();
                        JavaScript.Alert("删除成功", "/");
                        break;
                    }
            }
        }

        btnAdd.Click += btnAdd_Click;
        btnSave.Click += btnSave_Click;

        // 数据分页、并绑定到Repeater中
        // 这里的分页是真实的数据库先进行分页获取数据，然后再在Repeater中进行实现 上一页、下一页的操作显示
        // rptList 是FS:Repeater控件 继承自原生的Repeater控件的基础上，实现上下页的菜单显示。 
        // ToList 传入rptList。实际是告诉ToList 我要从数据库获取第几页的数据，同时把数据库的Count总数，设置到Repeater中
        rptList.Bind(Users.Data.Desc(o => o.ID).ToList(rptList));
    }

    void btnSave_Click(object sender, EventArgs e)
    {
        // req的意思是Request的缩写，这样我们很容易判断出这个变量的意思
        // QS全称是：Request.QueryString 派生自：BasePage实现的。其方法实现在：FS.Utils.Web.Req.QS 中
        // 除此外，还有QF：Request.Form、QA:先执行QS，如果没值，再执行QF操作
        // 第二个 0 的参数意思是 默认值 ，0 代表是整型。当转换不成功。则默认为0 可以传入任意基本类型，如：bool、string、decimal等等
        var reqID = QS("ID", 0);

        // Users.Form 从表单Post接收的数据。
        // 我们通过"前缀名称" + 实体属性名称 来定义各个html input控件的Name。达到自动转换成实体
        // 前缀一般默认为："hl" 代表html的意思
        Users info = Users.Form(null, "hl2");

        // 检测实体类赋值情况。根据实体类的特性申明，进行判断
        // 比如判断用户输入的字符长度、必填、数字类型、手机、邮箱类型（支持正则验证）
        if (!info.Check()) { return; }

        // 这里是更新到数据库的意思
        // reqID 是通过标识字段的ID来判断。里面有多个重载版本。
        // 框架通过判断属性是否为null 来决定是保存要保存到数据库。
        info.Update(reqID);

        // 弹出alert() js 框
        JavaScript.Alert("修改成功！", "/");
    }

    void btnAdd_Click(object sender, EventArgs e)
    {
        // Users.Form 从表单Post接收的数据。
        // 我们通过"前缀名称" + 实体属性名称 来定义各个html input控件的Name。达到自动转换成实体
        // 前缀一般默认为："hl" 代表html的意思
        Users info = Users.Form();

        // 检测实体类赋值情况。根据实体类的特性申明，进行判断
        // 比如判断用户输入的字符长度、必填、数字类型、手机、邮箱类型（支持正则验证）
        if (!info.Check()) { return; }

        // 将值插入到数据库
        // 因为ID是自增，所以不需要赋值，同时框架通过判断属性是否为null 来决定是保存要保存到数据库。
        info.Insert();

        // 弹出alert() js 框
        JavaScript.Alert("注册成功！", "/");
    }
}