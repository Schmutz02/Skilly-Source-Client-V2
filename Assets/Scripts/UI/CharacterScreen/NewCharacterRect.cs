using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterScreen
{
    public class NewCharacterRect : MonoBehaviour
    {
        [SerializeField]
        private Button _frameButton;

        private void Awake()
        {
            _frameButton.onClick.AddListener(OnFrameClick);
        }

        private void OnFrameClick()
        {
            ViewManager.Instance.ChangeView(View.NewCharacter);
        }
    }
}