namespace NetLibsBench
{
    public interface ICompressor
    {
        byte[] Compress(byte[] proto);
        byte[] UnCompress(byte[] compressedProto, int offset, int length, out int outLength);
    }
}
