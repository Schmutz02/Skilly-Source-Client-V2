using System;
using System.Threading.Tasks;
using Models;
using Networking.WebRequestHandlers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DeathScreen
{
    public class DeathScreenController : UIController
    {
        [SerializeField]
        private Button _continueButton;

        private void Awake()
        {
            _continueButton.onClick.AddListener(async () => await OnContinueClick());
        }

        private async Task OnContinueClick()
        {
            var charListRequest = new CharListRequestHandler(Account.Username, Account.Password);
            await charListRequest.SendRequestAsync();
            
            ViewManager.Instance.ChangeView(View.Character);
        }
    }
}