using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using UnityEngine;
using Utils;

namespace Models
{
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
}