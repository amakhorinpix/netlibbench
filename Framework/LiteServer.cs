using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;

namespace NetLibsBench
{
    public class LiteServer : ServerBase, INetEventListener
    {
        private ConcurrentDictionary<IPEndPoint, NetPeer> _clients = new ConcurrentDictionary<IPEndPoint, NetPeer>();
        private NetManager _server;
        public LiteServer(ICompressor compressor) : base(compressor)
        {
            
        }

        protected override void Init()
        {
            _server = new NetManager(this);
            _server.Start(2345);
        }

        protected override void Read()
        {
            _server.PollEvents();
        }

        protected override void Send(byte[] buffer, IEnumerable<IPEndPoint> clients)
        {
            foreach (var ip in clients)
            {
                if(_clients.TryGetValue(ip, out var peer)) peer.Send(buffer, DeliveryMethod.ReliableOrdered);
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            _clients.TryAdd(peer.EndPoint, peer);
            //Console.WriteLine("Client connected");
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            _clients.TryRemove(peer.EndPoint, out _);
            //Console.WriteLine("Client disconnected");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            throw new NotImplementedException();
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            OnReceive(reader.RawData, reader.UserDataOffset, reader.UserDataSize, peer.EndPoint);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            throw new NotImplementedException();
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Accept();
        }
    }
}
