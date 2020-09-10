using System;

using System.Threading.Tasks;
using Framework;
using NetLibsBench;

namespace ClientBench
{
    class Program
    {
        static void Main(string[] args)
        {
            var isPix = args[0] == "-p";
            var rooms = args.Length == 3 ? int.Parse(args[2]) : 10;

            ICompressor compressor;
            switch (args[1])
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

            for (var i = 0; i < rooms * 12; i++)
            {
                if (isPix) Task.Run((Action)new PixClient(compressor).Start);
                else Task.Run((Action)new LiteClient(compressor).Start);
            }

            Console.ReadLine();
        }
    }
}
