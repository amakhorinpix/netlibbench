using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace NetLibsBench
{
    public abstract class ServerBase
    {
        private struct PlayerIndex
        {
            public int RoomNr;
            public int Index;
        };

        private readonly Dictionary<string, PlayerIndex>
            _clients = new Dictionary<string, PlayerIndex>();

        private readonly List<List<IPEndPoint>> _endpoints = new List<List<IPEndPoint>>();
        private readonly List<List<FullMechState>> _rooms = new List<List<FullMechState>>();

        private readonly ICompressor _compressor;
        private readonly MemoryStream _memoryStream = new MemoryStream();
        private const int RoomSize = 12;
        private PerformanceCounter _requestsCounter;


        protected ServerBase(ICompressor compressor)
        {
            _compressor = compressor;
            if (!PerformanceCounterCategory.Exists("benchmarking"))
            {
                var data = new CounterCreationData("Requests Count Per Sec", "", PerformanceCounterType.RateOfCountsPerSecond32);
                var collection = new CounterCreationDataCollection();
                collection.Add(data);
                PerformanceCounterCategory.Create("benchmarking", string.Empty,
                    PerformanceCounterCategoryType.SingleInstance, collection);
            }

            _requestsCounter = new PerformanceCounter("benchmarking", "Requests Count Per Sec", false);
        }

        protected void OnReceive(byte[] buffer, int offset, int dataLength, IPEndPoint ip)
        {
            try
            {
                var decompressed = _compressor.UnCompress(buffer, offset, dataLength, out var length);
                var deserialize =
                    (FullMechState) ProtoBuf.Serializer.NonGeneric.Deserialize(typeof(FullMechState),
                        new ReadOnlyMemory<byte>(decompressed, 0, length));

                if (_clients.TryGetValue(deserialize.Id, out var index))
                {
                    _rooms[index.RoomNr][index.Index] = deserialize;
                }
                else
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
                    if (mechState.Count == RoomSize) Console.WriteLine($"{DateTime.Now} Room {roomNr + 1} started");
                    endpoints.Add(ip);
                    _clients.Add(deserialize.Id, new PlayerIndex
                    {
                        RoomNr = roomNr,
                        Index = mechState.Count - 1
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _requestsCounter.Increment();
        }


        public void Start()
        {
            Init();
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
                Read();
                SendUpdates();
            }
        }

        private void SendUpdates()
        {
            for (var i = 0; i < _rooms.Count; i++)
            {
                if (_rooms[i].Count < RoomSize) continue;
                var roomState = new WorldState
                {
                    Players = _rooms[i]
                };

                ProtoBuf.Serializer.NonGeneric.Serialize(_memoryStream, roomState);
                Send(_compressor.Compress(_memoryStream.ToArray()), _endpoints[i]);
                _memoryStream.Position = 0;
                _memoryStream.SetLength(0);
            }
        }

        protected abstract void Init();
        protected abstract void Read();
        protected abstract void Send(byte[] buffer, IEnumerable<IPEndPoint> clients);
    }
}