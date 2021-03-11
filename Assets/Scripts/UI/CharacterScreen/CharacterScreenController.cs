using Models;
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
            var i = 0;
            foreach (Transform child in _characterGroup.transform)
            {
                if (i < Account.Characters.Count)
                {
                    child.GetComponent<Image>().sprite = AssetLibrary.GetImage("lofiEnvironment", 2, 2);
                    i++;
                    continue;
                }
                
                child.gameObject.SetActive(false);
            }
            
            while (i < Account.Characters.Count)
            {
                var characterImage = Instantiate(_characterRectPrefab, _characterGroup).GetComponent<Image>();
                characterImage.sprite = AssetLibrary.GetImage("lofiEnvironment", 3, 3);
                i++;
            }
        }
    }
}