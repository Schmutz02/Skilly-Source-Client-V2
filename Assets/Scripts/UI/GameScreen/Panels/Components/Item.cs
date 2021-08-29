using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.GameScreen.Panels.Components
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private TextMeshProUGUI _dosesText;
        
        public int ItemType { get; private set; }
        public int ItemData { get; private set; }
        
        public void SetType(int itemType, int itemData)
        {
            ItemType = itemType;
            ItemData = itemData;

            if (itemType == (int) Models.Static.ItemType.None)
            {
                gameObject.SetActive(false);
                return;
            }

            var desc = AssetLibrary.GetItemDesc(itemType);
            var sprite = desc.TextureData.GetTexture();
            _image.sprite = SpriteUtils.Redraw(sprite, 80);

            if (desc.Doses > 0)
            {
                _dosesText.text = desc.Doses.ToString();
            }
            
            gameObject.SetActive(true);
        }
    }
}