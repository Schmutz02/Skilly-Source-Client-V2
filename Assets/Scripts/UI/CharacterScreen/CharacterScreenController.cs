using System.Collections.Generic;
using Models;
using UnityEngine;

namespace UI.CharacterScreen
{
    public class CharacterScreenController : UIController
    {
        [SerializeField]
        private RectTransform _characterGroup;

        [SerializeField]
        private CharacterRect _characterRectPrefab;

        [SerializeField]
        private NewCharacterRect _newCharacterRectPrefab;

        private List<CharacterRect> _characters;
        private List<NewCharacterRect> _newCharacterRects;

        private void Awake()
        {
            _characters = new List<CharacterRect>();
            _newCharacterRects = new List<NewCharacterRect>();
        }

        public override void Reset(object data)
        {
            base.Reset(data);
            
            var i = 0;
            foreach (var character in _characters)
            {
                if (i < Account.Characters.Count)
                {
                    character.Init(Account.Characters[i]);
                    character.gameObject.SetActive(true);
                    i++;
                    continue;
                }
                
                character.gameObject.SetActive(false);
            }

            while (i < Account.Characters.Count)
            {
                var characterRect = Instantiate(_characterRectPrefab, _characterGroup);
                characterRect.Init(Account.Characters[i]);
                characterRect.gameObject.SetActive(true);
                _characters.Add(characterRect);
                i++;
            }

            var j = Account.Characters.Count;
            foreach (var newCharacterRect in _newCharacterRects)
            {
                if (j < Account.MaxCharacters)
                {
                    newCharacterRect.gameObject.SetActive(true);
                    j++;
                    continue;
                }
                
                newCharacterRect.gameObject.SetActive(false);
            }

            while (j < Account.MaxCharacters)
            {
                var newCharacterRect = Instantiate(_newCharacterRectPrefab, _characterGroup);
                newCharacterRect.gameObject.SetActive(true);
                _newCharacterRects.Add(newCharacterRect);
                j++;
            }
        }
    }
}