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
    public abstract class AbstractTcpServer : IDisposable
    {
        protected string name;

        protected Socket socket = null;
        protected bool disposed = false;
        protected SortedList<int, Object> sessionMap = null;

        protected ReaderWriterLockSlim rwlock;

        protected AbstractTcpServer(string name)
        {
            this.name = name;

            rwlock = new ReaderWriterLockSlim();
            sessionMap = new SortedList<int, object>();
        }

        #region Get/Set
        public Object GetSession(int handle)
        {
            using (new ReadLock(rwlock))
            {
                sessionMap.TryGetValue(handle, out var session);
                return session;
            }
        }

        public IList GetSessionList()
        {
            using (new ReadLock(rwlock))
            {
                return new List<Object>(sessionMap.Values);
            }
        }
        #endregion

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

        // 상속받는 서버에서 Accept 후처리 구현
        protected abstract void AcceptInternal();

        protected virtual bool OnAcceptInternal(Object session)
        {
            var tcpSession = (Object)session;

            try
            {
                if (tcpSession == null) { return false; }

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

        #region Send
        public virtual void Send(int handle)
        {
            Object session = null;
            using (new ReadLock(rwlock))
            {
                if (!sessionMap.TryGetValue(handle, out session)) { return; }
            }
            //session.Send()
        }

        public virtual void Broadcast()
        {
            var sessionList = GetSessionList();
            foreach(var session in sessionList)
            {
                //session.Send();
            }
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) { return; }

            if(socket != null)
            {
                socket.Close();
            }

            rwlock.Dispose();
        }
    }
}