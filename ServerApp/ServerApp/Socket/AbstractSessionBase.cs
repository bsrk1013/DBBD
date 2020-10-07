using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace dbbd
{
    public abstract class AbstractSessionBase : IDisposable
    {
        protected int handle;

        protected Socket socket;
        protected IPEndPoint remoteEndPoint;
        protected AbstractServerBase server;

        protected object lockObject = new Object();

        protected bool disposed = false;

        #region Get/Set
        public Socket Socket
        {
            get
            {
                return socket;
            }
            private set
            {
                socket = value;
            }
        }
        #endregion

        public void Dispose()
        {
            if (disposed) { return; }

            disposed = true;
        }
    }
}
