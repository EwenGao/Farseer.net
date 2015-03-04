<%@ WebHandler Language="C#" Class="Upload" %>

/**
 * KindEditor ASP.NET
 *
 * 本ASP.NET程序是演示程序，建议不要直接在实际项目中使用。
 * 如果您确定直接使用本程序，使用之前请仔细确认相关安全设置。
 *
 */

using System;
using System.Collections;
using System.Web;
using System.IO;
using System.Globalization;
using LitJson;
using Qyn.Studio.Extend;
using Qyn.Studio.Configs;
using Qyn.Studio.Utils;

public class Upload : IHttpHandler
{
    //文件保存目录路径
    private String savePath = ParseFile.GetRootPath() + GeneralConfigs.ConfigInfo.UploadDirectory;
    //文件保存目录URL
    private String saveUrl = GeneralConfigs.ConfigInfo.WebDirectory + GeneralConfigs.ConfigInfo.UploadDirectory;
    //定义允许上传的文件扩展名
    private String fileTypes = "gif,jpg,jpeg,png,bmp,rar,ico,doc,xls,txt";
    //最大文件大小
    //private int maxSize = 1024 * 1024 * 1024 * 10;

    private HttpContext context;

    public void ProcessRequest(HttpContext context)
    {
        this.context = context;
        
        HttpPostedFile imgFile = context.Request.Files["imgFile"];
        if (imgFile == null)
        {
            showError("请选择文件。");
        }

        

        String fileName = imgFile.FileName.Replace("?",".");
        String fileExt = Path.GetExtension(fileName).ToLower();
        ArrayList fileTypeList = ArrayList.Adapter(fileTypes.Split(','));

        if (imgFile.InputStream == null)// || imgFile.InputStream.Length > maxSize
        {
            showError("上传文件大小超过限制。"); return;
        }

        if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(fileTypes.Split(','), fileExt.Substring(1).ToLower()) == -1)
        {
            showError("上传文件扩展名是不允许的扩展名。" + fileName); return;
        }

        String DateDir = ParseDateTime.GetDateTimePath();
        savePath += DateDir;
        saveUrl += DateDir;

        ParseFile.CreateDirs(savePath.DelLastOf("/"));
        
        String filePath = savePath + fileExt;

        imgFile.SaveAs(filePath);

        String fileUrl = saveUrl + fileExt;

        Hashtable hash = new Hashtable();
        hash["error"] = 0;
        hash["url"] = fileUrl;
        context.Response.AddHeader("Content-Type", "text/html; charset=GB2312");
        context.Response.Write(JsonMapper.ToJson(hash));
        context.Response.End();
    }

    private void showError(string message)
    {
        Hashtable hash = new Hashtable();
        hash["error"] = 1;
        hash["message"] = message;
        context.Response.AddHeader("Content-Type", "text/html; charset=GB2312");
        context.Response.Write(JsonMapper.ToJson(hash));
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}
