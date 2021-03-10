using System.Threading.Tasks;
using Networking.WebRequestHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TitleScreen
{
    public class LogInController : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _usernameField;

        [SerializeField]
        private TMP_InputField _passwordField;

        [SerializeField]
        private Toggle _rememberUsernameToggle;

        [SerializeField]
        private Button _playButton;

        [SerializeField]
        private TextMeshProUGUI _errorTextField;

        private void Awake()
        {
            _playButton.onClick.AddListener(async () => await OnPlayButtonClick());
            SetUsernameField();
        }

        private void SetUsernameField()
        {
            if (PlayerPrefs.HasKey(Account.USERNAME_KEY))
            {
                _usernameField.text = PlayerPrefs.GetString(Account.USERNAME_KEY);
            }
        }

        private async Task OnPlayButtonClick()
        {
            _playButton.enabled = false;
            _errorTextField.text = "";

            if (ValidUsername() && ValidPassword())
            {
                await SendLoginAsync();

                if (Account.Exists)
                {
                    // screen manager switch -> CharacterSelect
                }
            }
                
            ScreenManager.Instance.ChangeScreen(Screen.Character);
            _playButton.enabled = true;
        }

        private bool ValidUsername()
        {
            if (_usernameField.text.Length > _usernameField.characterLimit)
            {
                _errorTextField.text = "Username too long";
                return false;
            }

            return true;
        }

        private bool ValidPassword()
        {
            if (_passwordField.text.Length < 12)
            {
                _errorTextField.text = "Password too short";
                return false;
            }
            
            return true;
        }

        private async Task SendLoginAsync()
        {
            var logInTask = new LogInRequestHandler(
                _usernameField.text, 
                _passwordField.text, 
                _rememberUsernameToggle.isOn);

            logInTask.OnError += OnLogInError;

            
            await logInTask.SendRequestAsync();
        }

        private void OnLogInError(string error)
        {
            _errorTextField.text = error;
        }
    }
}