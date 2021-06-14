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

        [SerializeField]
        private GameManager _gameManager;

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
            _gameManager.StartGame(-1, _charId, false);
        }
    }
}