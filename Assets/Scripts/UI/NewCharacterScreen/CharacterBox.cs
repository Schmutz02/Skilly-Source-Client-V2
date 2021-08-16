using System;
using Models.Static;
using UnityEngine;
using UnityEngine.UI;
using Action = Models.Static.Action;

namespace UI.NewCharacterScreen
{
    public class CharacterBox : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private Button _frameButton;

        private PlayerDesc _desc;

        private void Awake()
        {
            _frameButton.onClick.AddListener(OnFrameClick);
        }

        public void Init(PlayerDesc desc)
        {
            _desc = desc;
            _image.sprite = desc.TextureData.Animation.ImageFromDir(Facing.Down, Action.Stand, 0);
        }

        private void OnFrameClick()
        {
            ViewManager.Instance.ChangeView(View.SkinSelect, _desc.Type);
        }
    }
}