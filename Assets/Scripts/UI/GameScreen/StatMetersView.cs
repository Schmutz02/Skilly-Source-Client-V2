using System;
using Game.Entities;
using UnityEngine;
using Utils;

namespace UI.GameScreen
{
    public class StatMetersView : MonoBehaviour
    {
        [SerializeField]
        private StatusBar _expBar;

        [SerializeField]
        private StatusBar _fameBar;

        [SerializeField]
        private StatusBar _hpBar;

        [SerializeField]
        private StatusBar _mpBar;

        private Player _player;
        
        private void Awake()
        {
            Networking.Packets.Incoming.Update.OnMyPlayerJoined += OnMyPlayerJoined;

            _expBar.gameObject.SetActive(true);
            _fameBar.gameObject.SetActive(false);
        }

        private void OnMyPlayerJoined(Player player)
        {
            _player = player;
        }

        private void Update()
        {
            if (_player == null)
                return;
            
            var levelText = "Lvl " + _player.Level;
            if (levelText != _expBar.LabelText)
            {
                _expBar.LabelText = levelText;
            }

            if (_player.Level != 20)
            {
                if (!_expBar.gameObject.activeSelf)
                {
                    _expBar.gameObject.SetActive(true);
                    _fameBar.gameObject.SetActive(false);
                }
                _expBar.Draw(_player.Exp, _player.NextLevelExp, 0);
            }
            else
            {
                if (!_fameBar.gameObject.activeSelf)
                {
                    _fameBar.gameObject.SetActive(true);
                    _expBar.gameObject.SetActive(false);
                }
                _fameBar.Draw(_player.Fame, _player.NextClassQuestFame, 0);
            }
            
            _hpBar.Draw(_player.Hp, _player.MaxHp, _player.MaxHpBoost, _player.Desc.Stats[0].MaxValue);
            _mpBar.Draw(_player.Mp, _player.MaxMp, _player.MaxMpBoost, _player.Desc.Stats[1].MaxValue);
        }
    }
}
