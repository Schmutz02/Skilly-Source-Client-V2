using System.Collections.Generic;
using System.Xml.Linq;
using Models.Static;
using Networking;
using UnityEngine;
using Utils;

namespace Models
{
    public readonly struct GameInitData
    {
        public readonly int WorldId;
        public readonly int CharId;
        public readonly bool NewCharacter;
        public readonly int ClassType;
        public readonly int SkinType;

        public GameInitData(int worldId, int charId, bool newCharacter, int classType, int skinType)
        {
            WorldId = worldId;
            CharId = charId;
            NewCharacter = newCharacter;
            ClassType = classType;
            SkinType = skinType;
        }
    }
    
    public readonly struct GuildInfo
    {
        public static GuildInfo None = new GuildInfo(null, GuildRank.Initiate);

        public readonly string Name;
        public readonly GuildRank Rank;

        public GuildInfo(string name, GuildRank rank)
        {
            Name = name;
            Rank = rank;
        }

        public GuildInfo(XElement xml)
        {
            Name = xml.ParseString("Name");
            Rank = (GuildRank) xml.ParseInt("Rank");
        }
    }

    public enum GuildRank
    {
        Initiate = 0,
        Member = 10,
        Officer = 20,
        Leader = 30,
        Founder = 40
    }

    public readonly struct ClassStats
    {
        public readonly int ClassType;
        public readonly int BestLevel;
        public readonly int BestFame;

        public ClassStats(int classType, int bestLevel, int bestFame)
        {
            ClassType = classType;
            BestLevel = bestLevel;
            BestFame = bestFame;
        }

        public ClassStats(XElement xml)
        {
            ClassType = xml.ParseInt("@objectType");
            BestLevel = xml.ParseInt("BestLevel");
            BestFame = xml.ParseInt("BestFame");
        }
    }

    public readonly struct CharacterStats
    {
        public readonly int Id;
        public readonly int ClassType;
        public readonly int Level;
        public readonly int Experience;
        public readonly int Fame;
        public readonly int[] Stats;
        public readonly int[] Inventory;
        public readonly int[] ItemDatas;
        public readonly int HP;
        public readonly int MP;
        public readonly int Tex1;
        public readonly int Tex2;
        public readonly int SkinType;

        public CharacterStats(XElement xml)
        {
            Id = xml.ParseInt("@id");
            ClassType = xml.ParseInt("ObjectType");
            Level = xml.ParseInt("Level");
            Experience = xml.ParseInt("Exp");
            HP = xml.ParseInt("HitPoints");
            MP = xml.ParseInt("MagicPoints");
            Stats = xml.ParseIntArray("Stats", ",");
            Inventory = xml.ParseIntArray("Equipment", ",");
            ItemDatas = xml.ParseIntArray("ItemDatas", ",");
            Fame = xml.ParseInt("CurrentFame");
            Tex1 = xml.ParseInt("Tex1");
            Tex2 = xml.ParseInt("Tex2");
            SkinType = xml.ParseInt("SkinType");
        }
    }

    public struct TileData
    {
        public ushort TileType;
        public short X;
        public short Y;

        public TileData(PacketReader rdr)
        {
            X = rdr.ReadInt16();
            Y = rdr.ReadInt16();
            TileType = rdr.ReadUInt16();
        }
    }

    public struct ObjectDrop
    {
        public int Id;
        public bool Explode;

        public ObjectDrop(PacketReader rdr)
        {
            Id = rdr.ReadInt32();
            Explode = rdr.ReadBoolean();
        }
    }

    public struct ObjectDefinition
    {
        public ushort ObjectType;
        public ObjectStatus ObjectStatus;

        public ObjectDefinition(PacketReader rdr)
        {
            ObjectType = rdr.ReadUInt16();
            ObjectStatus = new ObjectStatus(rdr);
        }
    }

    public struct ObjectStatus
    {
        public int Id;
        public Vector3 Position;
        public Dictionary<StatType, object> Stats;

        public ObjectStatus(PacketReader rdr)
        {
            Id = rdr.ReadInt32();
            Position = new Vector3()
            {
                x = rdr.ReadSingle(),
                y = rdr.ReadSingle(),
                z = 0
            };

            var statsCount = rdr.ReadByte();
            Stats = new Dictionary<StatType, object>();
            for (var i = 0; i < statsCount; i++)
            {
                var key = (StatType)rdr.ReadByte();
                if (IsStringStat(key))
                {
                    Stats[key] = rdr.ReadString();
                }
                else
                {
                    Stats[key] = rdr.ReadInt32();
                }
            }
        }

        public static bool IsStringStat(StatType stat)
        {
            switch (stat)
            {
                case StatType.Name:
                case StatType.GuildName:
                    return true;
            }

            return false;
        }
    }
}