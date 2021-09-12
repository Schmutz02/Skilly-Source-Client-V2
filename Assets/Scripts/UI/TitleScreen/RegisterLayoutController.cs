using System.Threading.Tasks;
using Models;
using Networking.WebRequestHandlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TitleScreen
{
    public class RegisterLayoutController : UIController
    {
        [SerializeField]
        private TMP_InputField _usernameField;

        [SerializeField]
        private TMP_InputField _passwordField;

        [SerializeField]
        private TMP_InputField _reTypePasswordField;
        
        [SerializeField]
        private Toggle _rememberUsernameToggle;

        [SerializeField]
        private Button _playButton;

        [SerializeField]
        private Button _logInButton;

        [SerializeField]
        private TextMeshProUGUI _errorTextField;

        [SerializeField]
        private TitleScreenController _titleScreenController;
        
        private void Awake()
        {
            _playButton.onClick.AddListener(async () => await OnPlayButtonClick());
            _logInButton.onClick.AddListener(_titleScreenController.ShowLoginLayout);
        }
        
        public override void Reset(object data)
        {
            base.Reset(data);

            _usernameField.text = "";
            _passwordField.text = "";
            _reTypePasswordField.text = "";
            _errorTextField.text = "";
        }
        
        private async Task OnPlayButtonClick()
        {
            _playButton.enabled = false;
            _logInButton.enabled = false;
            _errorTextField.text = "";

            if (ValidUsername() && ValidPassword() && PasswordsMatch())
            {
                await SendRegisterAsync();

                if (Account.Exists)
                {
                    ViewManager.Instance.ChangeView(View.Character);
                }
            }

            _playButton.enabled = true;
            _logInButton.enabled = true;
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

        private bool PasswordsMatch()
        {
            if (_passwordField.text != _reTypePasswordField.text)
            {
                _errorTextField.text = "Passwords do not match";
                return false;
            }

            return true;
        }
        
        private async Task SendRegisterAsync()
        {
            var logInTask = new RegisterRequestHandler(
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