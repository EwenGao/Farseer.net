using System;
using System.IO;
using System.Xml.Serialization;

namespace FS.Configs
{
    /// <summary>
    ///     配置管理工具
    /// </summary>
    public class BaseConfigs<T> where T : class, new()
    {
        /// <summary>
        ///     锁对象
        /// </summary>
        private static object m_LockHelper = new object();

        /// <summary>
        ///     配置文件路径
        /// </summary>
        private static string filePath;

        /// <summary>
        ///     配置文件名称
        /// </summary>
        private static string fileName;

        /// <summary>
        ///     配置变量
        /// </summary>
        protected static T m_ConfigInfo;

        /// <summary>
        ///     Config修改时间
        /// </summary>
        protected static DateTime LoadAt;

        /// <summary>
        ///     获取配置文件所在路径，支持自定义路径
        /// </summary>
        public static string FilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    filePath = (AppDomain.CurrentDomain.BaseDirectory + "/App_Data/").Replace("/", "\\");
                }
                return filePath;
            }
            set
            {
                filePath = value.Replace("/", "\\");
                if (filePath.EndsWith("/"))
                {
                    filePath += "/";
                }
            }
        }

        /// <summary>
        ///     获取配置文件所在路径
        /// </summary>
        protected static string FileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = string.Format("{0}.config",
                                             typeof(T).Name.EndsWith("Config", true, null)
                                                 ? typeof(T).Name.Substring(0, typeof(T).Name.Length - 6)
                                                 : typeof(T).Name);
                }
                return fileName;
            }
        }

        /// <summary>
        ///     配置变量
        /// </summary>
        public static T ConfigInfo
        {
            get
            {
                if (m_ConfigInfo == null || LoadAt != File.GetLastWriteTime(FilePath + FileName))
                {
                    LoadConfig();
                }
                return m_ConfigInfo;
            }
        }

        /// <summary>
        ///     加载(反序列化)指定对象类型的配置对象
        /// </summary>
        protected static void LoadConfig()
        {
            //不存在则自动接创建
            if (!File.Exists(FilePath + FileName))
            {
                SaveConfig(new T());
            }

            LoadAt = File.GetLastWriteTime(FilePath + FileName);

            lock (m_LockHelper)
            {
                m_ConfigInfo = Deserialize(FilePath + FileName);
            }
        }

        /// <summary>
        ///     保存(序列化)指定路径下的配置文件
        /// </summary>
        /// <param name="t">Config配置</param>
        public static bool SaveConfig(T t = null)
        {
            if (t == null) { t = ConfigInfo; }
            var result = Serialize(t, FilePath + FileName);
            return result;
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static T Deserialize(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return default(T);
            }

            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fs);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }


        /// <summary>
        ///     序列化
        /// </summary>
        /// <param name="t">对象</param>
        /// <param name="filePath">文件路径</param>
        public static bool Serialize(T t, string filePath)
        {
            var succeed = false;
            FileStream fs = null;
            try
            {
                Directory.CreateDirectory(filePath.Substring(0, filePath.LastIndexOf("\\")));
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                var serializer = new XmlSerializer(t.GetType());
                serializer.Serialize(fs, t);
                succeed = true;
            }
            catch
            {
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return succeed;
        }
    }
}