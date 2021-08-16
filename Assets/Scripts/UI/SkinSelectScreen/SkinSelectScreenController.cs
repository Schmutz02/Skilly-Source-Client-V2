using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SkinSelectScreen
{
    public class SkinSelectScreenController : UIController
    {
        [SerializeField]
        private RectTransform _characterGroup;

        [SerializeField]
        private SkinCharacterBox _characterBoxPrefab;

        [SerializeField]
        private Button _backButton;
        
        private List<SkinCharacterBox> _skinCharacterBoxes;
        
        private void Awake()
        {
            _skinCharacterBoxes = new List<SkinCharacterBox>();
            
            _backButton.onClick.AddListener(OnBack);
        }
        
        public override void Reset(object data)
        {
            base.Reset(data);

            var classType = (ushort) data;
            var skins = AssetLibrary.ClassType2Skins[classType];
                
            var i = 0;
            foreach (var character in _skinCharacterBoxes)
            {
                if (i < skins.Count)
                {
                    character.Init(skins[i]);
                    character.gameObject.SetActive(true);
                    i++;
                    continue;
                }
                
                character.gameObject.SetActive(false);
            }

            while (i < skins.Count)
            {
                var characterRect = Instantiate(_characterBoxPrefab, _characterGroup);
                characterRect.Init(skins[i]);
                characterRect.gameObject.SetActive(true);
                _skinCharacterBoxes.Add(characterRect);
                i++;
            }
        }
        
        private void OnBack()
        {
            ViewManager.Instance.ChangeView(View.NewCharacter);
        }
    }
}