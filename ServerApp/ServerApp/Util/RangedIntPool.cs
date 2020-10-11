using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace dbbd.Util
{
    public class RangedIntPool
    {
        private bool advancing;
        private int minValue;
        private int maxValue;
        private int position;
        private BitArray bitArray;

        public int Length { get { return bitArray.Length; } }

        public RangedIntPool(int maxValue)
            : this(0, maxValue)
        {}

        public RangedIntPool(int maxValue, bool advancing)
            : this(0, maxValue, advancing)
        {}

        public RangedIntPool(int minValue, int maxValue)
            : this(minValue, maxValue, false)
        {}

        public RangedIntPool(int minValue, int maxValue, bool advancing)
        {
            if(minValue > maxValue)
            {
                throw new ArgumentException();
            }

            this.advancing = advancing;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.bitArray = new BitArray(maxValue - minValue - 1);
            
            this.position = 0;
        }

        public int Acquire()
        {
            lock(bitArray)
            {
                int index = position;
                int length = Length;
                for(int i = 0; i < length; i++, index++)
                {
                    // index가 maxValue를 넘어서면
                    if(index >= length) { index = 0; }
                    // 이미 쓰고 있는 index는 넘어간다.
                    if (bitArray[index]) { continue; }

                    bitArray.Set(index, true);

                    if(advancing)
                    {
                        position = index + 1;
                        if(position >= length) { position = 0; }
                    }

                    return minValue + index;
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        public void Release(int value)
        {
            if(value < minValue || maxValue < value)
            {
                throw new ArgumentOutOfRangeException();
            }

            int index = value - minValue;
            lock(bitArray)
            {
                if(bitArray[index])
                {
                    bitArray.Set(index, false);
                }
            }
        }
    }
}
