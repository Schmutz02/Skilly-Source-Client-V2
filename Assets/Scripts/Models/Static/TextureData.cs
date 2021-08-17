using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using Utils;

namespace Models.Static
{
    public class TextureData
    {
        public CharacterAnimation Animation { get; private set; }
        public Sprite Texture { get; private set; }
        public TextureData[] RandomTextureData { get; private set; }
        public Dictionary<int, TextureData> AltTextures { get; private set; }

        public TextureData(XElement xml)
        {
            if (xml.Element("Texture") != null)
            {
                Parse(xml.Element("Texture"));
            }
            else if (xml.Element("AnimatedTexture") != null)
            {
                Parse(xml.Element("AnimatedTexture"));
            }
            else if (xml.Element("RandomTexture") != null)
            {
                Parse(xml.Element("RandomTexture"));
            }
            else if (xml.Element("AltTexture") != null)
            {
                Parse(xml);
            }
            else
            {
                Parse(xml);
            }
        }

        public Sprite GetTexture(int id = 0)
        {
            if (RandomTextureData == null)
                return Texture;

            var textureData = RandomTextureData[id % RandomTextureData.Length];
            return textureData.GetTexture(id);
        }

        public TextureData GetAltTextureData(int id)
        {
            return AltTextures?[id];
        }

        private void Parse(XElement textureXml)
        {
            switch (textureXml.Name.ToString())
            {
                case "Texture":
                    Texture = GetTexture(textureXml);
                    break;
                case "AnimatedTexture":
                    Animation = GetAnimatedTexture(textureXml);
                    Texture = Animation.ImageFromAngle(0, Action.Stand, 0);
                    break;
                case "RandomTexture":
                    RandomTextureData = GetRandomTexture(textureXml);
                    break;
                case "AltTexture":
                    AltTextures = GetAltTextures(textureXml);
                    break;
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

        private static TextureData[] GetRandomTexture(XElement textureXml)
        {
            var textureData = new List<TextureData>();
            foreach (var child in textureXml.Elements())
            {
                textureData.Add(new TextureData(child));
            }

            return textureData.ToArray();
        }

        private static Dictionary<int, TextureData> GetAltTextures(XElement objectXml)
        {
            var altTextures = new Dictionary<int, TextureData>();
            foreach (var textureXml in objectXml.Elements("AltTexture"))
            {
                altTextures[textureXml.ParseInt("@id")] = new TextureData(textureXml);
            }

            return altTextures;
        }
    }
}