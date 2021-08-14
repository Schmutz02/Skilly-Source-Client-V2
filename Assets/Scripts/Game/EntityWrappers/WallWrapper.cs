using Game.Entities;
using UnityEngine;

namespace Game.EntityWrappers
{
    //TODO maybe make a static object??
    public class WallWrapper : EntityWrapper
    {
        [SerializeField]
        private SpriteRenderer[] _sides;

        public override void Init(Entity portal, bool rotating = true)
        {
            base.Init(portal, false);

            Renderer.sprite = portal.Desc.TopTextureData.Texture;
            foreach (var side in _sides)
            {
                side.sprite = portal.Desc.TextureData.Texture;
            }
        }

        protected override void Update()
        {
            transform.position = Entity.Position;
        }
    }
}