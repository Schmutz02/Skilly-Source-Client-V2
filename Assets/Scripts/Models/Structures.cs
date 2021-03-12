using System.Xml.Linq;
using Utils;

namespace Models
{
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
        Rank = (GuildRank)xml.ParseInt("Rank");
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
}