using K4os.Compression.LZ4;

namespace NetLibsBench
{
    public class LZ4Compressor : ICompressor
    {
        public byte[] Compress(byte[] proto)
        {
            return LZ4Pickler.Pickle(proto);
        }

        public byte[] UnCompress(byte[] compressedProto, int offset, int length, out int outLength)
        {
            var unwrapped = LZ4Pickler.Unpickle(compressedProto, offset, length);
            outLength = unwrapped.Length;
            return unwrapped;
        }
    }
}
