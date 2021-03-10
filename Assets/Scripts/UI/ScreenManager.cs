using System.Threading.Tasks;
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

        private GameObject _activeScreen;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            _activeScreen = Instantiate(_menuScreen);
        }

        public void ChangeScreen(Screen screen)
        {
            GameObject newScreenPrefab = null;
            switch (screen)
            {
                case Screen.Menu:
                    newScreenPrefab = _menuScreen;
                    break;
                case Screen.Character:
                    newScreenPrefab = _characterScreen;
                    break;
            }

            _activeScreen.SetActive(false);
            var newScreenObject = Instantiate(newScreenPrefab);
            
            //maybe do some saving?
            Destroy(_activeScreen);
            _activeScreen = newScreenObject;
        }
    }

    public enum Screen
    {
        Menu,
        Character
    }
}