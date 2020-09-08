using System;
using System.IO;
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
                var model = BaseClient.GenerateState();
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
