using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NetLibsBench
{
    public abstract class ServerBase
    {
        private struct PlayerIndex
        {
            public int RoomNr;
            public int Index;
        };

        private readonly ConcurrentDictionary<string, PlayerIndex>
            _clients = new ConcurrentDictionary<string, PlayerIndex>();

        private readonly List<List<IPEndPoint>> _endpoints = new List<List<IPEndPoint>>();
        private readonly List<List<FullMechState>> _rooms = new List<List<FullMechState>>();

        private readonly ICompressor _compressor;
        private readonly MemoryStream _memoryStream = new MemoryStream();
        private const int RoomSize = 12;

        protected ServerBase(ICompressor compressor)
        {
            _compressor = compressor;
        }


        protected void OnReceive(byte[] buffer, int offset, int dataLength, IPEndPoint ip)
        {
            try
            {
                var decompressed = _compressor.UnCompress(buffer, offset, dataLength, out var length);
                var deserialize =
                    (FullMechState)ProtoBuf.Serializer.NonGeneric.Deserialize(typeof(FullMechState),
                        new ReadOnlyMemory<byte>(decompressed, 0, length));

                if (_clients.TryGetValue(deserialize.Id, out var index))
                {
                    _rooms[index.RoomNr][index.Index] = deserialize;
                }
                else
                {
                    lock (_rooms)
                    {
                        List<FullMechState> mechState;
                        List<IPEndPoint> endpoints;
                        var roomNr = _rooms.Count - 1;
                        if (roomNr < 0 || _rooms[roomNr].Count == RoomSize)
                        {
                            mechState = new List<FullMechState>();
                            endpoints = new List<IPEndPoint>();
                            _rooms.Add(mechState);
                            _endpoints.Add(endpoints);
                            roomNr += 1;
                        }
                        else
                        {
                            mechState = _rooms[roomNr];
                            endpoints = _endpoints[roomNr];
                        }
                        mechState.Add(deserialize);
                        if (mechState.Count == RoomSize) Task.Run(() => RoomThread(roomNr));
                        endpoints.Add(ip);
                        _clients.TryAdd(deserialize.Id, new PlayerIndex
                        {
                            RoomNr = roomNr,
                            Index = mechState.Count - 1
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }


        public void Start()
        {
            Init();
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
                Read();
            }
        }

        private void RoomThread(int roomIndex)
        {
            Console.WriteLine($"Room {roomIndex + 1} started");
            while (!Console.KeyAvailable)
            {
                try
                {
                    Thread.Sleep(100);
                    var roomState = new WorldState
                    {
                        Players = _rooms[roomIndex].ToArray()
                    };

                    lock (_memoryStream)
                    {
                        ProtoBuf.Serializer.NonGeneric.Serialize(_memoryStream, roomState);
                        Send(_compressor.Compress(_memoryStream.ToArray()), _endpoints[roomIndex]);
                        _memoryStream.Position = 0;
                        _memoryStream.SetLength(0);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
        }

        protected abstract void Init();
        protected abstract void Read();
        protected abstract void Send(byte[] buffer, IEnumerable<IPEndPoint> clients);
    }
}