using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Pixockets;
using Pixockets.DebugTools;
using Pixockets.Pools;

namespace NetLibsBench
{
    public class PixServer : ServerBase
    {
        private class PixSoc : SmartReceiverBase
        {
            public override void OnConnect(IPEndPoint endPoint)
            {
                //Console.WriteLine("Client connected");
            }

            public override void OnDisconnect(IPEndPoint endPoint)
            {
                //Console.WriteLine("Client disconnected");
            }
        }

        private SmartSock _servSock;
        private readonly PixSoc _soc;
        private ByteBufferPool _bufferPool;

        public PixServer(ICompressor compressor) : base(compressor)
        {
            _soc = new PixSoc();
        }

        protected override void Init()
        {
            _bufferPool = new ByteBufferPool();
            _servSock = new SmartSock(_bufferPool,
                new ThreadSock(_bufferPool, AddressFamily.InterNetwork, new LoggerStub()), _soc);
            _servSock.Listen(2345);
        }

        protected override void Read()
        {
            var packet = new ReceivedSmartPacket();
            while (_servSock.Receive(ref packet))
            {
                OnReceive(packet.Buffer, packet.Offset, packet.Length, packet.EndPoint);
                _bufferPool.Put(packet.Buffer);
            }
            _servSock.Tick();
        }

        protected override void Send(byte[] buffer, IEnumerable<IPEndPoint> clients)
        {
            foreach (var ip in clients)
            {
                _servSock.Send(ip, buffer, 0, buffer.Length, false);
            }
        }
    }
}
