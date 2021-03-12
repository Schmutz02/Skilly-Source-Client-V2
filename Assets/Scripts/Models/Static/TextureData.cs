using System.Xml.Linq;
using UnityEngine;
using Utils;

namespace Models.Static
{
    public class TextureData
    {
        public CharacterAnimation Animation;
        public Sprite Texture;

        public TextureData(XElement xml)
        {
            if (xml.Element("Texture") != null)
            {
                Texture = GetTexture(xml.Element("Texture"));
            }

            if (xml.Element("AnimatedTexture") != null)
            {
                Animation = GetAnimatedTexture(xml.Element("AnimatedTexture"));
            }
        }

        private static Sprite GetTexture(XElement textureXml)
        {
            var sheetName = textureXml.ParseString("File");
            var index = textureXml.ParseUshort("Index");
            return AssetLibrary.GetImage(sheetName, index);
        }

        private static CharacterAnimation GetAnimatedTexture(XElement textureXml)
        {
            var sheetName = textureXml.ParseString("File");
            var index = textureXml.ParseUshort("Index");
            return AssetLibrary.GetAnimation(sheetName, index);
        }
    }
}