using Models.Static;
using UnityEngine;

namespace Game.Entities
{
    public class Wall : Entity
    {
        [SerializeField]
        private SpriteRenderer[] _sides;

        protected override void Init(ObjectDesc desc, int objectId, bool isMyPlayer, Map map, bool rotating = true)
        {
            base.Init(desc, objectId, false, map, false);
            
            Renderer.sprite = Desc.TopTextureData.GetTexture(ObjectId);
            foreach (var side in _sides)
            {
                side.sprite = Desc.TextureData.GetTexture(ObjectId);
            }
        }

        public override bool Tick()
        {
            return true;
        }

        public override void Draw()
        {
            
        }
    }
}