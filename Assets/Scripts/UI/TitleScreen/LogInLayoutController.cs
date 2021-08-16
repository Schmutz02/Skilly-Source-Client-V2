using System.Threading.Tasks;
using Models;
using Networking.WebRequestHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TitleScreen
{
    public class LogInLayoutController : UIController
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
        }

        private void SetUsernameField()
        {
            if (PlayerPrefs.HasKey(Account.USERNAME_KEY))
            {
                _usernameField.text = PlayerPrefs.GetString(Account.USERNAME_KEY);
            }
            else
            {
                _usernameField.text = "";
            }
        }
        
        public override void Reset(object data)
        {
            base.Reset(data);
            
            SetUsernameField();
            _passwordField.text = "";
            _rememberUsernameToggle.isOn = PlayerPrefs.GetInt(Account.REMEMBER_USERNAME_KEY) == 1;
            _errorTextField.text = "";
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
                    ViewManager.Instance.ChangeView(View.Character);
                }
            }

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