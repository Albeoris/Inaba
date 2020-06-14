// The MIT License (MIT)
// Copyright (c) 2017 usagirei - https://github.com/usagirei/Pac_Utils/blob/master/LICENSE
// Copyright (c) 2020 Albeoris
using System;

namespace Inaba.Huffman
{
    public class HuffmanTree
    {
        private Node _root;

        public void ReadTree(BitReader reader)
        {
            Read(reader, out _root);
        }

        private void Read(BitReader buffer, out Node n)
        {
            Boolean branch = buffer.ReadBit();
            if (branch)
            {
                Node nn = new Node();
                n = nn;
                Read(buffer, out nn.Left);
                Read(buffer, out nn.Right);
            }
            else
            {
                Byte value = buffer.ReadByte();
                n = new Node(value);
            }
        }

        internal Cursor GetCursor() => new Cursor(_root);

        internal class Node
        {
            public Node Left;
            public Node Right;
            public UInt32 Weight;
            public Byte Value;
            public Boolean IsLeaf;

            public Node()
            {
            }

            public Node(in Byte value)
            {
                Value = value;
                IsLeaf = true;
            }
        };

        internal class Cursor
        {
            private Node _node;

            public Cursor(Node node)
            {
                _node = node;
            }

            public Boolean IsLeaf => _node.IsLeaf;
            public Byte Value => _node.Value;
            public void MoveLeft() => _node = _node.Left;
            public void MoveRight() => _node = _node.Right;
        };
    }
}