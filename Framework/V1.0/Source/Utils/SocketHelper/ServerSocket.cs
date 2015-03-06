using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace FS.Utils.SocketHelper
{
    public class ServerSocket
    {
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// 端口
        /// </summary>
        private int Point { get; set; }
        /// <summary>
        /// 监听排队数
        /// </summary>
        private int Listen { get; set; }
        /// <summary>
        /// 已连接的客户端列表
        /// </summary>
        public Dictionary<IPEndPoint, Socket> LstClient { get; set; }
        /// <summary>
        /// 当前运行总的连接人数
        /// </summary>
        public int AllNum { get; set; }
        /// <summary>
        /// 当前连接人数
        /// </summary>
        public int CurrentNum { get { return LstClient.Count; } }

        public int BufferSize { get; set; }

        /// <summary>
        /// 接收客户端请求连接时执行
        /// </summary>
        public Action<StateObject> ActAccept { get; set; }
        /// <summary>
        /// 接收信息时执行
        /// </summary>
        public Action<StateObject> ActReceive { get; set; }

        public ServerSocket(int point, int listen, int bufferSize = 1024)
        {
            Point = point;
            Listen = listen;
            BufferSize = bufferSize;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            LstClient = new Dictionary<IPEndPoint, Socket>();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, Point));
            socket.Listen(Listen);
            socket.BeginAccept(new AsyncCallback(AcceptEnd), new StateObject(BufferSize));
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            if (socket != null)
            {
                if (socket.Connected) { socket.Shutdown(SocketShutdown.Both); }
                socket.Close(); socket.Dispose(); socket = null;
            }
            foreach (var item in LstClient) { item.Value.Close(); item.Value.Dispose(); }
            LstClient.Clear();
        }

        /// <summary>
        /// 接收客户机连接的方法
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptEnd(IAsyncResult ar)
        {
            try
            {
                if (socket == null) { return; }

                var state = ar.AsyncState as StateObject;
                // 获取客户端的Socket
                state.WorkSocket = socket.EndAccept(ar);

                // 添加到客户端列表
                var ep = state.WorkSocket.RemoteEndPoint as IPEndPoint;
                LstClient[ep] = state.WorkSocket;

                AllNum++;

                // 执行传入的委托
                if (ActAccept != null) { ActAccept(state); }

                // 客户端接受消息
                state.WorkSocket.BeginReceive(state.Buffer, 0, state.BufferSize, 0, new AsyncCallback(ReceiveEnd), ar.AsyncState);


                // 继续接收下一个客户机连接
                socket.BeginAccept(new AsyncCallback(AcceptEnd), new StateObject(BufferSize));
            }
            catch { }
        }

        /// <summary>
        /// 接收线程
        /// </summary>
        /// <param name="o"></param>
        private void ReceiveEnd(IAsyncResult ar)
        {
            var state = ar.AsyncState as StateObject;
            try { state.ByteCount = state.WorkSocket.EndReceive(ar); }
            catch { state.ByteCount = 0; state.Buffer = new byte[0]; }

            if (state.WorkSocket.Connected && state.ByteCount > 0)
            {
                state.WorkSocket.BeginReceive(state.Buffer, 0, state.BufferSize, 0, new AsyncCallback(ReceiveEnd), state);
            }
            else // 客户端关闭
            {
                // 添加到客户端列表
                var ep = state.WorkSocket.RemoteEndPoint as IPEndPoint;
                LstClient.Remove(ep);
            }

            // 执行传入的委托
            if (ActReceive != null) { ActReceive(state); }
        }
    }
}
