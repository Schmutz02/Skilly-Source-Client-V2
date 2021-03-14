using Models;
using Models.Static;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterScreen
{
    public class CharacterRect : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        
        [SerializeField]
        private Button _frameButton;

        private int _charId;
        
        private void Awake()
        {
            _frameButton.onClick.AddListener(OnFrameClick);
        }

        public void Init(CharacterStats stats)
        {
            var desc = AssetLibrary.GetObjectDesc(stats.ClassType);
            _image.sprite = desc.TextureData.Animation.GetFrame(Direction.Right, Action.Stand, 0);

            _charId = stats.Id;
        }

        private void OnFrameClick()
        {
            Account.SetGameInitData(-1, _charId, false); // probably not a good way to do this
            ScreenManager.Instance.ChangeScreen(Screen.Game);
        }
    }
}