using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    public class MiniMapChunk : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private RectTransform _rectTransform;

        public Sprite Sprite
        {
            get => _image.sprite;
            set => _image.sprite = value;
        }

        public RectTransform RectTransform => _rectTransform;
    }
}