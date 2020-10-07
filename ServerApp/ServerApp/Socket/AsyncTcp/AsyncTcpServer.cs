using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace dbbd
{
    public class AsyncTcpServer : AbstractTcpServer
    {
        private const int numConcurrentAcceptors = 16;
        private SocketAsyncEventArgs[] acceptEventArgs;

        public AsyncTcpServer(string name)
            :base(name)
        {
            acceptEventArgs = new SocketAsyncEventArgs[numConcurrentAcceptors];
            for(int i = 0; i < numConcurrentAcceptors; i++)
            {
                var saea = new SocketAsyncEventArgs();
                saea.Completed += Saea_Completed;
                acceptEventArgs[i] = saea;
            }
        }

        private void Saea_Completed(object sender, SocketAsyncEventArgs e)
        {
            OnAccept(e);
        }

        protected override void AcceptInternal()
        {
            foreach(var saea in acceptEventArgs)
            {
                AcceptImpl(saea);
            }
        }

        private void AcceptImpl(SocketAsyncEventArgs e)
        {
            e.AcceptSocket = null;

            bool pending = socket.AcceptAsync(e);
            if (!pending)
            {
                OnAccept(e);
            }
        }

        private void OnAccept(SocketAsyncEventArgs e)
        {
            try
            {
                if(e.SocketError == SocketError.Success)
                {
                    var clientSocket = e.AcceptSocket;
                    var session = new Object();// new AsyncTcpSession(this, clientSocket);

                    //if(!OnAcceptInternal(session))
                    //{
                    //    session.CloseInternal();
                    //}
                }
                else if(e.SocketError == SocketError.OperationAborted)
                {
                    /// FIXME 로그 작성
                    return;
                }
                else
                {
                    /// FIXME 로그 작성
                }
            }
            catch(ObjectDisposedException)
            {
                /// FIXME 로그 작성
            }
            catch(Exception ex)
            {
                /// FIXME 로그 작성
            }

            AcceptImpl(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { return; }

            foreach(var saea in acceptEventArgs)
            {
                saea.Completed -= Saea_Completed;
                saea.Dispose();
            }

            acceptEventArgs = null;

            base.Dispose(disposing);
        }
    }
}
