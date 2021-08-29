using System;
using Game.Entities;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    public class NamePanel : MonoBehaviour
    {
        [SerializeField]
        private Image _portrait;

        [SerializeField]
        private TextMeshProUGUI _nameText;

        private Player _player;

        private void Awake()
        {
            Networking.Packets.Incoming.Update.OnMyPlayerJoined += OnMyPlayerJoined;
        }

        private void OnMyPlayerJoined(Player player)
        {
            _player = player;
        }

        private void Update()
        {
            if (_player == null)
                return;
            
            _portrait.sprite = _player.TextureProvider.GetPortrait();
            _nameText.text = Account.Username;
        }
    }
}