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
            var desc = AssetLibrary.GetPlayerDesc(stats.ClassType);
            _image.sprite = desc.TextureData.Animation.GetFrame(Facing.Right, Action.Stand, 0);

            _charId = stats.Id;
        }

        private void OnFrameClick()
        {
            Account.SetGameInitData(-1, _charId, false); // TODO probably not a good way to do this
            ViewManager.Instance.ChangeView(View.Game);
        }
    }
}