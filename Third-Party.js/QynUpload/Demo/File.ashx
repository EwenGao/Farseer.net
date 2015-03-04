<%@ WebHandler Language="C#" Class="upload" %>

using System;
using System.Web;

public class upload : IHttpHandler
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void ProcessRequest(HttpContext context)
    {
        // .net程序上传代码
        if (context.Request.Files["Filedata"] == null) { return; }
        context.Response.Write("这里填写上传成功后的地址");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}