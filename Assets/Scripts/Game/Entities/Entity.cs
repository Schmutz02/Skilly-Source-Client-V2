using Models.Static;
using UnityEngine;

namespace Game.Entities
{
    public class Entity : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _renderer;
        
        public void Init(int type)
        {
            var desc = AssetLibrary.GetObjectDesc(type);
            _renderer.sprite = desc.TextureData.Texture ??
                               desc.TextureData.Animation.GetFrame(Direction.Down, Action.Stand, 0);
        }
    }
}