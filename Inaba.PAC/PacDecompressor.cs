using System;
using System.IO;
using System.Linq;
using Inaba.Framework;
using Inaba.Huffman;

namespace Inaba.PAC
{
    public sealed class PacDecompressor
    {
        private readonly Stream _input;
        private readonly Stream _output;
        private readonly DataReader _reader;

        private PacDecompressor(Stream input, Stream output, DataReader reader)
        {
            _input = input;
            _output = output;
            _reader = reader;
        }

        public static void Decompress(Stream input, Stream output, DataReader reader)
        {
            var decompressor = new PacDecompressor(input, output, reader);
            decompressor.Decompress();
        }

        public static void Decompress(String inputFile, String outputFile, DataReader reader)
        {
            FileSystem.CreateFileDirectory(outputFile);
            using var input = File.OpenRead(inputFile);
            using var output = File.Create(outputFile);
            Decompress(input, output, reader);
        }

        private void Decompress()
        {
            PacHeader header = _input.EnsureRead<PacHeader>();
            header.Check();

            PacEntry[] entries = _input.EnsureRead<PacEntry>(header.FileCount);

            Int64 beginPosition = _output.Position;
            Int32 headerSize = PacHeader.StructSize + PacEntry.StructSize * entries.Length;
            Int64 decompressedArchiveSize = headerSize + entries.Sum(e => (Int64)e.UncompressedSize);
            Int64 totalSize = beginPosition + decompressedArchiveSize;
            
            if (totalSize > UInt32.MaxValue)
                throw new NotSupportedException("The game cannot address more than 4 GB of files. You can unpack this file, but you cannot decompress it.");
            
            _output.SetLength(totalSize);
            _output.Seek(headerSize, SeekOrigin.Current);

            Int64 dataPosition = _output.Position;
            for (Int32 index = 0; index < entries.Length; index++)
            {
                ref var entry = ref entries[index];
                Boolean isCompressed = IsCompressed(entry.CompressionType);
                Span<Byte> data = _reader.Read(_input, entry.CompressedSize, entry.UncompressedSize, isCompressed);

                entry.Offset = checked((UInt32) (_output.Position - dataPosition));
                entry.CompressedSize = entry.UncompressedSize;
                entry.CompressionType = PakCompression.None;

                _output.Write(data);
            }

            _output.Seek(beginPosition, SeekOrigin.Begin);
            _output.Write(header);
            _output.Write(entries);
        }

        public static Boolean IsCompressed(PakCompression type) => type switch
        {
            PakCompression.None => false,
            PakCompression.Huffman => true,
            _ => throw new NotSupportedException(type.ToString())
        };
    }
}