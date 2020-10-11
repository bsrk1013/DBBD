using System;
using System.Collections.Generic;
using System.Text;

namespace dbbd.Util
{
    public static class SessionHandlePool
    {
        private const int maxSessionHandle = 4096;
        private static RangedIntPool pool;

        static SessionHandlePool()
        {
            pool = new RangedIntPool(1, maxSessionHandle, true);
        }

        public static int Acquire()
        {
            return pool.Acquire();
        }

        public static void Release(int value)
        {
            pool.Release(value);
        }
    }
}