<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UrlRewriter.aspx.cs" Inherits="UrlRewriter" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<body>
    我的ID 是：<%=FS.Utils.Web.Req.QS("ID")%> <br />
    我的ID2是：<%=FS.Utils.Web.Req.QS("ID2")%>
</body>
</html>
