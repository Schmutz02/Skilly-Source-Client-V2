using System.Xml.Linq;
using Utils;

namespace Models.Static
{
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

        public readonly string Class;

        public readonly TextureData TextureData;

        public readonly string HitSound;
        public readonly string DeathSound;

        public readonly float BloodChance;

        public ObjectDesc(XElement xml)
        {
            Id = xml.ParseString("@id");
            Type = xml.ParseUshort("@type");

            DisplayId = xml.ParseString("DisplayId", Id);

            Class = xml.ParseString("Class");

            TextureData = new TextureData(xml);

            HitSound = xml.ParseString("HitSound");
            DeathSound = xml.ParseString("DeathSound");

            BloodChance = xml.ParseFloat("BloodProb");
        }
    }
}