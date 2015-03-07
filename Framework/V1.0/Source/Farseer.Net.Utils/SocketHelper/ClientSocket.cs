using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FS.Utils.SocketHelper
{
    public class ClientSocket : IDisposable
    {
        /// <summary>
        /// 套接字
        /// </summary>
        private Socket socket;
        /// <summary>
        /// 服务器端口
        /// </summary>
        private IPEndPoint Server { get; set; }

        public ClientSocket(string IP, int point, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            socket = new Socket(addressFamily, socketType, protocolType);
            Server = new IPEndPoint(IPAddress.Parse(IP), point);
        }

        /// <summary>
        /// 视情况决定是否需要绑定本地
        /// </summary>
        /// <param name="IP"></param>
        public void Bind(IPEndPoint IP, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            socket.Bind(IP);
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        public bool Start()
        {
            try
            {
                socket.Connect(Server);
                return true;
            }
            catch { return false; }
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
        }

        public void Send(string msg, int bufferSize = 1024, AsyncCallback asyncCallBack = null)
        {
            var byteData = Encoding.ASCII.GetBytes(msg);
            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), new StateObject(bufferSize));
        }
        public void Send(byte[] buffer, int bufferSize = 1024, AsyncCallback asyncCallBack = null)
        {
            if (asyncCallBack == null) { asyncCallBack = new AsyncCallback(SendCallback); }
            socket.BeginSend(buffer, 0, buffer.Length, 0, asyncCallBack, new StateObject(bufferSize));
        }
        private void SendCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as StateObject;
            try { state.ByteCount = state.WorkSocket.EndSend(ar); }
            catch { state.ByteCount = 0; state.Buffer = new byte[0]; }
        }

        public void Receive(StateObject state = null, AsyncCallback asyncCallBack = null)
        {
            if (state == null) { state = new StateObject(); }
            if (asyncCallBack == null) { asyncCallBack = new AsyncCallback(ReceiveCallback); }
            state.WorkSocket = socket;
            socket.BeginReceive(state.Buffer, 0, state.BufferSize, 0, asyncCallBack, state);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as StateObject;
            try { state.ByteCount = state.WorkSocket.EndReceive(ar); }
            catch { state.ByteCount = 0; state.Buffer = new byte[0]; }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
