using System;
using System.IO;
using System.Linq;
using NetLibsBench;

namespace CompressionBench
{
    class Program
    {
        static void Main(string[] args)
        {
            var lz4 = new LZ4Compressor();
            var snappy = new SnappyCompressor();
            for (var i = 0; i < 100; i++)
            {
                var model = new WorldState2()
                {
                    Players = Enumerable.Range(0,1000).Select(_ => 1).ToArray()
                };
                byte[] serialized;
                using (var m = new MemoryStream())
                {
                    ProtoBuf.Serializer.NonGeneric.Serialize(m, model);
                    serialized = m.ToArray();
                }

                var lz4Length = lz4.Compress(serialized).Length;
                var snappLength = snappy.Compress(serialized).Length;
                Console.WriteLine($"{serialized.Length}\t{lz4Length}\t{snappLength}\t{lz4Length - snappLength}");
            }
            
            Console.ReadLine();
        }
    }
}
