using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetLibsBench
{
    [ProtoContract]
    public class WorldState
    {
        [ProtoMember(1)] public IList<FullMechState> Players;
    }

    [ProtoContract]
    public class ClientState
    {
        [ProtoMember(1)] public int X;
    }


    [ProtoContract]
    public class FullMechState
    {
        [ProtoMember(1)]
        public MechState[] Mechs;

        [ProtoMember(2)]
        public string Id;

        [ProtoContract]
        public class MechState
        {
            [ProtoMember(1)]
            public int ViewId;

            [ProtoMember(2)]
            public MechSpawnSuccessMessage SpawnMessage;

            [ProtoMember(3)]
            public PartHealth[] PartsHealth;

            [ProtoMember(4)]
            public AppliedEffect[] AppliedEffects;

            [ProtoMember(5)]
            public AccumulatedEffectCharge[] AccumulatedCharges;

            [ProtoMember(6)] public int Pos;
        }

        [ProtoContract]
        public struct PartHealth
        {
            [ProtoMember(1)]
            public bool IsActive;
            [ProtoMember(2)]
            public byte LegacyPartId;
            [ProtoMember(3)]
            public int CurrentHealth;
            [ProtoMember(4)]
            public int HealLimit;
            [ProtoMember(5)]
            public PartId PartId;
        }
    }

    [ProtoContract]
    public struct PartId
    {
        [ProtoMember(1)]
        public PartKind Kind;
        [ProtoMember(2)]
        public byte Index;
    }

    public enum PartKind : byte
    {
        MechBody = 0,
        MechSlot = 1,
        MechShield = 2,
        DroneSlot = 3,
        DroneShield = 4
    }

    [ProtoContract]
    public struct AppliedEffect
    {
        [ProtoMember(1)]
        public readonly short Source;

        [ProtoMember(2)]
        public readonly short Target;

        [ProtoMember(3)]
        public readonly int Index;

        [ProtoMember(4)]
        public readonly EffectKind Kind;

        [ProtoMember(5)]
        public readonly int EndTime;

        [ProtoMember(6)]
        public readonly float Value;

        [ProtoMember(7)]
        public readonly bool IsEndless;

        [ProtoMember(8)]
        public readonly bool FromDrone;

        public AppliedEffect(short source,
            short target,
            int index,
            EffectKind kind,
            int endTime,
            float value,
            bool isEndless,
            bool fromDrone)
        {
            Source = source;
            Target = target;
            Index = index;
            Kind = kind;
            EndTime = endTime;
            Value = value;
            IsEndless = isEndless;
            FromDrone = fromDrone;
        }
    }

    public enum EffectKind : byte
    {
        None = 0,
        Immune = 1,
        Root = 1 << 1,
        RootImmune = Root | Immune,
        Suppression = 2 << 1,
        SuppressionImmune = Suppression | Immune,
        LastStand = 3 << 1,
        LastStandImmune = LastStand | Immune,
        DamageOverTime = 4 << 1,
        DamageOverTimeImmune = DamageOverTime | Immune,
        PreDisguise = 5 << 1,
        Disguise = 6 << 1,
        BattleBorn = 7 << 1,
        BattleBornImmune = BattleBorn | Immune,
        HealOverTime = 8 << 1,
        HealOverTimeImmune = HealOverTime | Immune,
        MoreDmg = 9 << 1,
        MoreDmgImmune = MoreDmg | Immune,
        MoreSpeed = 10 << 1,
        MoreSpeedImmune = MoreSpeed | Immune,
        MoreArmor = 11 << 1,
        MoreArmorImmune = MoreArmor | Immune,
        ModuleRecharge = 12 << 1,
        ModuleRechargeImmune = ModuleRecharge | Immune,
        AbilityRecharge = 13 << 1,
        AbilityRechargeImmune = AbilityRecharge | Immune,
        ModuleSkipCooldown = 14 << 1,
        ModuleSkipCooldownImmune = ModuleSkipCooldown | Immune,
        AbilitySkipCooldown = 15 << 1,
        AbilitySkipCooldownImmune = AbilitySkipCooldown | Immune,
        DeathMark = 16 << 1,
        DeathMarkImmune = DeathMark | Immune,
        ControlDumper = 17 << 1,
        ControlDumperImmune = ControlDumper | Immune,
        Drain = 18 << 1,
        DrainImmune = Drain | Immune,
        PreLastStand = 19 << 1,
        MoreDmgOnce = 20 << 1,
        PhaseShift = 21 << 1,
        Freeze = 22 << 1,
        FreezeImmune = Freeze | Immune,
        FreezeCharge = 22 << 2,
        IgnoreEnergyShields = 23 << 1,
        MorePenetration = 24 << 1,
        Roamer = 25 << 1 /*50*/,
        RoamerImmune = Roamer | Immune,
        RootCharge = 26 << 1/*52*/,
        SuppressionCharge = 26 << 1 | Immune/*53*/,
        DamageOverTimeCharge = 27 << 1/*54*/,
        AntiStealth = (28 << 1),
        AbilityEnergyShieldOff = (29 << 1),
        Stealth = (30 << 1),
        Berserk = (31 << 1),
        PreBerserk = (32 << 1),
        AntiStealthRadius = (33 << 1),
        HealBinding = (34 << 1),
        CounterShield = 35 << 1,
        GearscoreDamage = 36 << 1,
        EnergyShield = 37 << 1,
        AbsorberShield = 38 << 1,
        HealOnce = 39 << 1,
        // client effects
        Local = 1 << 7,
        Jammer = (1 << 1) | Local,
        Bomb = (2 << 1) | Local,
        ArmorLowered = (3 << 1) | Local,
        DarkZone = (4 << 1) | Local,
    }

    [ProtoContract]
    public struct AccumulatedEffectCharge
    {
        [ProtoMember(1)]
        public EffectKind Kind;

        /// <summary>
        /// Робот на котором накапливается(заряжается) эффект
        /// </summary>
        [ProtoMember(2)]
        public short MechViewId;

        /// <summary>
        /// Процент заряда эффекта в диапозоне значений 0-1
        /// </summary>
        [ProtoMember(3)]
        public float AccumulatedPercent;

        /// <summary>
        /// Надо ли удалить заряд с цели
        /// </summary>
        [ProtoMember(4)]
        public bool IsDropped;

        public AccumulatedEffectCharge(EffectKind kind, short mechViewId, float accumulatedPercent, bool isDropped)
        {
            Kind = kind;
            MechViewId = mechViewId;
            AccumulatedPercent = accumulatedPercent;
            IsDropped = isDropped;
        }
    }

    [ProtoContract]
    public class MechSpawnSuccessMessage
    {
        [ProtoMember(1)]
        public PresetMech SpawnedMech;

        [ProtoMember(2)]
        public sbyte SpawnType;

        [ProtoMember(3)]
        public int PointIndex;

        /// <summary>
        /// Иногда, это сообщение приходит раньше, чем клиент узнает о том, в какой команде находится
        /// игрок заспавнивший меха. Поэтому отправляем номер команды прямо в этом сообщении. Подробности у клиента)
        /// </summary>
        [ProtoMember(4, IsRequired = false)]
        [System.ComponentModel.DefaultValue((sbyte)-1)]
        public sbyte TeamIndex = -1;

        [ProtoMember(5)]
        public MechState State;

        [ProtoMember(6)]
        public PresetDrone SpawnedDrone;

        [ProtoContract]
        public class MechState
        {
            [ProtoMember(1)]
            public MechPartState[] PartStates;
        }

        [ProtoContract]
        public struct MechPartState
        {
            [ProtoMember(1)]
            public byte LegacyPartId;

            [ProtoMember(2)]
            public float MaxHealth;

            [ProtoMember(3)]
            public float ActivationHealth;

            [ProtoMember(4)]
            public float RestoreRate;

            [ProtoMember(5)]
            public PartId PartId;
        }
    }

    [ProtoContract]
    public class PresetMech
    {
        public PresetMech(
            string id,
            int viewId,
            bool isAlive,
            string itemId,
            int level,
            int rank,
            int presetIndex,
            int slotNumber,
            string skinId,
            int spawnsCount,
            IEnumerable<PresetMechEquip> equip,
            IEnumerable<PresetMechEquip> modules,
            PresetPilot pilot,
            MechKind kind,
            BranchWithLevel[] branchesWithLevels)
        {
            Id = id;
            ViewId = viewId;
            IsAlive = isAlive;
            ItemId = itemId;
            Level = level;
            PresetIndex = presetIndex;
            SlotNumber = slotNumber;
            SkinId = skinId;
            Equip = new List<PresetMechEquip>(equip);
            Modules = new List<PresetMechEquip>(modules);
            Pilot = pilot;
            Rank = rank;
            SpawnsCount = spawnsCount;
            MechKind = kind;
            BranchesWithLevels = branchesWithLevels;
        }

        public PresetMech()
        {
        }

        /// <summary>
        /// Определеяет был ли этот робот хотя бы раз использован в бою.
        /// </summary>
        public bool IsSpawned
        {
            get { return ViewId > 0; }
        }

        /// <summary>
        /// Определяет является ли этот мех текущим мехом игрока.
        /// </summary>
        public bool IsCurrent
        {
            get { return IsSpawned && IsAlive; }
        }

        /// <summary>
        /// Идентификатор робота.
        /// </summary>
        [ProtoMember(1)]
        public string Id { get; set; }

        /// <summary>
        /// Идентификатор типа робота. Пример: Destrier_V1
        /// </summary>
        [ProtoMember(2)]
        public string ItemId { get; set; }

        /// <summary>
        /// Уровень робота.
        /// </summary>
        [Obsolete("Перенести в BranchesWithLevels")]
        [ProtoMember(3)]
        public int Level { get; set; }

        /// <summary>
        /// Номер слота в гараже. zero based
        /// </summary>
        [ProtoMember(4)]
        public int SlotNumber { get; set; }

        /// <summary>
        /// Идентификатор скина робота.
        /// </summary>
        [ProtoMember(5)]
        public string SkinId { get; set; }

        /// <summary>
        /// Список вооружения одетого на робота.
        /// </summary>
        [ProtoMember(6)]
        public List<PresetMechEquip> Equip { get; set; }

        /// <summary>
        /// Статус робота: true - доступен, false - уничтожен.
        /// </summary>
        [ProtoMember(7)]
        public bool IsAlive { get; set; }

        /// <summary>
        /// Индекс пресета ангара в котором находится робот. zero based
        /// </summary>
        [ProtoMember(8, IsRequired = false)]
        public int? PresetIndex { get; set; }

        /// <summary>
        /// Уровень апргрейда робота.
        /// </summary>
        [Obsolete("Перенести в BranchesWithLevels")]
        [ProtoMember(9, IsRequired = false)]
        public int Rank { get; set; }

        /// <summary>
        /// Photon ViewID. Если 0, то робот еще ни разу не спавнился.
        /// </summary>
        [ProtoMember(10, IsRequired = false)]
        public int ViewId { get; set; }

        /// <summary>
        /// Сколько раз робот был использован в бою.
        /// </summary>
        [ProtoMember(11, IsRequired = false)]
        public int SpawnsCount { get; set; }

        /// <summary>
        /// Модули на роботе
        /// </summary>
        [ProtoMember(12)]
        public List<PresetMechEquip> Modules { get; set; }

        /// <summary>
        /// Пилот
        /// </summary>
        [ProtoMember(13)]
        public PresetPilot Pilot { get; set; }

        /// <summary>
        /// Mech Kind per se
        /// </summary>
        [ProtoMember(14)]
        public MechKind MechKind { get; set; }

        /// <summary>
        ///  Список веток прокачки и их уровень
        /// </summary>
        [ProtoMember(15)]
        public BranchWithLevel[] BranchesWithLevels { get; set; }

        /// <summary>
        /// Сколько раз робот был воскрешен в бою.
        /// </summary>
        public int ResurrectionsCount
        {
            get
            {
                if (SpawnsCount == 0)
                {
                    return 0;
                }
                return SpawnsCount - 1;
            }
        }
    }

    [ProtoContract]
    public class BranchWithLevel
    {
        /// <summary>
        /// Идентификатор ветки прокачки.
        /// </summary>
        [ProtoMember(1)]
        public string Id { get; set; }

        /// <summary>
        /// Уровень ветки
        /// </summary>
        [ProtoMember(2)]
        public int Level { get; set; }

        /// <summary>
        /// Ранк ветки
        /// </summary>
        [ProtoMember(3)]
        public int Rank { get; set; }
    }

    /// <summary>
    /// Вооружение робота.
    /// </summary>
    [ProtoContract]
    public class PresetMechEquip
    {
        public PresetMechEquip(string id, string itemId, int level, int rank, int slotNumber, int seed)
        {
            Id = id;
            ItemId = itemId;
            Level = level;
            Rank = rank;
            SlotNumber = slotNumber;
            Seed = seed;
        }

        public PresetMechEquip()
        {
        }

        /// <summary>
        /// Идентификатор вооружения.
        /// </summary>
        [ProtoMember(1)]
        public string Id { get; set; }

        /// <summary>
        /// Идентификатор типа вооружения. Пример: AC_Molot_V1, AT_Spiral_V1
        /// </summary>
        [ProtoMember(2)]
        public string ItemId { get; set; }

        /// <summary>
        /// Уровень вооружения.
        /// </summary>
        [ProtoMember(3)]
        public int Level { get; set; }

        /// <summary>
        /// Номер слота на роботе в который установлено вооружение.
        /// </summary>
        [ProtoMember(4)]
        public int SlotNumber { get; set; }

        /// <summary>
        /// Инициализационное значение для генератора случайных чисел. Используется для расчета траекторий.
        /// </summary>
        [ProtoMember(5)]
        public int Seed { get; set; }

        /// <summary>
        /// Уровень апргрейда вооружения.
        /// </summary>
        [ProtoMember(6, IsRequired = false)]
        public int Rank { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class PresetPilot
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public PresetPilotSkill[] Skills { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class PresetPilotSkill
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public byte Level { get; set; }

        [ProtoMember(3)]
        public bool IsActive { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class PresetDrone
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public string TypeId { get; set; }

        [ProtoMember(3)]
        public PresetChip[] Chips { get; set; }

        [ProtoMember(4)]
        public byte Slot { get; set; }

        [ProtoMember(5)]
        public byte SpawnCount { get; set; }

        [ProtoMember(6)]
        public bool Current { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class PresetChip
    {
        [ProtoMember(1)]
        public string TypeId { get; set; }

        [ProtoMember(2)]
        public byte Slot { get; set; }
    }

    public enum MechKind
    {
        Robot = 0,
        Titan = 1
    }
}
