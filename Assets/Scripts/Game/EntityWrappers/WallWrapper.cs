using Game.Entities;
using UnityEngine;

namespace Game.EntityWrappers
{
    //TODO maybe make a static object??
    public class WallWrapper : EntityWrapper
    {
        [SerializeField]
        private SpriteRenderer[] _sides;

        public override void Init(Entity entity, bool rotating = true)
        {
            base.Init(entity, false);

            Renderer.sprite = entity.Desc.TopTextureData.GetTexture(entity.ObjectId);
            foreach (var side in _sides)
            {
                side.sprite = entity.Desc.TextureData.Texture;
            }
            
            transform.position = entity.Position;
        }

        public override bool Tick()
        {
            return true;
        }
    }
}