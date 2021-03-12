using System;
using UnityEngine;

namespace UI
{
    public class ScreenManager : MonoBehaviour
    {
        public static ScreenManager Instance { get; private set; }

        [SerializeField]
        private GameObject _menuScreen;

        [SerializeField]
        private GameObject _characterScreen;
        
        [SerializeField]
        private GameObject _gameScreen;

        private GameObject _activeScreen;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            ChangeScreen(Screen.Menu);
        }

        public void ChangeScreen(Screen screen)
        {
            GameObject newScreen;
            switch (screen)
            {
                case Screen.Menu:
                    newScreen = _menuScreen;
                    break;
                case Screen.Character:
                    newScreen = _characterScreen;
                    break;
                case Screen.Game:
                    if (_activeScreen != _characterScreen)
                    {
                        throw new Exception($"Unable to go from {_activeScreen.name} to {Screen.Game}");
                    }
                    newScreen = _gameScreen;
                    break;
                default:
                    throw new Exception($"{screen} not yet implemented");
            }

            _activeScreen?.SetActive(false);
            _activeScreen = newScreen;
            _activeScreen?.SetActive(true);
        }
    }

    public enum Screen
    {
        Menu,
        Character,
        Game
    }
}