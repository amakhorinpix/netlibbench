using Framework;
using NetLibsBench;

namespace ServerBench
{
    class Program
    {
        static void Main(string[] args)
        {
            var isPix = false;//args[0] == "-p";

            ICompressor compressor;
            switch ("n")//args[1])
            {
                case "-l":
                    compressor = new LZ4Compressor();
                    break;
                case "-s":
                    compressor = new SnappyCompressor();
                    break;
                default:
                    compressor = new NoCompression();
                    break;
            }

            if (isPix) new PixServer(compressor).Start();
            else new LiteServer(compressor).Start();
        }
    }
}
