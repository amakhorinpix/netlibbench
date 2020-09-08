using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;

namespace NetLibsBench
{
    public class LiteClient : BaseClient, INetEventListener
    {
        private NetPeer _peer;
        private NetManager _manager;

        public LiteClient(ICompressor compressor) : base(compressor)
        {
        }

        public override void Send(byte[] data)
        {
            _peer.Send(data, DeliveryMethod.ReliableOrdered);
        }

        protected override void Init()
        {
            _manager = new NetManager(this);
            _manager.Start();
            _peer = _manager.Connect(IPAddress.Loopback.ToString(), 2345, "SomeConnectionKey");
        }

        protected override void Read()
        {
            _manager.PollEvents();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine("connected");
            SendInit();
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("disconnected");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            ApplyState(reader.RawData, reader.UserDataOffset, reader.UserDataSize);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            
        }
    }
}
