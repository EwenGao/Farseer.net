<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Speed.aspx.cs" Inherits="Speed" %>

<%@ Register Assembly="Farseer.Net" Namespace="FS.UI" TagPrefix="FS" %>
<%@ Import Namespace="FS.Model.Members" %>
<%@ Import Namespace="FS.Extend" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
        .auto-style1 { width: 400px; }
        body { font-size: 12px; background-color: #eee; padding: 0; }
        li { height: 30px; }
        dt { font-weight: bold; color: #0094ff; font-size: 13px; }
        h1 { color: #eceff2; text-shadow: 3px 3px 3px #9c9ea5; }
        legend { font-weight: bold; color: #ff4c24; font-size: 14px; }
        fieldset { border: dashed 1px #496d8a; margin-top: 30px; background-color: #fffeee; }
        .head { height: 90px; line-height: 90px; padding-left: 20px; background-color: #103858; margin: -20px -20px 0 -20px; }
        a:visited { color: #0082ff; }
        a:hover { color: #0082ff; }
        a { color: #0082ff; }
        blue { color: #0094ff; font-weight: bold; }
        orange { color: #e63b15; }
        green { color: #136d06; }
        pre { background-color: #eee; }
        table, th, td { border: 1px solid grey; border-collapse: collapse; padding: 5px; }
            table tr:nth-child(odd) { background-color: #f1f1f1; }
            table tr:nth-child(even) { background-color: #fff; }
    </style>
</head>
<body>
    <form runat="server">
        <div class="head">
            <h1>Farseer.net 性能测试演示</h1>
        </div>
        <fieldset>
            <legend>测试环境</legend>
            软件环境：Sql08 R2、 IIS7.0、4.0、Win7 64位 SP1<br />
            硬件环境： Core-i7 4710MQ @ 2.5GHz 四核、16G DDR3（1600MHz）、240G固态硬盘
        </fieldset>
        <fieldset>
            <legend>表结构</legend>
            <pre>
        CREATE TABLE [dbo].[Speed](
	        [<strong>ID</strong>] [<blue>int</blue>] IDENTITY(1,1) NOT NULL,
	        [<strong>UserName</strong>] [<blue>nvarchar</blue>](16) NOT NULL,
	        [<strong>PassWord</strong>] [<blue>varchar</blue>](50) NOT NULL,
	        [<strong>GenderType</strong>] [<blue>tinyint</blue>] NOT NULL,
	        [<strong>LoginCount</strong>] [<blue>int</blue>] NOT NULL,
	        [<strong>LoginIP</strong>] [<blue>char</blue>](15) NOT NULL,
	        [<strong>RoleID</strong>] [<blue>int</blue>] NOT NULL,
            CONSTRAINT [PK_Speed] PRIMARY KEY CLUSTERED ( [ID] ASC))
        </pre>
        </fieldset>
        <br />
        <asp:Button ID="btnRun" runat="server" Text="开始 " />
        <fieldset>
            <legend>数据插入</legend>
            ADO Insert:<asp:Literal ID="litAdoInsert" runat="server"></asp:Literal>
            <br />
            Farseer DbExecutor Insert:<asp:Literal ID="litDbExecutorInsert" runat="server"></asp:Literal>
            <br />
            LINQ 2 SQL Insert:<asp:Literal ID="litLINQInsert" runat="server"></asp:Literal>
            <br />
            Farseer ORM Insert:<asp:Literal ID="litORMInsert" runat="server"></asp:Literal>
        </fieldset>

        <fieldset>
            <legend>数据更新</legend>
            ADO Update:<asp:Literal ID="litAdoUpdate" runat="server"></asp:Literal>
            <br />
            Farseer DbExecutor Update:<asp:Literal ID="litDbExecutorUpdate" runat="server"></asp:Literal>
            <br />
            LINQ 2 SQL Update:<asp:Literal ID="litLINQUpdate" runat="server"></asp:Literal>
            <br />
            Farseer ORM Update:<asp:Literal ID="litORMUpdate" runat="server"></asp:Literal>
        </fieldset>

        <fieldset>
            <legend>数据查询</legend>
            ADO Select:<asp:Literal ID="litAdoSelect" runat="server"></asp:Literal>
            <br />
            Farseer DbExecutor Select:<asp:Literal ID="litDbExecutorSelect" runat="server"></asp:Literal>
            <br />
            Farseer ORM Select(DataSet):<asp:Literal ID="litORMSelect" runat="server"></asp:Literal>
            <br />
            LINQ 2 SQL Select(List实体):<asp:Literal ID="litLINQSelectList" runat="server"></asp:Literal>
            <br />
            Farseer ORM Select(List实体):<asp:Literal ID="litORMSelectList" runat="server"></asp:Literal>
        </fieldset>

        <fieldset>
            <legend>数据删除</legend>
            ADO Delete:<asp:Literal ID="litAdoDelete" runat="server"></asp:Literal>
            <br />
            Farseer DbExecutor Delete:<asp:Literal ID="litDbExecutorDelete" runat="server"></asp:Literal>
            <br />
            LINQ 2 SQL Delete:<asp:Literal ID="litLINQDelete" runat="server"></asp:Literal>
            <br />
            Farseer ORM Delete:<asp:Literal ID="litORMDelete" runat="server"></asp:Literal>
        </fieldset>
    </form>
</body>
</html>
