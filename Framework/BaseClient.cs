using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace NetLibsBench
{
    public abstract class BaseClient
    {
        private FullMechState _state;
        private static readonly Random _rnd = new Random();
        private readonly ICompressor _compressor;
        private readonly MemoryStream _memoryStream = new MemoryStream();

        protected BaseClient(ICompressor compressor)
        {
            _compressor = compressor;
            _state = GenerateState();
        }

        public void Start()
        {
            Init();
            SendInit();
            while (!Console.KeyAvailable)
            {
                Read();
                SendUpdate();
                Thread.Sleep(100);
            }
        }
        public abstract void Send(byte[] data);

        protected abstract void Init();
        protected abstract void Read();

        protected void SendInit()
        {
            ProtoBuf.Serializer.NonGeneric.Serialize(_memoryStream, _state);
            var serialized = _memoryStream.ToArray();
            try
            {
                var compressed = _compressor.Compress(serialized);
                Send(compressed);
                _memoryStream.Position = 0;
                _memoryStream.SetLength(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SendUpdate()
        {
            unchecked
            {
                _state.Mechs[_rnd.Next(0, _state.Mechs.Length)].Pos++;
            }

            ProtoBuf.Serializer.NonGeneric.Serialize(_memoryStream, _state);
            var compressed = _compressor.Compress(_memoryStream.ToArray());
            Send(compressed);
            _memoryStream.Position = 0;
            _memoryStream.SetLength(0);
        }
        protected void ApplyState(byte[] compressed, int offset, int size)
        {
            var decompressed = _compressor.UnCompress(compressed, offset, size, out var length);
            var deserialize =
                (WorldState) ProtoBuf.Serializer.NonGeneric.Deserialize(typeof(WorldState),
                    new ReadOnlyMemory<byte>(decompressed, 0, length));
            foreach (var player in deserialize.Players)
            {
                if (player.Id == _state.Id) _state = player;
            }
        }

        public static FullMechState GenerateState()
        {
            var effectKinds = Enum.GetValues(typeof(EffectKind));
            var applied = new List<AppliedEffect>();
            var mechs = new List<FullMechState.MechState>();
            var parts = new List<FullMechState.PartHealth>();
            var partState = new List<MechSpawnSuccessMessage.MechPartState>();
            for (var j = 0; j < 1; j++)
            {
                for (var i = 0; i < 3; i++)
                {
                    var partId = new PartId
                    {
                        Index = (byte)i,
                        Kind = (PartKind)i
                    };

                    var maxHealth = _rnd.Next(10000, 100000);

                    parts.Add(new FullMechState.PartHealth
                    {
                        CurrentHealth = maxHealth,
                        HealLimit = maxHealth,
                        IsActive = true,
                        PartId = partId
                    });

                    partState.Add(new MechSpawnSuccessMessage.MechPartState()
                    {
                        PartId = partId,
                        RestoreRate = _rnd.Next(0, 100),
                        ActivationHealth = _rnd.Next(0, 100),
                        MaxHealth = maxHealth
                    });
                }

                for (var i = 0; i < 5; i++)
                {
                    applied.Add(new AppliedEffect((short)_rnd.Next(1000, 13000), (short)_rnd.Next(1000, 13000), _rnd.Next(0, 1000),
                        (EffectKind)effectKinds.GetValue(_rnd.Next(0, effectKinds.Length)), _rnd.Next(0, int.MaxValue), _rnd.Next(0, 1000), true,
                        true));
                }

                var viewId = _rnd.Next(1000, 13000);
                mechs.Add(new FullMechState.MechState
                {
                    ViewId = viewId,
                    AppliedEffects = applied.ToArray(),
                    PartsHealth = parts.ToArray(),
                    //SpawnMessage = new MechSpawnSuccessMessage
                    //{
                    //    PointIndex = _rnd.Next(0, 64),
                    //    SpawnType = 1,
                    //    TeamIndex = 1,
                    //    State = new MechSpawnSuccessMessage.MechState
                    //    {
                    //        PartStates = partState.ToArray()
                    //    },
                    //    SpawnedDrone = new PresetDrone
                    //    {
                    //        Id = Guid.NewGuid().ToString(),
                    //        Current = true,
                    //        Slot = (byte)_rnd.Next(0, 6),
                    //        SpawnCount = 1,
                    //        TypeId = Guid.NewGuid().ToString(),
                    //        Chips = Enumerable.Range(0, 10).Select(_ => new PresetChip
                    //        {
                    //            Slot = (byte)_,
                    //            TypeId = Guid.NewGuid().ToString()
                    //        }).ToArray()
                    //    },
                    //    SpawnedMech = new PresetMech
                    //    {
                    //        ViewId = viewId,
                    //        Id = Guid.NewGuid().ToString(),
                    //        IsAlive = true,
                    //        ItemId = Guid.NewGuid().ToString(),
                    //        SpawnsCount = 1,
                    //        SkinId = Guid.NewGuid().ToString(),
                    //        SlotNumber = _rnd.Next(0, 5),
                    //        MechKind = MechKind.Robot,
                    //        PresetIndex = 1,
                    //        Pilot = new PresetPilot
                    //        {
                    //            Id = Guid.NewGuid().ToString(),
                    //            Skills = Enumerable.Range(0, 10).Select(_ => new PresetPilotSkill
                    //            {
                    //                Id = Guid.NewGuid().ToString(),
                    //                IsActive = true,
                    //                Level = (byte)_rnd.Next(1, 10)
                    //            }).ToArray()
                    //        },
                    //        BranchesWithLevels = Enumerable.Range(0, 10).Select(_ => new BranchWithLevel
                    //        {
                    //            Id = Guid.NewGuid().ToString(),
                    //            Level = _rnd.Next(1, 10),
                    //            Rank = _rnd.Next(1, 2)
                    //        }).ToArray(),
                    //        Equip = Enumerable.Range(0, 10).Select(_ => new PresetMechEquip
                    //        {
                    //            Id = Guid.NewGuid().ToString(),
                    //            Level = _rnd.Next(1, 10),
                    //            Rank = _rnd.Next(1, 2),
                    //            ItemId = Guid.NewGuid().ToString(),
                    //            Seed = _rnd.Next(0, 1000),
                    //            SlotNumber = _rnd.Next(0, 10)
                    //        }).ToList(),
                    //        Modules = Enumerable.Range(0, 10).Select(_ => new PresetMechEquip
                    //        {
                    //            Id = Guid.NewGuid().ToString(),
                    //            Level = _rnd.Next(1, 10),
                    //            Rank = _rnd.Next(1, 2),
                    //            ItemId = Guid.NewGuid().ToString(),
                    //            Seed = _rnd.Next(0, 1000),
                    //            SlotNumber = _rnd.Next(0, 10)
                    //        }).ToList()

                    //    }
                    //}
                });

                applied.Clear();
                parts.Clear();
                partState.Clear();
            }


            return new FullMechState
            {
                Id = Guid.NewGuid().ToString(),
                Mechs = mechs.ToArray()
            };
        }
    }
}