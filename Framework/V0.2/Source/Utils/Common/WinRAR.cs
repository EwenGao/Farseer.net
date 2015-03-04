using System.Diagnostics;
using System.IO;

namespace FS.Utils.Common
{
    /// <summary>
    ///     解压缩文件
    /// </summary>
    public abstract class WinRAR
    {
        /// <summary>
        ///     解压文件
        /// </summary>
        /// <param name="rarPath">解决压缩文件安装程序路径</param>
        /// <param name="filePath">压缩包文件路径</param>
        /// <param name="toPath">解压到路径</param>
        /// <returns></returns>
        public static bool OutRar(string filePath, string toPath, string rarPath)
        {
            //取得系统临时目录
            //string sysTempDir = Path.GetTempPath();

            //要解压的文件路径，请自行设置
            //string rarFilePath = @"d:\test.rar";

            //确定要解压到的目录，是系统临时文件夹下，与原压缩文件同名的目录里
            // string unrarDestPath = Path.Combine(sysTempDir,
            //     Path.GetFileNameWithoutExtension(rarFilePath));

            //组合出需要shell的完整格式
            //string shellArguments = string.Format("x -o+ \"{0}\" \"{1}\\\"",
            //    rarFilePath, unrarDestPath);

            filePath = Files.GetMapPath(filePath);
            toPath = Files.GetMapPath(toPath);
            Directory.CreateDirectory(toPath);

            var shellArguments = string.Format("x -o+ \"{0}\" \"{1}\\\"", filePath, toPath);

            //用Process调用
            using (var unrar = new Process())
            {
                unrar.StartInfo.FileName = rarPath;
                unrar.StartInfo.Arguments = shellArguments;
                //隐藏rar本身的窗口
                unrar.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                unrar.Start();
                //等待解压完成
                unrar.WaitForExit();
                unrar.Close();
            }
            return true;


            //统计解压后的目录和文件数
            //DirectoryInfo di = new DirectoryInfo(unrarDestPath);

            //MessageBox.Show(string.Format("解压完成，共解压出：{0}个目录，{1}个文件",
            //    di.GetDirectories().Length, di.GetFiles().Length));
        }
    }
}