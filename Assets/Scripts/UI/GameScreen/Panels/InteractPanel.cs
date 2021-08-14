using System;
using System.Collections.Generic;
using Game;
using Game.Entities;
using UnityEngine;

namespace UI.GameScreen.Panels
{
    public class InteractPanel : MonoBehaviour
    {
        [SerializeField]
        private PartyPanel _partyPanel;
        
        private Player _player;
        private Panel _currentPanel;

        private IInteractiveObject _newInteractive;
        private IInteractiveObject _closestInteractive;

        private Dictionary<Type, Panel> _panels;

        private void Awake()
        {
            _panels = new Dictionary<Type, Panel>();
            
            Networking.Packets.Incoming.Update.OnMyPlayerJoined += OnMyPlayerJoined;
            Map.UpdateInteractive += SetNewInteractive;
        }

        private void OnDisable()
        {
            Networking.Packets.Incoming.Update.OnMyPlayerJoined -= OnMyPlayerJoined;
            Map.UpdateInteractive -= SetNewInteractive;
        }

        private void OnMyPlayerJoined(Player player)
        {
            _player = player;
        }

        private void SetNewInteractive(IInteractiveObject obj)
        {
            _newInteractive = obj;
        }

        public T GetPanel<T>() where T : Panel
        {
            if (_panels.TryGetValue(typeof(T), out var panel))
                return (T)panel;
            return null;
        }

        public void SetPanel(Panel panel)
        {
            if (panel != _currentPanel)
            {
                _currentPanel?.gameObject.SetActive(false);
                _currentPanel = panel;
                _currentPanel?.gameObject.SetActive(true);

                if (_currentPanel != null && !_panels.ContainsKey(_currentPanel.GetType()))
                    _panels[_currentPanel.GetType()] = _currentPanel;
            }
        }

        private void Update()
        {
            if (_currentPanel == null || _newInteractive != _closestInteractive)
            {
                _closestInteractive = _newInteractive;
                var panel = _closestInteractive?.GetPanel(this) ?? _partyPanel;
                SetPanel(panel);
            }
        }
    }
}