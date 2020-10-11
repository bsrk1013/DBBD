using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace dbbd
{
    public abstract class AbstractSessionBase : IDisposable
    {
        protected Socket socket;
        protected IPEndPoint remoteEndPoint;
        protected AbstractServerBase server;

        protected object lockObject = new Object();

        protected bool disposed = false;
        protected bool rxBeginning = false;
        protected bool rxTransformed = false;
        protected bool txFlag = false;


        #region Get/Set
        public int Handle { get; set; }
        public bool Connected { get; set; }
        public object LockObject { get { return lockObject; } }
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

        #region Receive
        internal void BeginReceive(bool beginning)
        {
            lock(lockObject)
            {
                rxBeginning = beginning;
            }

            ReceiveInternal();
        }

        protected abstract void ReceiveInternal();
        #endregion

        #region Send
        internal void BeginSend()
        {
        }
        #endregion

        public void Dispose()
        {
            if (disposed) { return; }

            disposed = true;
        }
    }
}
