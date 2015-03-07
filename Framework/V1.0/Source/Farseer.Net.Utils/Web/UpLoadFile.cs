using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using FS.Extend;
using FS.Utils.Common;

namespace FS.Utils.Web
{
    /// <summary>
    ///     上传文件
    /// </summary>
    public class UpLoadFile
    {
        /// <summary>
        ///     保存方式
        /// </summary>
        public enum SaveType
        {
            /// <summary>
            ///     按日期时间
            /// </summary>
            DateTime,

            /// <summary>
            ///     按原文件名
            /// </summary>
            FileName
        }

        /// <summary>
        ///     上传文件
        /// </summary>
        /// <param name="files">获取要上传的文件</param>
        /// <param name="filePath">要保存的文件路径,</param>
        /// <param name="lstFileType">允许上传的文件类型,大小,单位为KB,Size=0表示无任何限制</param>
        /// <param name="saveType">保存方式：1:按时间取名</param>
        public List<stuUpLoadFile> Upload(HttpFileCollection files, string filePath,
                                          SaveType saveType = SaveType.DateTime,
                                          List<stuUpLoadFileType> lstFileType = null)
        {
            var lstUpLoadFile = new List<stuUpLoadFile>();
            foreach (HttpPostedFile file in files)
            {
                lstUpLoadFile.Add(Upload(file, filePath, saveType, lstFileType));
            }

            return lstUpLoadFile;
        }

        /// <summary>
        ///     上传文件
        /// </summary>
        /// <param name="file">FileUpload</param>
        /// <param name="filePath">要保存的文件路径,</param>
        /// <param name="lstFileType">允许上传的文件类型,大小,单位为KB,Size=0表示无任何限制</param>
        /// <param name="saveType">保存方式：1:按时间取名</param>
        public stuUpLoadFile Upload(HttpPostedFile file, string filePath, SaveType saveType = SaveType.DateTime,List<stuUpLoadFileType> lstFileType = null)
        {
            string fileName;

            //获取保存为的文件名
            switch (saveType)
            {
                case SaveType.FileName:
                    {
                        fileName = Files.ConvertPath(file.FileName);
                        fileName = fileName.Substring(fileName.IndexOf('/') + 1);
                        break;
                    }
                default:
                    {
                        fileName = string.Format("{0}{1}{2}", DateTime.Now.ToString("yyyyMMddHHmmss"),
                                                 Rand.CreateRandomString(4), Path.GetExtension(file.FileName));
                        break;
                    }
            }
            return Upload(file, filePath, fileName, lstFileType);
        }

        /// <summary>
        ///     上传文件
        /// </summary>
        /// <param name="file">FileUpload</param>
        /// <param name="filePath">要保存的文件路径,</param>
        /// <param name="lstFileType">允许上传的文件类型,大小,单位为KB,Size=0表示无任何限制</param>
        /// <param name="fileName">自定义文件名称</param>
        public stuUpLoadFile Upload(HttpPostedFile file, string filePath, string fileName,
                                    List<stuUpLoadFileType> lstFileType = null)
        {
            var uploadFile = new stuUpLoadFile {ErrorMessage = string.Empty};
            filePath = Url.ConvertPath(filePath);
            if (!filePath.EndsWith("/"))
            {
                filePath += "/";
            }

            //判断根目录,保存目录是否为空
            if (filePath.IsNullOrEmpty())
            {
                uploadFile.ErrorMessage = "保存路径没有填写,请联系管理员!";
                return uploadFile;
            }

            // 检测上传文件的合法性
            if (!CheckFile(file, ref uploadFile, lstFileType))
            {
                return uploadFile;
            }

            #region 上传文件操作

            try
            {
                #region 创建文件目录(任何级别目录)

                var fileMapPath = Files.GetMapPath(filePath); //获取目录绝对路径
                Directory.CreateDirectory(fileMapPath);

                #endregion

                file.SaveAs(fileMapPath + fileName);

                uploadFile.FileName = fileName;
                uploadFile.FileNamePrefix = Path.GetFileNameWithoutExtension(fileName);
                uploadFile.FileNameExtension = Path.GetExtension(fileName);
                uploadFile.Size = file.ContentLength >= 1024 ? file.ContentLength/1024 : 1;
                uploadFile.FilePath = filePath;
                uploadFile.FileMapPath = fileMapPath;
                uploadFile.IsSuccess = true;
            }

            catch (Exception Ex)
            {
                uploadFile.IsSuccess = false;
                uploadFile.ErrorMessage = Ex.Message;
                return uploadFile;
            }

            #endregion

            return uploadFile;
        }

        /// <summary>
        ///     检查每个文件的合法检验
        /// </summary>
        private bool CheckFile(HttpPostedFile file, ref stuUpLoadFile uploadFile,
                               List<stuUpLoadFileType> lstFileType = null)
        {
            //获取文件名前缀
            var fileNamePrefix = Path.GetFileNameWithoutExtension(file.FileName);
            //获取文件名后缀
            var fileNameExtension = file.FileName.LastIndexOf('.') > -1
                                           ? Path.GetExtension(file.FileName).Substring(1)
                                           : Path.GetExtension(file.FileName);

            //文件容量
            var fileContentLength = file.ContentLength/1024;

            if (file.FileName.IsNullOrEmpty())
            {
                uploadFile.ErrorMessage = "未检测到文件上传!";
                return false;
            }
            if (fileNamePrefix.IsNullOrEmpty())
            {
                uploadFile.ErrorMessage = "文件: [ " + file.FileName + " ] 前缀名不能为空";
                return false;
            }
            if (file.ContentLength > 0 && fileContentLength == 0)
            {
                fileContentLength = 1;
            }
            if (fileContentLength <= 0)
            {
                uploadFile.ErrorMessage = "文件: [ " + file.FileName + " ] 的大小不能为0KB";
                return false;
            }

            //循环文件类型,长度
            if (lstFileType == null || lstFileType.Count == 0)
            {
                return true;
            }

            //true表示属于允许上传的类型
            var isfileExtension = false;
            foreach (var fileType in lstFileType)
            {
                //判断文件类型,长度是否允许上传
                if (!fileNameExtension.IsEquals(fileType.type))
                {
                    continue;
                }
                isfileExtension = true;

                //判断文件容量是否超过上限
                if (fileType.size != 0 && fileContentLength > fileType.size)
                {
                    uploadFile.ErrorMessage = string.Format("文件类型为 [ {0} ] 的容量不能超过: [ {1} KB ]", fileType.type,
                                                            fileType.size);
                    return false;
                }
                break;
            }

            if (!isfileExtension)
            {
                uploadFile.ErrorMessage = string.Format("文件扩展名: [ {0} ] 不允许上传", fileNameExtension);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     上传文件后回发的资料
        /// </summary>
        public struct stuUpLoadFile
        {
            /// <summary>
            ///     上传成功的文件名
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            ///     上传成功的文件名-前缀
            /// </summary>
            public string FileNamePrefix { get; set; }

            /// <summary>
            ///     上传成功的文件名-扩展名
            /// </summary>
            public string FileNameExtension { get; set; }

            /// <summary>
            ///     上传成功的文件容量(单位：KB)
            /// </summary>
            public int Size { get; set; }

            /// <summary>
            ///     上传成功的文件相对路径(WebPath之后的路径,不带文件名)
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            ///     上传成功的文件物理路径(不带文件名)
            /// </summary>
            public string FileMapPath { get; set; }

            /// <summary>
            ///     错误消息
            /// </summary>
            public string ErrorMessage { get; set; }

            /// <summary>
            ///     上传成功
            /// </summary>
            public bool IsSuccess { get; set; }
        }

        /// <summary>
        ///     上传文件的权限
        /// </summary>
        public struct stuUpLoadFileType
        {
            /// <summary>
            ///     允许上传的容量(单位：KB. 0：无限制)
            /// </summary>
            public int size;

            /// <summary>
            ///     允许上传的类型
            /// </summary>
            public string type;
        }
    }
}