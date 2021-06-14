using System;
using Models;
using UnityEngine;

namespace UI
{
    public class ViewManager : MonoBehaviour
    {
        public static ViewManager Instance { get; private set; }

        [SerializeField]
        private GameObject _menuView;

        [SerializeField]
        private GameObject _characterView;
        
        [SerializeField]
        private GameObject _gameView;

        private GameObject _activeView;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            ChangeView(View.Menu);
        }

        public void ChangeView(View view)
        {
            GameObject newView;
            switch (view)
            {
                case View.Menu:
                    newView = _menuView;
                    break;
                case View.Character:
                    newView = _characterView;
                    break;
                case View.Game:
                    if (_activeView != _characterView)
                    {
                        throw new Exception($"Unable to go from {_activeView.name} to {View.Game}");
                    }
                    newView = _gameView;
                    break;
                default:
                    throw new Exception($"{view} not yet implemented");
            }

            _activeView?.SetActive(false);
            _activeView = newView;
            _activeView?.SetActive(true);
        }
    }

    public enum View
    {
        Menu,
        Character,
        Game
    }
}