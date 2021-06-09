using System;
using System.Xml.Linq;
using Utils;

namespace Models.Static
{
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

        public readonly TextureData TextureData;

        public readonly string HitSound;
        public readonly string DeathSound;

        public readonly float BloodChance;

        public ObjectDesc(XElement xml)
        {
            Id = xml.ParseString("@id");
            Type = xml.ParseUshort("@type");

            DisplayId = xml.ParseString("DisplayId", Id);

            Static = xml.ParseBool("Static");
            Class = xml.ParseString("Class");
            
            OccupySquare = xml.ParseBool("OccupySquare");
            FullOccupy = xml.ParseBool("FullOccupy");
            EnemyOccupySquare = xml.ParseBool("EnemyOccupySquare");

            TextureData = new TextureData(xml);

            HitSound = xml.ParseString("HitSound");
            DeathSound = xml.ParseString("DeathSound");

            BloodChance = xml.ParseFloat("BloodProb");
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

        public TileDesc(XElement e)
        {
            Id = e.ParseString("@id");
            Type = e.ParseUshort("@type");
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
}