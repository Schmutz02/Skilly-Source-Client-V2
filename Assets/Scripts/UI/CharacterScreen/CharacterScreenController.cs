using Models;
using Models.Static;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterScreen
{
    public class CharacterScreenController : UIController
    {
        [SerializeField]
        private RectTransform _characterGroup;

        [SerializeField]
        private GameObject _characterRectPrefab;
        
        protected override void Reset()
        {
            Camera.main.backgroundColor = BlueColor;
            
            var i = 0;
            foreach (Transform child in _characterGroup.transform)
            {
                if (i < Account.Characters.Count)
                {
                    var desc = AssetLibrary.GetObjectDesc(Account.Characters[i].ClassType);
                    child.GetComponent<Image>().sprite =
                        desc.TextureData.Animation.GetFrame(Direction.Right, Action.Stand, 0);
                    i++;
                    continue;
                }
                
                child.gameObject.SetActive(false);
            }
            
            while (i < Account.Characters.Count)
            {
                var desc = AssetLibrary.GetObjectDesc(Account.Characters[i].ClassType);
                var characterImage = Instantiate(_characterRectPrefab, _characterGroup).GetComponent<Image>();
                characterImage.sprite = desc.TextureData.Animation.GetFrame(Direction.Right, Action.Stand, 0);
                i++;
            }
        }
    }
}