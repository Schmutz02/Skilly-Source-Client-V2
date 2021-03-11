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

            ChangeScreen(Screen.Menu);
        }

        public void ChangeScreen(Screen screen)
        {
            GameObject newScreen = null;
            switch (screen)
            {
                case Screen.Menu:
                    newScreen = _menuScreen;
                    break;
                case Screen.Character:
                    newScreen = _characterScreen;
                    break;
            }

            _activeScreen?.SetActive(false);
            _activeScreen = newScreen;
            _activeScreen?.SetActive(true);
        }
    }

    public enum Screen
    {
        Menu,
        Character
    }
}