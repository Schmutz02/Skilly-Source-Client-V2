using Models;
using UnityEngine;

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
            Camera.main.rect = new Rect(0, 0, 1, 1);
            
            var i = 0;
            foreach (Transform child in _characterGroup.transform)
            {
                if (i < Account.Characters.Count)
                {
                    child.GetComponent<CharacterRect>().Init(Account.Characters[i]);
                    child.gameObject.SetActive(true);
                    i++;
                    continue;
                }
                
                child.gameObject.SetActive(false);
            }
            
            while (i < Account.Characters.Count)
            {
                var characterRect = Instantiate(_characterRectPrefab, _characterGroup).GetComponent<CharacterRect>();
                characterRect.Init(Account.Characters[i]);
                characterRect.gameObject.SetActive(true);
                i++;
            }
        }
    }
}