using dbbd.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace dbbd
{
    public abstract class AbstractTcpServer : AbstractServerBase
    {

        protected AbstractTcpServer(string name)
            :base(name)
        {
        }

        #region Listen/Accept
        public void Listen(int port)
        {
            Listen(IPAddress.Any, port);
        }

        public void Listen(string ip, int port)
        {
            Listen(IPAddress.Parse(ip), port);
        }

        public void Listen(IPAddress ip, int port)
        {
            if(socket != null) { throw new InvalidOperationException(); }

            EndPoint endPoint = new IPEndPoint(ip, port);

            try
            {
                socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(endPoint);
                socket.Listen(int.MaxValue);

                AcceptInternal();

                /// FIXME 로그 작성
            }
            catch (Exception e)
            {
                /// FIXME 로그 작성
                throw;
            }
        }

        protected override bool OnAcceptInternal(AbstractSessionBase session)
        {
            var tcpSession = (AbstractSessionBase)session;

            try
            {
                if (!base.OnAcceptInternal(session)) { return false; }
                
                //var clientSocket = tcpSession.Socket;

                // 소켓 옵션 설정
                //clientSocket.Nodelay = NoDelay;

                // 세션 수신 시작
                //tcpSession.BeginReceive(true);

                /// FIXME 로그 작성

                return true;
            }
            catch (ObjectDisposedException)
            {
                // 바로 연결 끊김
                /// FIXME 로그 작성

                return false;
            }
            catch (Exception e)
            {
                /// FIXME 로그 작성

                return false;
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposed) { return; }

            if(socket != null)
            {
                socket.Close();
            }

            base.Dispose(disposing);
        }
    }
}