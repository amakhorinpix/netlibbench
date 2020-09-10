using System;
using System.Net;
using System.Net.Sockets;
using Pixockets;
using Pixockets.DebugTools;
using Pixockets.Pools;

namespace NetLibsBench
{
    public class PixClient : BaseClient
    {
        private ThreadSafeSmartSock _socket;
        private ByteBufferPool _bufferPool;
        private PixSoc _soc = new PixSoc();

        private class PixSoc : SmartReceiverBase
        {
            public override void OnConnect(IPEndPoint endPoint)
            {
                Console.WriteLine("Connected");
            }

            public override void OnDisconnect(IPEndPoint endPoint)
            {
                Console.WriteLine("Disconnected");
            }
        }

        public PixClient(ICompressor compressor) : base(compressor)
        {
            _bufferPool = new ByteBufferPool();
        }

        public override void Send(byte[] data)
        {
            _socket.Send(data, 0, data.Length, true);
        }

        protected override void Init()
        {
            var smartSock = new SmartSock(_bufferPool, new ThreadSock(_bufferPool, AddressFamily.InterNetwork, new LoggerStub()), _soc);
            _socket = new ThreadSafeSmartSock(smartSock);
            _socket.Connect(IPAddress.Loopback, 2345);
            var receivedPacket = new ReceivedSmartPacket();
            while (_socket.State == PixocketState.Connecting)
            {
                _socket.Tick();
                _socket.Receive(ref receivedPacket);
            }
        }

        protected override void Read()
        {
            _socket.Tick();
            var packet = new ReceivedSmartPacket();
            while (_socket.Receive(ref packet))
            {
                try
                {
                    ApplyState(packet.Buffer, packet.Offset, packet.Length);
                    _bufferPool.Put(packet.Buffer);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
