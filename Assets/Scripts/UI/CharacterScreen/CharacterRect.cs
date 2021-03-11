using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterScreen
{
    public class CharacterRect : MonoBehaviour
    {
        [SerializeField]
        private Button _frameButton;
        
        private void Awake()
        {
            _frameButton.onClick.AddListener(() => ScreenManager.Instance.ChangeScreen(Screen.Menu));
        }
    }
}