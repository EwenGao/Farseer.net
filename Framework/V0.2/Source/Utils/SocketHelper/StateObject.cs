using System.Net.Sockets;
using System.Text;
using FS.Extend;

namespace FS.Utils.SocketHelper
{
    public class StateObject
    {
        /// <summary>
        /// 客户端连接的套接字
        /// </summary>
        public Socket WorkSocket { get; set; }
        /// <summary>
        /// 保存接收的数据
        /// </summary>
        public byte[] Buffer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Msg { get { return Encoding.ASCII.GetString(Buffer).ClearString("\\0"); } }
        /// <summary>
        /// 接收的最大值
        /// </summary>
        public int BufferSize { get; set; }
        /// <summary>
        /// 字节数
        /// </summary>
        public int ByteCount { get; set; }

        /// <summary>
        /// 接收的最大值
        /// </summary>
        /// <param name="bufferSize">接收的最大值</param>
        public StateObject(int bufferSize, Socket socket= null)
        {
            BufferSize = bufferSize;
            Buffer = new byte[bufferSize];
            WorkSocket = socket;
        }

        /// <summary>
        /// 默认为1024
        /// </summary>
        public StateObject() : this(1024) { }
    }
}
