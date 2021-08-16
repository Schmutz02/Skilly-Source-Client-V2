using System;
using Models;
using Models.Static;
using UnityEngine;
using UnityEngine.UI;
using Action = Models.Static.Action;

namespace UI.SkinSelectScreen
{
    public class SkinCharacterBox : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private Button _frameButton;

        private SkinDesc _desc;

        private void Awake()
        {
            _frameButton.onClick.AddListener(OnFrameClick);
        }

        public void Init(SkinDesc skinDesc)
        {
            _desc = skinDesc;
            
            _image.sprite = skinDesc.Animation.ImageFromDir(Facing.Right, Action.Stand, 0);
        }

        private void OnFrameClick()
        {
            ViewManager.Instance.ChangeView(View.Game,
                new GameInitData(-1, Account.NextCharId, true, _desc.ClassType, _desc.Type));
        }
    }
}