using Game.Entities;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen.Panels
{
    public class PortalPanel : Panel
    {
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private TextMeshProUGUI _fullText;

        [SerializeField]
        private Button _enterButton;

        private Portal _parent;

        private void Awake()
        {
            _enterButton.onClick.AddListener(EnterPortal);
        }

        public void Init(Portal portal)
        {
            _parent = portal;

            if (_parent.LockedPortal)
            {
                _fullText.text = "Locked";
            }
        }

        private void EnterPortal()
        {
            //TODO use portal
            //TcpTicker.Send();
        }

        private void Update()
        {
            var name = _parent.Name ?? _parent.Desc.DisplayId;
            const string lockedString = "Locked ";
            if (name.IndexOf(lockedString) == 0)
                name = name.Remove(0, lockedString.Length);

            _nameText.text = name;
            
            var newPos = _nameText.rectTransform.anchoredPosition;
            newPos.y = _nameText.rectTransform.rect.height > 30 ? 0 : 6;
            _nameText.rectTransform.anchoredPosition = newPos;

            if (!_parent.LockedPortal && _parent.Active && _fullText.gameObject.activeSelf)
            {
                _fullText.gameObject.SetActive(false);
                _enterButton.gameObject.SetActive(true);
            }
            else if ((_parent.LockedPortal || !_parent.Active) && _enterButton.gameObject.activeSelf)
            {
                _enterButton.gameObject.SetActive(false);
                _fullText.gameObject.SetActive(true);
            }
        }
    }
}