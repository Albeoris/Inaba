using System;

namespace Inaba.Huffman
{
    public static class HuffmanDecompressor
    {
        public static void Decompress(Span<Byte> compressed, Span<Byte> decompressed)
        {
            unsafe
            {
                fixed (Byte* cPtr = &compressed.GetPinnableReference())
                fixed (Byte* dPtr = &decompressed.GetPinnableReference())
                {
                    DecompressorContext context = DecompressorContext.PrepareDecompression(cPtr, checked((UInt32) compressed.Length), checked((UInt32) decompressed.Length));
                    context.Decompress(dPtr);
                }
            }
        }
    }
}