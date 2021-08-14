using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Utils;

namespace Models.Static
{
    [Flags]
    public enum ItemData : ulong
    {
        //Tiers
        T0 = 1 << 0,
        T1 = 1 << 1,
        T2 = 1 << 2,
        T3 = 1 << 3,
        T4 = 1 << 4,
        T5 = 1 << 5,
        T6 = 1 << 6,
        T7 = 1 << 7,

        //Bonuses
        MaxHP = 1 << 8,
        MaxMP = 1 << 9,
        Attack = 1 << 10,
        Defense = 1 << 11,
        Speed = 1 << 12,
        Dexterity = 1 << 13,
        Vitality = 1 << 14,
        Wisdom = 1 << 15,
        RateOfFire = 1 << 16,
        Damage = 1 << 17,
        Cooldown = 1 << 18,
        FameBonus = 1 << 19
    }
    
    public enum StatType
    {
        MaxHp,
        Hp,
        Size,
        MaxMp,
        Mp,
        NextLevelExp,
        Exp,
        Level,
        Inventory0,
        Inventory1,
        Inventory2,
        Inventory3,
        Inventory4,
        Inventory5,
        Inventory6,
        Inventory7,
        Inventory8,
        Inventory9,
        Inventory10,
        Inventory11,
        Attack,
        Defense,
        Speed,
        Vitality,
        Wisdom,
        Dexterity,
        Condition,
        NumStars,
        Name,
        DyeLarge,
        DyeSmall,
        MerchandiseType,
        MerchandisePrice,
        Credits,
        Active,
        AccountId,
        Fame,
        MerchandiseCurrency,
        Connect,
        MerchandiseCount,
        MerchandiseMinsLeft,
        MerchandiseDiscount,
        MerchandiseRankReq,
        MaxHpBoost,
        MaxMpBoost,
        AttackBoost,
        DefenseBoost,
        SpeedBoost,
        VitalityBoost,
        WisdomBoost,
        DexterityBoost,
        CharFame,
        NextClassQuestFame,
        LegendaryRank,
        SinkLevel,
        AltTexture,
        GuildName,
        GuildRank,
        Oxygen,
        HealthPotionStack,
        MagicPotionStack,
        Backpack0,
        Backpack1,
        Backpack2,
        Backpack3,
        Backpack4,
        Backpack5,
        Backpack6,
        Backpack7,
        HasBackpack,
        Texture,
        ItemData0,
        ItemData1,
        ItemData2,
        ItemData3,
        ItemData4,
        ItemData5,
        ItemData6,
        ItemData7,
        ItemData8,
        ItemData9,
        ItemData10,
        ItemData11,
        ItemData12,
        ItemData13,
        ItemData14,
        ItemData15,
        ItemData16,
        ItemData17,
        ItemData18,
        ItemData19,
        OwnerAccountId
    }
    
    [Flags]
    public enum ConditionEffect : ulong
    {
        Nothing = 1 << 0,
        Quiet = 1 << 1,
        Weak = 1 << 2,
        Slowed = 1 << 3,
        Sick = 1 << 4,
        Dazed = 1 << 5,
        Stunned = 1 << 6,
        Blind = 1 << 7,
        Hallucinating = 1 << 8,
        Drunk = 1 << 9,
        Confused = 1 << 10,
        StunImmune = 1 << 11,
        Invisible = 1 << 12,
        Paralyzed = 1 << 13,
        Speedy = 1 << 14,
        Bleeding = 1 << 15,
        Healing = 1 << 16,
        Damaging = 1 << 17,
        Berserk = 1 << 18,
        Stasis = 1 << 19,
        StasisImmune = 1 << 20,
        Invincible = 1 << 21,
        Invulnerable = 1 << 23,
        Armored = 1 << 24,
        ArmorBroken = 1 << 25,
        Hexed = 1 << 26,
        NinjaSpeedy = 1 << 27,
    }
    
    public enum ActivateEffectIndex
    {
        Create,
        Dye,
        Shoot,
        IncrementStat,
        Heal,
        Magic,
        HealNova,
        StatBoostSelf,
        StatBoostAura,
        BulletNova,
        ConditionEffectSelf,
        ConditionEffectAura,
        Teleport,
        PoisonGrenade,
        VampireBlast,
        Trap,
        StasisBlast,
        Pet,
        Decoy,
        Lightning,
        UnlockPortal,
        MagicNova,
        ClearConditionEffectAura,
        RemoveNegativeConditions,
        ClearConditionEffectSelf,
        ClearConditionsEffectSelf,
        RemoveNegativeConditionsSelf,
        Shuriken,
        DazeBlast,
        Backpack,
        PermaPet
    }
    
    public enum ItemType : byte
    {
        All,
        Sword,
        Dagger,
        Bow,
        Tome,
        Shield,
        Leather,
        Plate,
        Wand,
        Ring,
        Potion,
        Spell,
        Seal,
        Cloak,
        Robe,
        Quiver,
        Helm,
        Staff,
        Poison,
        Skull,
        Trap,
        Orb,
        Prism,
        Scepter,
        Katana,
        Shuriken,
    }
    
    public class ObjectDesc
    {
        public readonly string Id;
        public readonly ushort Type;

        public readonly string DisplayId;

        public readonly bool Static;
        public readonly string Class;
        
        public readonly bool OccupySquare;
        public readonly bool FullOccupy;
        public readonly bool EnemyOccupySquare;
        
        public readonly bool Player;
        public readonly bool Enemy;

        public readonly int Size;
        public readonly int ShadowSize;
        public readonly Color ShadowColor;
        public readonly WhileMovingDesc WhileMoving;
        public readonly bool Flying;
        public readonly float Z;

        public readonly string Model;

        public readonly TextureData TextureData;
        public readonly TextureData TopTextureData;
        public readonly TextureData Portrait;

        public readonly bool DontFaceAttacks;

        public readonly string HitSound;
        public readonly string DeathSound;

        public readonly float BloodChance;
        
        public readonly Dictionary<int, ProjectileDesc> Projectiles;

        public readonly float AngleCorrection;
        public readonly float Rotation;

        public readonly bool NexusPortal;
        public readonly bool LockedPortal;

        public ObjectDesc(XElement xml, string id, ushort type)
        {
            Id = id;
            Type = type;
            
            DisplayId = xml.ParseString("DisplayId", Id);

            Static = xml.ParseBool("Static");
            Class = xml.ParseString("Class");
            
            OccupySquare = xml.ParseBool("OccupySquare");
            FullOccupy = xml.ParseBool("FullOccupy");
            EnemyOccupySquare = xml.ParseBool("EnemyOccupySquare");
            
            Enemy = xml.ParseBool("Enemy");
            Player = xml.ParseBool("Player");

            Size = xml.ParseInt("Size", 100);
            ShadowSize = xml.ParseInt("ShadowSize", 100);
            // ShadowColor = xml.ParseColor("ShadowColor");
            ShadowColor = Color.black;
            if (xml.Element("WhileMoving") != null)
                WhileMoving = new WhileMovingDesc(xml.Element("WhileMoving"));
            Flying = xml.ParseBool("Flying");
            Z = xml.ParseFloat("Z");

            Model = xml.ParseString("Model");

            TextureData = new TextureData(xml);
            if (xml.Element("Top") != null)
                TopTextureData = new TextureData(xml.Element("Top"));
            if (xml.Element("Portrait") != null)
                Portrait = new TextureData(xml.Element("Portrait"));

            DontFaceAttacks = xml.ParseBool("DontFaceAttacks");

            HitSound = xml.ParseString("HitSound");
            DeathSound = xml.ParseString("DeathSound");

            BloodChance = xml.ParseFloat("BloodProb");
            
            Projectiles = new Dictionary<int, ProjectileDesc>();
            foreach (var k in xml.Elements("Projectile"))
            {
                var desc = new ProjectileDesc(k, Type);
                Projectiles[desc.BulletType] = desc;
            }

            AngleCorrection = xml.ParseInt("AngleCorrection") * -Mathf.PI / 4;
            Rotation = xml.ParseFloat("Rotation");

            NexusPortal = xml.ParseBool("NexusPortal");
            LockedPortal = xml.ParseBool("LockedPortal");
        }
    }

    public class WhileMovingDesc
    {
        public readonly float Z;
        public readonly bool Flying;

        public WhileMovingDesc(XElement xml)
        {
            Z = xml.ParseFloat("Z");
            Flying = xml.ParseBool("Flying");
        }
    }
    
    public class PlayerDesc : ObjectDesc
    {
        public readonly ItemType[] SlotTypes;
        public readonly int[] Equipment;
        public readonly int[] ItemDatas;
        public readonly StatDesc[] Stats;
        public readonly int[] StartingValues;

        public PlayerDesc(XElement e, string id, ushort type) : base(e, id, type)
        {
            SlotTypes = e.ParseIntArray("SlotTypes", ",").Select(x => (ItemType)x).ToArray();

            var equipment = e.ParseUshortArray("Equipment", ",").Select(k => k == 0xffff ? -1 : k).ToArray();
            Equipment = new int[20];
            for (var k = 0; k < 20; k++)
                Equipment[k] = k >= equipment.Length ? -1 : equipment[k];

            ItemDatas = new int[20];
            for (var k = 0; k < 20; k++)
                ItemDatas[k] = -1;

            Stats = new StatDesc[8];
            for (var i = 0; i < Stats.Length; i++)
                Stats[i] = new StatDesc(i, e);
            Stats = Stats.OrderBy(k => k.Index).ToArray();

            StartingValues = Stats.Select(k => k.StartingValue).ToArray();
        }
    }
    
    public class StatDesc
    {
        public readonly string Type;
        public readonly int Index;
        public readonly int MaxValue;
        public readonly int StartingValue;
        public readonly int MinIncrease;
        public readonly int MaxIncrease;

        public StatDesc(int index, XElement e)
        {
            Index = index;
            Type = StatIndexToName(index);

            StartingValue = e.ParseInt(Type);
            MaxValue = e.Element(Type).ParseInt("@max");

            foreach (var stat in e.Elements("LevelIncrease"))
            {
                if (stat.Value == Type)
                {
                    MinIncrease = stat.ParseInt("@min");
                    MaxIncrease = stat.ParseInt("@max");
                    break;
                }
            }
        }

        public static string StatIndexToName(int index)
        {
            switch (index)
            {
                case 0: return "MaxHitPoints";
                case 1: return "MaxMagicPoints";
                case 2: return "Attack";
                case 3: return "Defense";
                case 4: return "Speed";
                case 5: return "Dexterity";
                case 6: return "HpRegen";
                case 7: return "MpRegen";
            }
            return null;
        }

        public static int StatNameToIndex(string name)
        {
            switch (name)
            {
                case "MaxHitPoints": return 0;
                case "MaxMagicPoints": return 1;
                case "Attack": return 2;
                case "Defense": return 3;
                case "Speed": return 4;
                case "Dexterity": return 5;
                case "HpRegen": return 6;
                case "MpRegen": return 7;
            }
            return -1;
        }
    }
    
    public class TileDesc
    {
        public readonly string Id;
        public readonly ushort Type;
        public readonly TextureData TextureData;
        public readonly bool NoWalk;
        public readonly int Damage;
        public readonly float Speed;
        public readonly bool Sinking;
        public readonly bool Push;
        public readonly float DX;
        public readonly float DY;

        public TileDesc(XElement e, string id, ushort type)
        {
            Id = id;
            Type = type;
            TextureData = new TextureData(e);
            NoWalk = e.ParseBool("NoWalk");
            Damage = e.ParseInt("Damage");
            Speed = e.ParseFloat("Speed", 1.0f);
            Sinking = e.ParseBool("Sinking");
            if (Push = e.ParseBool("Push"))
            {
                DX = e.Element("Animate").ParseFloat("@dx") / 1000f;
                DY = e.Element("Animate").ParseFloat("@dy") / 1000f;
            }
        }
    }
    
    public class ActivateEffectDesc
    {
        public readonly ActivateEffectIndex Index;
        public readonly ConditionEffectDesc[] Effects;
        public readonly ConditionEffect Effect;
        public readonly string Id;
        public readonly int DurationMS;
        public readonly float Range;
        public readonly int Amount;
        public readonly int TotalDamage;
        public readonly float Radius;
        public readonly uint? Color;
        public readonly int MaxTargets;
        public readonly int Stat;
        
        public ActivateEffectDesc(XElement e)
        {
            Index = (ActivateEffectIndex)Enum.Parse(typeof(ActivateEffectIndex), e.Value.Replace(" ", ""));
            Id = e.ParseString("@id");
            Effect = e.ParseConditionEffect("@effect");
            DurationMS = (int)(e.ParseFloat("@duration") * 1000);
            Range = e.ParseFloat("@range");
            Amount = e.ParseInt("@amount");
            TotalDamage = e.ParseInt("@totalDamage");
            Radius = e.ParseFloat("@radius");
            MaxTargets = e.ParseInt("@maxTargets");
            Stat = e.ParseInt("@stat", -1);

            Effects = new ConditionEffectDesc[1]
            {
                new ConditionEffectDesc(Effect, DurationMS)
            };

            if (e.Attribute("color") != null)
                Color = e.ParseUInt("@color");
        }
    }

    public class ItemDesc
    {
        public const float RATE_OF_FIRE_MULTIPLIER = 0.05f;
        public const float DAMAGE_MULTIPLIER = 0.05f;
        public const float COOLDOWN_MULTIPLIER = 0.05f;
        
        public static float GetStat(int data, ItemData i, float multiplier)
        {
            var rank = GetRank(data);
            if (rank == -1)
                return 0;
            var value = 0;
            if (HasStat(data, i))
            {
                value += rank;
            }
            return value * multiplier;
        }

        public static int GetRank(int data)
        {
            if (data == -1)
                return -1;
            if (HasStat(data, ItemData.T0))
                return 1;
            if (HasStat(data, ItemData.T1))
                return 2;
            if (HasStat(data, ItemData.T2))
                return 3;
            if (HasStat(data, ItemData.T3))
                return 4;
            if (HasStat(data, ItemData.T4))
                return 5;
            if (HasStat(data, ItemData.T5))
                return 6;
            if (HasStat(data, ItemData.T6))
                return 7;
            if (HasStat(data, ItemData.T7))
                return 8;
            return -1;
        }

        public static bool HasStat(int data, ItemData i)
        {
            if (data == -1)
                return false;
            return ((ItemData)data & i) != 0;
        }
        
        public readonly string Id;
        public readonly ushort Type;

        public readonly ItemType SlotType;
        public readonly int Tier;
        public readonly string Description;
        public readonly float RateOfFire;
        public readonly bool Usable;
        public readonly int BagType;
        public readonly int MpCost;
        public readonly int FameBonus;
        public readonly int NumProjectiles;
        public readonly float ArcGap;
        public readonly bool Consumable;
        public readonly bool Potion;
        public readonly string DisplayId;
        public readonly string SuccessorId;
        public readonly bool Soulbound;
        public readonly int CooldownMS;
        public readonly bool Resurrects;
        public readonly int Tex1;
        public readonly int Tex2;
        public readonly int Doses;

        public readonly KeyValuePair<int, int>[] StatBoosts;
        public readonly ActivateEffectDesc[] ActivateEffects;
        public readonly ProjectileDesc Projectile;
        public readonly TextureData TextureData;

        public ItemDesc(XElement e, string id, ushort type)
        {
            Id = id;
            Type = type;

            SlotType = (ItemType)e.ParseInt("SlotType");
            Tier = e.ParseInt("Tier", -1);
            Description = e.ParseString("Description");
            RateOfFire = e.ParseFloat("RateOfFire", 1);
            Usable = e.ParseBool("Usable");
            BagType = e.ParseInt("BagType");
            MpCost = e.ParseInt("MpCost");
            FameBonus = e.ParseInt("FameBonus");
            NumProjectiles = e.ParseInt("NumProjectiles", 1);
            ArcGap = e.ParseFloat("ArcGap", 11.25f);
            Consumable = e.ParseBool("Consumable");
            Potion = e.ParseBool("Potion");
            DisplayId = e.ParseString("DisplayId", Id);
            Doses = e.ParseInt("Doses");
            SuccessorId = e.ParseString("SuccessorId");
            Soulbound = e.ParseBool("Soulbound");
            CooldownMS = (int)(e.ParseFloat("Cooldown", .2f) * 1000);
            Resurrects = e.ParseBool("Resurrects");
            Tex1 = (int)e.ParseUInt("Tex1");
            Tex2 = (int)e.ParseUInt("Tex2");

            var stats = new List<KeyValuePair<int, int>>();
            foreach (var s in e.Elements("ActivateOnEquip"))
                stats.Add(new KeyValuePair<int, int>(
                    s.ParseInt("@stat"),
                    s.ParseInt("@amount")));
            StatBoosts = stats.ToArray();

            var activate = new List<ActivateEffectDesc>();
            foreach (var i in e.Elements("Activate"))
                activate.Add(new ActivateEffectDesc(i));
            ActivateEffects = activate.ToArray();

            if (e.Element("Projectile") != null)
                Projectile = new ProjectileDesc(e.Element("Projectile"), Type);
            
            TextureData = new TextureData(e);
        }
    }
    
    public class ProjectileDesc
    {
        public readonly byte BulletType;
        public readonly string ObjectId;
        public readonly int LifetimeMS;
        public readonly float Speed;
        public readonly int Size;

        public readonly int Damage;
        public readonly int MinDamage; //Only for players
        public readonly int MaxDamage;

        public readonly ConditionEffectDesc[] Effects;

        public readonly bool MultiHit;
        public readonly bool PassesCover;
        public readonly bool ArmorPiercing;
        public readonly bool Wavy;
        public readonly bool Parametric;
        public readonly bool Boomerang;

        public readonly float Amplitude;
        public readonly float Frequency;
        public readonly float Magnitude;

        public readonly bool Accelerate;
        public readonly bool Decelerate;

        public readonly ushort ContainerType;

        public ProjectileDesc(XElement e, ushort containerType)
        {
            ContainerType = containerType;
            BulletType = (byte)e.ParseInt("@id");
            ObjectId = e.ParseString("ObjectId");
            LifetimeMS = e.ParseInt("LifetimeMS");
            Speed = e.ParseFloat("Speed");
            Size = e.ParseInt("Size", -1);
            
            Damage = e.ParseInt("Damage");
            MinDamage = e.ParseInt("MinDamage", Damage);
            MaxDamage = e.ParseInt("MaxDamage", Damage);

            var effects = new List<ConditionEffectDesc>();
            foreach (var k in e.Elements("ConditionEffect"))
                effects.Add(new ConditionEffectDesc(k));
            Effects = effects.ToArray();

            MultiHit = e.ParseBool("MultiHit");
            PassesCover = e.ParseBool("PassesCover");
            ArmorPiercing = e.ParseBool("ArmorPiercing");
            Wavy = e.ParseBool("Wavy");
            Parametric = e.ParseBool("Parametric");
            Boomerang = e.ParseBool("Boomerang");

            Amplitude = e.ParseFloat("Amplitude");
            Frequency = e.ParseFloat("Frequency", 1);
            Magnitude = e.ParseFloat("Magnitude", 3);

            Accelerate = e.ParseBool("Accelerate");
            Decelerate = e.ParseBool("Decelerate");
        }
    }

    public class ConditionEffectDesc
    {
        public readonly ConditionEffect Effect;
        public readonly int DurationMS;

        public ConditionEffectDesc(ConditionEffect effect, int durationMs)
        {
            Effect = effect;
            DurationMS = durationMs;
        }

        public ConditionEffectDesc(XElement e)
        {
            Effect = (ConditionEffect)Enum.Parse(typeof(ConditionEffect), e.Value.Replace(" ", ""));
            DurationMS = (int)(e.ParseFloat("@duration") * 1000);
        }
    }
}