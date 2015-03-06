<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

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
            .auto-style2 { width: 151px; }
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
            blue { color: #0094ff; }
            orange { color: #e63b15; }
            green { color: #136d06; }
            pre { background-color: #eee; }
            table, th, td { border: 1px solid grey; border-collapse: collapse; padding: 5px; }
            table tr:nth-child(odd) { background-color: #f1f1f1; }
            table tr:nth-child(even) { background-color: #fff; }
        </style>
    </head>
    <body>
        <div class="head">
            <h1>Farseer.net 演示</h1>
        </div>
        <form id="form1" runat="server">
            <fieldset>
                <legend>数据列表（分页）演示</legend>
                <table class="auto-style1" style="width: 800px">
                    <tr>
                        <td>系统编号</td>
                        <td>名称</td>
                        <td>密码</td>
                        <td>性别</td>
                        <td>登陆次数</td>
                        <td>登陆IP</td>
                        <td>操作</td>
                    </tr>
                    <FS:Repeater ID="rptList" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# ((Users)Container.DataItem).ID%></td>
                                <td><%# ((Users)Container.DataItem).UserName%></td>
                                <td><%# ((Users)Container.DataItem).PassWord%></td>
                                <td><%# ((Users)Container.DataItem).GenderType.GetName()%></td>
                                <td><%# ((Users)Container.DataItem).LoginCount%></td>
                                <td><%# ((Users)Container.DataItem).LoginIP%></td>
                                <td><a href="?oper=update&ID=<%# ((Users)Container.DataItem).ID%>">修改</a>
                                    <a href="?oper=del&ID=<%# ((Users)Container.DataItem).ID%>">删除</a>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <PaginationHtml><tr class="tdbg" align="center" style="height: 28px;"><td colspan="12"><Pagination /></td></tr></PaginationHtml>
                    </FS:Repeater>
                </table>
            </fieldset>

            <fieldset>
                <legend>数据添加演示</legend>
                <table class="auto-style1">
                    <tr>
                        <td class="auto-style2">用户名</td>
                        <td><asp:TextBox ID="hlUserName" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="auto-style2">密码</td>
                        <td><asp:TextBox ID="hlPassWord" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="auto-style2">性别</td>
                        <td><asp:RadioButtonList ID="hlGenderType" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center"><asp:Button ID="btnAdd" runat="server" Text="添加" /></td>
                    </tr>
                </table>
            </fieldset>

            <fieldset>
                <legend>数据修改演示</legend>
                <table class="auto-style1">
                    <tr>
                        <td class="auto-style2">用户名</td>
                        <td><asp:TextBox ID="hl2UserName" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="auto-style2">密码</td>
                        <td><asp:TextBox ID="hl2PassWord" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="auto-style2">性别</td>
                        <td><asp:RadioButtonList ID="hl2GenderType" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList></td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center"><asp:Button ID="btnSave" runat="server" Text="修改" Enabled="false" /></td>
                    </tr>
                </table>
            </fieldset>
        </form>
    </body>
</html>
