using System;
using System.Runtime.InteropServices;

namespace Inaba.PAC
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PacEntry
    {
        public static unsafe Int32 StructSize => sizeof(PacEntry); 
            
        public Int32 Unknown2;
        public Int16 FileId;
        public Int16 Unknown4;
        private unsafe fixed SByte _name[264];
        public Int32 CompressedSize;
        public Int32 UncompressedSize;
        public PakCompression CompressionType;
        public UInt32 Offset;

        public String RelativePath
        {
            get
            {
                unsafe
                {
                    fixed (SByte* ptr = _name)
                        return new String(ptr);
                }
            }
        }
    }
}