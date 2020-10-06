using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace dbbd.Util
{
    /// <summary>
    /// 읽기 락
    /// </summary>
    public class ReadLock : IDisposable
    {
        private ReaderWriterLockSlim rlock = null;

        public ReadLock(ReaderWriterLockSlim rlock)
        {
            this.rlock = rlock;
            rlock.EnterReadLock();
        }

        public void Dispose()
        {
            rlock.ExitReadLock();
        }
    }

    /// <summary>
    ///  읽기중 쓰기 락 가능
    /// </summary>
    public class UpgradeableReadLock : IDisposable
    {
        private ReaderWriterLockSlim urlock = null;

        public UpgradeableReadLock(ReaderWriterLockSlim urlock)
        {
            this.urlock = urlock;
            urlock.EnterUpgradeableReadLock();
        }

        public void Dispose()
        {
            urlock.ExitUpgradeableReadLock();
        }
    }

    /// <summary>
    /// 쓰기 락
    /// </summary>
    public class WriteLock : IDisposable
    {
        private ReaderWriterLockSlim wlock = null;

        public WriteLock(ReaderWriterLockSlim wlock)
        {
            this.wlock = wlock;
            wlock.EnterWriteLock();
        }

        public void Dispose()
        {
            wlock.ExitWriteLock();
        }
    }
}
