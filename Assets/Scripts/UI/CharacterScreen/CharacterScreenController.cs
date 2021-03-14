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
                    child.GetComponent<CharacterRect>().Init(Account.Characters[i]);
                    i++;
                    continue;
                }
                
                child.gameObject.SetActive(false);
            }
            
            while (i < Account.Characters.Count)
            {
                var characterRect = Instantiate(_characterRectPrefab, _characterGroup).GetComponent<CharacterRect>();
                characterRect.Init(Account.Characters[i]);
                i++;
            }
        }
    }
}