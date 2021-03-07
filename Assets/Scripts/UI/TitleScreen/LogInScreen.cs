using System;
using Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.TitleScreen
{
    public class LogInScreen : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _usernameField;

        [SerializeField]
        private TMP_InputField _passwordField;

        [SerializeField]
        private Toggle _rememberUsernameToggle;

        [SerializeField]
        private Button _playButton;

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonClick);
            SetUsernameField();
        }

        private void SetUsernameField()
        {
            if (PlayerPrefs.HasKey(Account.USERNAME_KEY))
            {
                _usernameField.text = PlayerPrefs.GetString(Account.USERNAME_KEY);
            }
        }

        private void OnPlayButtonClick()
        {
            //TODO input validating
            var logInTask = new LogInTask(
                _usernameField.text, 
                _passwordField.text, 
                _rememberUsernameToggle.isOn);
            
            logInTask.StartAsync();
        }
    }
}