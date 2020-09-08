namespace NetLibsBench
{
    public class SnappyCompressor : ICompressor
    {
        private byte[] _uncompressBuffer = new byte[100000];
        public byte[] Compress(byte[] proto)
        {
            return Snappy.SnappyCodec.Compress(proto);
        }

        public byte[] UnCompress(byte[] compressedProto, int offset, int length, out int outLength)
        {
             outLength = Snappy.SnappyCodec.Uncompress(compressedProto, offset, length, _uncompressBuffer, 0);
            return _uncompressBuffer;
        }
    }
}
