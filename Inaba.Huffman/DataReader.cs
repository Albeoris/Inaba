using System;
using System.IO;
using Inaba.Framework;
using Buffer = Inaba.Framework.Buffer;

namespace Inaba.Huffman
{
    public sealed class DataReader
    {
        private readonly Buffer _data = new Buffer();
        private readonly Buffer _decompressed = new Buffer();

        public Span<Byte> Read(Stream input, Int32 compressedSize, Int32 uncompressedSize, Boolean isCompressed)
        {
            Span<Byte> data = _data.GetSpan(compressedSize);
            input.EnsureRead(data);

            if (!isCompressed)
            {
                if (compressedSize != uncompressedSize)
                    throw new NotSupportedException("if (compressedSize != uncompressedSize)");
                
                return data;
            }

            Span<Byte> decompressed = _decompressed.GetSpan(uncompressedSize);
            HuffmanDecompressor.Decompress(data, decompressed);

            return decompressed;
        }
    }
}