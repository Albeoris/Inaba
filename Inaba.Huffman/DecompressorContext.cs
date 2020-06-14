// The MIT License (MIT)
// Copyright (c) 2017 usagirei - https://github.com/usagirei/Pac_Utils/blob/master/LICENSE
// Copyright (c) 2020 Albeoris

using System;

namespace Inaba.Huffman
{
    public unsafe class DecompressorContext
    {
        private UInt32 _inputSize;
        private UInt32 _outputSize;
        private UInt32 _blockSize;
        private UInt32 _blockCount;

        private UInt32[] _chunkDecSizes;
        private UInt32[] _chunkCmpSizes;
        private UInt32[] _chunkDataOffsets;
        private Byte* _dataInput;

        private void SetInput(Byte* buf, UInt32 size)
        {
            _inputSize = size;
            _dataInput = buf;
        }

        private void SetOutput(UInt32 size)
        {
            _outputSize = size;
        }

        private void SetBlocks(UInt32 count, UInt32 size)
        {
            _blockCount = count;
            _blockSize = size;

            _chunkCmpSizes = new UInt32[(Int32) count];
            _chunkDecSizes = new UInt32[(Int32) count];
            _chunkDataOffsets = new UInt32[(Int32) count];
        }

        private void SetDataChunk(Int32 n, UInt32 decompressedSize, UInt32 compressedSize, UInt32 dataOffset)
        {
            if (n > _blockCount)
                throw new ArgumentOutOfRangeException(nameof(n), "if (n > _blockCount)");

            _chunkCmpSizes[n] = compressedSize;
            _chunkDecSizes[n] = decompressedSize;
            _chunkDataOffsets[n] = dataOffset;
        }

        public static DecompressorContext PrepareDecompression(Byte* data, UInt32 compressedSize, UInt32 decompressedSize)
        {
            UInt32* data32 = (UInt32*) data;

            UInt32 magic = *data32++;
            if (magic != 0x1234)
                throw new NotSupportedException("if (magic != 0x1234)");

            UInt32 blkCnt = *data32++;
            UInt32 blkSz = *data32++;
            UInt32 headerSz = *data32++;

            if (headerSz != 16 + 12 * blkCnt)
                throw new NotSupportedException("if (header_sz != 16 + 12 * blk_cnt)");

            var info = new DecompressorContext();
            info.SetInput(data, compressedSize);
            info.SetBlocks(blkCnt, blkSz);

            UInt32 outputSize = 0;
            for (Int32 i = 0; i < blkCnt; ++i)
            {
                UInt32 decSz = *data32++;
                UInt32 cmpSz = *data32++;
                UInt32 offset = *data32++;

                outputSize += decSz;
                info.SetDataChunk(i, decSz, cmpSz, offset);
            }

            if (decompressedSize != outputSize)
                throw new NotSupportedException("if (uncompressedSize != output_size)");

            info.SetOutput(outputSize);

            return info;
        }

        public void Decompress(Byte* dst)
        {
            UInt32 blkCnt = _blockCount;
            UInt32 blkSz = _blockSize;
            UInt32 inputSz = _inputSize;
            Byte* inputBuf = _dataInput;

            Byte* dst8 = dst;
            UInt32 headerSize = 16 + 12 * blkCnt;

            // TODO: Parallel
            UInt32 dstOffset = 0;
            for (Int32 i = 0; i < blkCnt; ++i)
            {
                UInt32 inChunkSz = _chunkCmpSizes[i];
                UInt32 outChunkSz = _chunkDecSizes[i];
                UInt32 srcOffset = _chunkDataOffsets[i];

                DecompressBlock(dst8 + dstOffset,
                    outChunkSz,
                    inputBuf + srcOffset + headerSize,
                    inChunkSz);

                dstOffset += outChunkSz;
            }
        }

        private static void DecompressBlock(Byte* dst, UInt32 dstSize, Byte* src, UInt32 srcSize)
        {
            BitReader reader = new BitReader(src);
            HuffmanTree huffmanTree = new HuffmanTree();
            huffmanTree.ReadTree(reader);

            for (UInt32 i = 0; i < dstSize; ++i)
            {
                var cur = huffmanTree.GetCursor();
                while (!cur.IsLeaf)
                {
                    Boolean moveRight = reader.ReadBit();
                    if (moveRight)
                        cur.MoveRight();
                    else
                        cur.MoveLeft();
                }

                dst[i] = cur.Value;
            }
        }
    }
}