using System;
using System.Runtime.InteropServices;

namespace Inaba.CPK
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CpkHeader
    {
        public Int32 Zero;
        public Int32 CompressedSize;
        public Int32 UncompressedSize;
        public CpkCompression CompressionType; // 1

        public void Check()
        {
            if (Zero != 0)
                throw new NotSupportedException("if (Zero != 0)");
            if (CompressionType != CpkCompression.Huffman)
                throw new NotSupportedException("if (CompressionType != CpkCompression.Huffman)");
            if (CompressedSize < 0)
                throw new NotSupportedException("if (CompressedSize < 0)");
            if (UncompressedSize < 0)
                throw new NotSupportedException("if (UncompressedSize < 0)");
        }
    }
}