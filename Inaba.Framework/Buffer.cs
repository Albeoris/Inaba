using System;

namespace Inaba.Framework
{
    public sealed class Buffer
    {
        private Byte[] _buff = Array.Empty<Byte>();

        public Span<Byte> GetSpan(Int32 capacity)
        {
            if (capacity > _buff.Length)
                _buff = new Byte[EstimateCapacity(capacity)];

            return new Span<Byte>(_buff, 0, capacity);
        }

        private static Int64 EstimateCapacity(Int32 capacity)
        {
            return Math.Min(Int32.MaxValue, (Int64) (capacity * 1.5));
        }
    }
}