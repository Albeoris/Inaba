using System;
using System.IO;
using Inaba.Framework;
using Inaba.Huffman;

namespace Inaba.PAC
{
    public sealed class PacUnpacker
    {
        private readonly Stream _input;
        private readonly DataReader _reader;
        private readonly IOutputPathBuilder _pathBuilder;

        public PacUnpacker(Stream input, DataReader reader, IOutputPathBuilder pathBuilder)
        {
            _input = input;
            _reader = reader;
            _pathBuilder = pathBuilder;
        }

        public void Unpack()
        {
            PacHeader header = _input.EnsureRead<PacHeader>();
            header.Check();

            PacEntry[] entries = _input.EnsureRead<PacEntry>(header.FileCount);
            Int64 dataPosition = _input.Position;

            for (Int32 index = 0; index < entries.Length; index++)
            {
                ref var entry = ref entries[index];
                Boolean isCompressed = PacDecompressor.IsCompressed(entry.CompressionType);

                _input.SetPosition(dataPosition + entry.Offset);
                Span<Byte> data = _reader.Read(_input, entry.CompressedSize, entry.UncompressedSize, isCompressed);

                String outputPath = _pathBuilder.GetOutputPath(entry.RelativePath, entry.FileId);
                FileSystem.CreateFileDirectory(outputPath);

                using (var output = File.Create(outputPath))
                    output.Write(data);
            }
        }
    }
}