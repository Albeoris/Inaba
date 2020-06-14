// The MIT License (MIT)
// Copyright (c) 2017 usagirei - https://github.com/usagirei/Pac_Utils/blob/master/LICENSE
// Copyright (c) 2020 Albeoris

using System;

namespace Inaba.Huffman
{
    public unsafe class BitReader
    {
        private Byte _bitLeft;
        private UInt64 _offset;
        private readonly Byte* _data;

        public BitReader(Byte* data)
        {
            _bitLeft = 8;
            _offset = 0;
            _data = data;
        }

        public Boolean ReadBit()
        {
            if (_bitLeft == 0)
            {
                _offset++;
                _bitLeft = 8;
            }

            return ((_data[_offset] >> --_bitLeft) & 0x01) == 0x01;
        }

        public Byte ReadByte()
        {
            if (_bitLeft == 8)
                return _data[_offset++];
            if (_bitLeft == 0)
                return _data[++_offset];

            Int32 first = Math.Min((Byte) 8, _bitLeft);
            Int32 second = 8 - first;

            Byte value = 0;

            Byte v1 = _data[_offset];
            v1 = (Byte) (v1 & ((1 << first) - 1));
            v1 <<= second;
            value |= v1;

            _offset++;

            Byte v2 = _data[_offset];
            v2 = (Byte) (v2 & ((1 << second) - 1 << first));
            v2 >>= first;
            value |= v2;

            return value;
        }
    }
}