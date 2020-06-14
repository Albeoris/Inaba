using System;
using System.IO;
using Inaba.Framework;
using Inaba.Huffman;

namespace Inaba.CPK
{
    public sealed class CpkDecompressor
    {
        private readonly Stream _input;
        private readonly Stream _output;
        private readonly DataReader _reader;

        private CpkDecompressor(Stream input, Stream output, DataReader reader)
        {
            _input = input;
            _output = output;
            _reader = reader;
        }

        public static void Decompress(Stream input, Stream output, DataReader reader)
        {
            var decompressor = new CpkDecompressor(input, output, reader);
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
            CpkHeader header = _input.EnsureRead<CpkHeader>();
            header.Check();

            Boolean isCompressed = IsCompressed(header.CompressionType);
            Span<Byte> data = _reader.Read(_input, header.CompressedSize, header.UncompressedSize, isCompressed);

            _output.Write(data);
        }

        private Boolean IsCompressed(CpkCompression type)
        {
            if (type == CpkCompression.Huffman)
                return true;

            throw new NotSupportedException(type.ToString());
        }
    }
}