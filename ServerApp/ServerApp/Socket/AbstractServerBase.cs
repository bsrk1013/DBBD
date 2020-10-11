using dbbd.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace dbbd
{
    public abstract class AbstractServerBase : IDisposable
    {
        protected string name;
        protected bool disposed = false;
        protected SortedList<int, AbstractSessionBase> sessionMap = null;
        protected Socket socket = null;

        protected ReaderWriterLockSlim rwlock;

        protected AbstractServerBase(string name)
        {
            this.name = name;
            this.rwlock = new ReaderWriterLockSlim();
            this.sessionMap = new SortedList<int, AbstractSessionBase>();
        }

        #region Get/Set
        public bool Listening
        {
            get { return (socket != null && socket.IsBound); }
        }

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

        public int SessionCount()
        {
            using (new ReadLock(rwlock))
            {
                return sessionMap.Count;
            }
        }
        #endregion

        #region Send
        public virtual void Send(int handle)
        {
            AbstractSessionBase session = null;
            using (new ReadLock(rwlock))
            {
                if (!sessionMap.TryGetValue(handle, out session)) { return; }
            }
            //session.Send()
        }

        public virtual void Broadcast()
        {
            var sessionList = GetSessionList();
            foreach (var session in sessionList)
            {
                //session.Send();
            }
        }
        #endregion

        #region Accept
        // 상속받는 서버에서 Accept 후처리 구현
        protected abstract void AcceptInternal();

        protected virtual bool OnAcceptInternal(AbstractSessionBase session)
        {
            if(session == null) { return false; }
            InitSession(session);
            return true;
        }

        private void InitSession(AbstractSessionBase session)
        {
            // FIXME 하트비트 등 session 기능 추가
            lock (session.LockObject)
            {
                session.Handle = SessionHandlePool.Acquire();
                session.Connected = true;
            }

            session.BeginReceive(true);

            using (new WriteLock(rwlock))
            {
                sessionMap.Add(session.Handle, session);
            }
        }
        #endregion

        protected void OnSessionDisconnectedInternal(AbstractSessionBase session)
        {
            SessionHandlePool.Release(session.Handle);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) { return; }

            rwlock.Dispose();

            var sessionList = GetSessionList();
            foreach(var session in sessionList)
            {
                //session.Close();
            }

            sessionList.Clear();

            disposed = true;
        }
    }
}
