using System;
using UI.CharacterScreen;
using UI.DeathScreen;
using UI.GameScreen;
using UI.NewCharacterScreen;
using UI.SkinSelectScreen;
using UI.TitleScreen;
using UnityEngine;

namespace UI
{
    public class ViewManager : MonoBehaviour
    {
        public static ViewManager Instance { get; private set; }

        [SerializeField]
        private TitleScreenController _menuView;

        [SerializeField]
        private CharacterScreenController _characterView;
        
        [SerializeField]
        private NewCharacterScreenController _newCharacterView;

        [SerializeField]
        private SkinSelectScreenController _skinSelectView;
        
        [SerializeField]
        private GameScreenController _gameView;

        [SerializeField]
        private DeathScreenController _deathView;

        private UIController _activeView;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            ChangeView(View.Menu);
        }

        public void ChangeView(View view, object data = null)
        {
            UIController newView;
            switch (view)
            {
                case View.Menu:
                    newView = _menuView;
                    break;
                case View.Character:
                    newView = _characterView;
                    break;
                case View.NewCharacter:
                    newView = _newCharacterView;
                    break;
                case View.SkinSelect:
                    newView = _skinSelectView;
                    break;
                case View.Game:
                    newView = _gameView;
                    break;
                case View.Death:
                    newView = _deathView;
                    break;
                default:
                    throw new Exception($"{view} not yet implemented");
            }

            _activeView?.gameObject.SetActive(false);
            _activeView = newView;
            _activeView?.gameObject.SetActive(true);
            _activeView.Reset(data);
        }

        public void DisableCurrentView()
        {
            _activeView?.gameObject.SetActive(false);
        }
        
        public void EnableCurrentView()
        {
            _activeView?.gameObject.SetActive(true);
        }
    }

    public enum View
    {
        Menu,
        Character,
        NewCharacter,
        SkinSelect,
        Game,
        Death
    }
}