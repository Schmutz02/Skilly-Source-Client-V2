using Game.Entities;
using UnityEngine;

namespace Game.EntityWrappers
{
    //TODO maybe make a static object??
    public class WallWrapper : EntityWrapper
    {
        [SerializeField]
        private SpriteRenderer[] _sides;

        public override void Init(Entity child, bool rotating)
        {
            base.Init(child, false);

            Renderer.sprite = child.Desc.TopTextureData.Texture;
            foreach (var side in _sides)
            {
                side.sprite = child.Desc.TextureData.Texture;
            }
        }
    }
}