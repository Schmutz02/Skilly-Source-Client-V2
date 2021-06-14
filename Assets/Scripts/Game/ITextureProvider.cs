using UnityEngine;

namespace Game
{
    public interface ITextureProvider
    {
        public Sprite GetTexture(int time);
    }
}