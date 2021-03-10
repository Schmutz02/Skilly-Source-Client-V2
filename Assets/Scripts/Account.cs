using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using UnityEngine;

public static class Account
{
    public const string USERNAME_KEY = "username";
    public static bool Exists => !string.IsNullOrEmpty(Username);
    
    public static string Username { get; private set; }
    public static int MaxCharacters { get; private set; }
    public static GuildInfo Guild { get; private set; }
    public static int CurrentFame { get; private set; }
    public static int CurrentGold { get; private set; }

    private static readonly Dictionary<int, ClassStats> _ClassStats =
        new Dictionary<int, ClassStats>();

    public static readonly ReadOnlyDictionary<int, ClassStats> ClassStats =
        new ReadOnlyDictionary<int, ClassStats>(_ClassStats);

    private static readonly List<CharacterStats> _Characters = new List<CharacterStats>();

    public static readonly ReadOnlyCollection<CharacterStats> Characters =
        new ReadOnlyCollection<CharacterStats>(_Characters);

    public static void Login(string username, bool rememberUsername)
    {
        Reset();
        Username = username;

        if (rememberUsername)
        {
            PlayerPrefs.SetString(USERNAME_KEY, username);
        }
    }

    private static void Reset()
    {
        Username = null;
        MaxCharacters = 1;
        Guild = GuildInfo.None;
        CurrentFame = 0;
        CurrentGold = 0;
        _ClassStats.Clear();
        _Characters.Clear();
        PlayerPrefs.DeleteKey(USERNAME_KEY);
    }

    public static void LoadFromCharList(XElement xml)
    {
        MaxCharacters = xml.ParseInt("@maxNumChars");
        ParseAccountXml(xml.Element("Account"));

        foreach (var charXml in xml.Elements("Char"))
        {
            _Characters.Add(new CharacterStats(charXml));
        }
    }

    private static void ParseAccountXml(XElement xml)
    {
        Username = xml.ParseString("Name");
        Guild = new GuildInfo(xml.Element("Guild"));
        ParseStatsXml(xml.Element("Stats"));
    }

    private static void ParseStatsXml(XElement xml)
    {
        CurrentFame = xml.ParseInt("Fame");
        CurrentGold = xml.ParseInt("Credits");

        foreach (var classStatXml in xml.Elements("ClassStats"))
        {
            var classType = classStatXml.ParseInt("@objectType");
            _ClassStats[classType] = new ClassStats(classStatXml);
        }
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
        ClassType = xml.ParseInt("ClassType");
        Level = xml.ParseInt("Level");
        Experience = xml.ParseInt("Experience");
        HP = xml.ParseInt("HP");
        MP = xml.ParseInt("MP");
        Stats = xml.ParseIntArray("Stats", ",");
        Inventory = xml.ParseIntArray("Equipment", ",");
        ItemDatas = xml.ParseIntArray("ItemDatas", ",");
        Fame = xml.ParseInt("Fame");
        Tex1 = xml.ParseInt("Tex1");
        Tex2 = xml.ParseInt("Tex2");
        SkinType = xml.ParseInt("SkinType");
    }
}