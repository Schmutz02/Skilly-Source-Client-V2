using UnityEngine;
using UnityEngine.UI;

namespace UI.NewCharacterScreen
{
    public class NewCharacterScreenController : UIController
    {
        [SerializeField]
        private RectTransform _characterGroup;

        [SerializeField]
        private CharacterBox _characterBoxPrefab;
        
        [SerializeField]
        private Button _backButton;

        private void Awake()
        {
            _backButton.onClick.AddListener(OnBack);
            
            foreach (var playerDesc in AssetLibrary.Type2PlayerDesc.Values)
            {
                var characterBox = Instantiate(_characterBoxPrefab, _characterGroup);
                characterBox.Init(playerDesc);
            }
        }

        private void OnBack()
        {
            ViewManager.Instance.ChangeView(View.Character);
        }
    }
}