using System;
using System.Runtime.InteropServices;

namespace Inaba.PAC
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PacHeader
    {
        public static unsafe Int32 StructSize => sizeof(PacHeader);
        
        public UInt64 MagicNumber; // 0x004B4341505F5744 = "DW_PACK"
        public Int32 Zero;
        public Int32 FileCount;
        public Int32 Unknown;

        public void Check()
        {
            if (MagicNumber != 0x004B4341505F5744)
                throw new NotSupportedException("if (MagicNumber != 0x004B4341505F5744)");
            if (Zero != 0)
                throw new NotSupportedException("if (Zero != 0)");
        }
    }
}