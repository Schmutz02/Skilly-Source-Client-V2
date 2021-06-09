using Game.Entities;
using Models.Static;
using UnityEngine.Tilemaps;

namespace Game
{
    public class Square : Tile
    {
        public TileDesc Desc { get; private set; }
        public ushort Type { get; private set; }
        public Entity StaticObject;

        public void Init(TileDesc desc)
        {
            Desc = desc;
            Type = desc.Type;

            sprite = desc.TextureData.Texture;
        }
    }
}