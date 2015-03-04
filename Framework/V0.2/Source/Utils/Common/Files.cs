using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;
using FS.Configs;
using FS.Extend;
using FS.Utils.Web;

namespace FS.Utils.Common
{
    /// <summary>
    ///     文件工具
    /// </summary>
    public abstract class Files
    {
        /// <summary>
        ///     获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        public static string GetMapPath(string strPath)
        {
            strPath = ConvertPath(strPath);
            try
            {
                return ConvertPath(HttpContext.Current != null ? HttpContext.Current.Server.MapPath(strPath) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath));
            }
            catch
            {
                return ConvertPath(strPath);
            }
        }

        /// <summary>
        ///     以指定的ContentType输出指定文件文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">输出的文件名</param>
        /// <param name="fileType">将文件输出时设置的ContentType</param>
        public static void ResponseFile(string filePath, string fileName, string fileType)
        {
            Stream iStream = null;
            // 缓冲区为10k
            var buffer = new Byte[10000];
            // 文件长度
            // 需要读的数据长度

            try
            {
                // 打开文件
                iStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // 需要读的数据长度
                var dataToRead = iStream.Length;

                HttpContext.Current.Response.ContentType = fileType;
                HttpContext.Current.Response.AddHeader("Content-Disposition",
                                                       "attachment;filename=" +
                                                       Url.UrlEncode(fileName.Trim()).Replace("+", " "));

                while (dataToRead > 0)
                {
                    // 检查客户端是否还处于连接状态
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        var length = iStream.Read(buffer, 0, 10000);
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        // 如果不再连接则跳出死循环
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    // 关闭文件
                    iStream.Close();
                }
            }
            HttpContext.Current.Response.End();
        }

        /// <summary>
        ///     判断文件流是否为UTF8字符集
        /// </summary>
        /// <param name="sbInputStream">文件流</param>
        private static bool IsUTF8(FileStream sbInputStream)
        {
            int i;
            var bAllAscii = true;
            var iLen = sbInputStream.Length;

            byte cOctets = 0;
            for (i = 0; i < iLen; i++)
            {
                var chr = (byte) sbInputStream.ReadByte();

                if ((chr & 0x80) != 0) bAllAscii = false;

                if (cOctets == 0)
                {
                    if (chr >= 0x80)
                    {
                        do
                        {
                            chr <<= 1;
                            cOctets++;
                        } while ((chr & 0x80) != 0);

                        cOctets--;
                        if (cOctets == 0) return false;
                    }
                }
                else
                {
                    if ((chr & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    cOctets--;
                }
            }

            return cOctets <= 0 && !bAllAscii;
        }

        /// <summary>
        ///     建立文件夹
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static void CreateDir(string path)
        {
            path = ConvertPath(path);
            if (path.IsNullOrEmpty() || path.Trim() == "\\")
            {
                return;
            }

            if (path.IsStartsWith(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        ///// <summary>
        /////     智能创建文件目录(任何级别目录)
        ///// </summary>
        ///// <param name="dirPath">路径</param>
        //public static bool CreateDirs(string dirPath)
        //{
        //    dirPath = ConvertPath(dirPath);

        //    if (Directory.Exists(dirPath))
        //    {
        //        return true;
        //    }

        //    var lstPath = dirPath.ToList(string.Empty, "\\");
        //    if (lstPath.GetLast().IndexOf('.') > -1 || lstPath.GetLast().IsNullOrEmpty())
        //    {
        //        lstPath.RemoveAt(lstPath.Count - 1);
        //    }

        //    var path = new StringBuilder();
        //    foreach (var str in lstPath)
        //    {
        //        path.Append(str + "\\");
        //        if (!Directory.Exists(path.ToString()))
        //        {
        //            CreateDir(path.ToString());
        //        }
        //    }
        //    return true;
        //}

        /// <summary>
        ///     删除目录,同时删除子目录所有文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static List<string> DeleteDir(string path)
        {
            var lst = new List<string>() { path };
            if (!Directory.Exists(path)) { return lst; }

            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
                lst.Add(file);
            }
            foreach (var dir in dirs) { lst.AddRange(DeleteDir(dir)); }
            Directory.Delete(path, false);
            return lst;
        }

        /// <summary>
        ///     复制文件夹内的文件到指定路径
        /// </summary>
        /// <param name="srcPath">源文件夹</param>
        /// <param name="aimPath">目录文件夹</param>
        /// <param name="isCopySubDir">true:复制子文件夹;false:只复制根文件夹下的文件</param>
        /// <param name="overCopy">是复制覆盖</param>
        /// <param name="filterExtension">后缀名过滤，格式："svn|aspx|asp|exe"</param>
        public static bool CopyDir(string srcPath, string aimPath, bool isCopySubDir = true, bool overCopy = true,string filterExtension = "")
        {
            try
            {
                aimPath = ConvertPath(aimPath);
                if (!aimPath.EndsWith("\\"))
                {
                    aimPath += "\\";
                }

                if (!Directory.Exists(aimPath))
                {
                    Directory.CreateDirectory(aimPath);
                }

                var lstFilter = filterExtension.ToList(string.Empty, "|");

                var fileList = Directory.GetFileSystemEntries(srcPath);

                // 遍历所有的文件和目录
                foreach (var file in fileList)
                {
                    if (lstFilter.Exists(o => o == Path.GetExtension(file)))
                    {
                        continue;
                    }

                    if (Directory.Exists(file) && !isCopySubDir)
                    {
                        continue;
                    }

                    if (Directory.Exists(file) && isCopySubDir)
                    {
                        CopyDir(file, aimPath + Path.GetFileName(file), isCopySubDir, overCopy, filterExtension);
                    }

                    else
                    {
                        File.Copy(file, aimPath + Path.GetFileName(file), overCopy);
                    }
                }
                return true;
            }

            catch (Exception ex)
            {
                var message = "Can Not Operate DataBase!";
                if (GeneralConfigs.ConfigInfo.DeBug)
                {
                    message = ex.Message;
                }

                if (HttpContext.Current != null)
                {
                    new Terminator().Throw(message);
                }
                else
                {
                    MessageBox.Show(message);
                }
                return false;
            }
        }

        /// <summary>
        ///     生成文件
        /// </summary>
        /// <param name="savePagePath">要保存的文件路径</param>
        /// <param name="writeCode">生成文件所使用的编码</param>
        /// <param name="strContent">要生成的内容</param>
        public static bool WriteFile(string savePagePath, string writeCode, string strContent)
        {
            try
            {
                var mapSavePagePathName = GetMapPath(savePagePath); //要保存的物路径
                var enWriteCode = Encoding.GetEncoding(writeCode); //生成文件所使用的编码方式
                var streamWriter = new StreamWriter(mapSavePagePathName, false, enWriteCode);
                streamWriter.Write(strContent);
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter.Dispose();
                return true;
            }
            catch (Exception oExcept)
            {
                var context = HttpContext.Current;
                if (context != null)
                {
                    new Terminator().Throw(oExcept.Message);
                }
                else
                {
                    MessageBox.Show(oExcept.Message, "发生错误");
                }
                return false;
            }
        }

        /// <summary>
        ///     将/转换成：\\
        /// </summary>
        /// <returns></returns>
        public static string ConvertPath(string path)
        {
            return path.IsNullOrEmpty() ? string.Empty : path.Replace("/", "\\");
        }

        /// <summary>
        ///     获取根目录路径
        /// </summary>
        /// <returns></returns>
        public static string GetRootPath()
        {
            return ConvertPath(AppDomain.CurrentDomain.BaseDirectory) + "\\";
            //if (HttpContext.Current != null) { return HttpContext.Current.Request.PhysicalApplicationPath; }
            //else { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        /// <summary>
        ///     获取App_Data路径
        /// </summary>
        /// <returns></returns>
        public static string GetAppDataPath()
        {
            return GetRootPath() + "App_Data/";
        }

        /// <summary>
        ///     获取App_Data路径
        /// </summary>
        /// <returns></returns>
        public static void CreateAppData()
        {
            var path = GetAppDataPath();
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
        }

        /// <summary>
        ///     获取文件夹容量
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns></returns>
        public static long GetDirLength(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(dirPath))
            {
                return 0;
            }

            //定义一个DirectoryInfo对象
            var di = new DirectoryInfo(dirPath);

            //通过GetFiles方法,获取di目录中的所有文件的大小
            var len = di.GetFiles().Sum(fi => fi.Length);

            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            var dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                len += dis.Sum(t => GetDirLength(t.FullName));
            }
            return len;
        }

        /// <summary>
        ///     传入网页相对路径返回网页的html代码,出错返回null
        /// </summary>
        /// <param name="loadPagePath">源文件网页路径(不用带根目录路径)</param>
        /// <param name="readCode">读取源文件所使用的编码</param>
        public static string GetFile(string loadPagePath, Encoding encoding)
        {
            var pageContent = ""; //源文件网页内容

            if (!File.Exists(loadPagePath)) { return null; }
            var mapLoadPagePath = Files.GetMapPath(loadPagePath);
            var streamReader = new StreamReader(mapLoadPagePath, encoding);
            pageContent = streamReader.ReadToEnd(); // 读取文件
            streamReader.Close();

            return pageContent;
        }

        /// <summary>
        /// 根据文件路径得到文件的MD5值
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5(string filePath)
        {
            try
            {
                var get_file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var md5 = new MD5CryptoServiceProvider();
                var hash_byte = md5.ComputeHash(get_file);
                var resule = System.BitConverter.ToString(hash_byte);
                resule = resule.Replace("-", "");
                md5.Clear(); md5.Dispose();
                get_file.Close(); get_file.Dispose();
                return resule;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        /// 根据文件路径得到文件的MD5值
        /// </summary>
        /// <param name="FilePath">文件的路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5(FileStream fs)
        {
            try
            {
                var md5 = new MD5CryptoServiceProvider();
                var hash_byte = md5.ComputeHash(fs);
                var resule = System.BitConverter.ToString(hash_byte);
                resule = resule.Replace("-", "");
                return resule;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        /// 对文件重命名（不改变路径）
        /// </summary>
        /// <param name="dir">源文件</param>
        /// <param name="newFileName">名称</param>
        /// <param name="isChangeExtension">是否改变扩展名，为True时，根据newFileName的值进行改变。没有，则变更为无扩展的文件。为False时，则忽略newFileName的扩展名</param> 
        public static bool Rename(string dir, string newFileName, bool isChangeExtension = false)
        {
            //  源文件名
            var fileName = Path.GetFileNameWithoutExtension(newFileName);
            //  扩展名
            var extendName = isChangeExtension ? Path.GetExtension(newFileName) : Path.GetExtension(dir);
            //  组成新的文件名
            var newFile = Path.GetDirectoryName(dir) + "\\" + newFileName + extendName;

            // 新的文件名存在，则执行失败。
            if (File.Exists(newFile)) { return false; }

            // 移动
            File.Move(dir, newFile);

            return true;
        }
    }
}