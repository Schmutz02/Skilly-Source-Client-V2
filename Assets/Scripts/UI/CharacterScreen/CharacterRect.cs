using Game;
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

        private CharacterStats _stats;
        
        private void Awake()
        {
            _frameButton.onClick.AddListener(OnFrameClick);
        }

        public void Init(CharacterStats stats)
        {
            var desc = AssetLibrary.GetPlayerDesc(stats.ClassType);
            _image.sprite = desc.TextureData.Animation.ImageFromDir(Facing.Right, Action.Stand, 0);

            _stats = stats;
        }

        private void OnFrameClick()
        {
            ViewManager.Instance.ChangeView(View.Game,
                new GameInitData(-1, _stats.Id, false, _stats.ClassType, _stats.SkinType));
        }
    }
}