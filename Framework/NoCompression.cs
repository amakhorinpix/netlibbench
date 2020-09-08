using System;
using NetLibsBench;

namespace Framework
{
    public class NoCompression : ICompressor
    {
        public byte[] Compress(byte[] proto)
        {
            return proto;
        }

        public byte[] UnCompress(byte[] compressedProto, int offset, int length, out int outLength)
        {
            outLength = length;
            var uncompressed = new byte[length];
            Array.Copy(compressedProto, offset, uncompressed, 0, length);
            return uncompressed;
        }
    }
}
