using UnityEngine;

namespace UI.TitleScreen
{
    public class TitleScreenController : UIController
    {
        [SerializeField]
        private LogInLayoutController _logInController;

        [SerializeField]
        private RegisterLayoutController _registerController;
        
        public override void Reset(object data)
        {
            base.Reset(data);
            
            ShowLoginLayout();
        }

        public void ShowLoginLayout()
        {
            _registerController.gameObject.SetActive(false);
            _logInController.Reset(null);
            _logInController.gameObject.SetActive(true);
        }

        public void ShowRegisterLayout()
        {
            _logInController.gameObject.SetActive(false);
            _registerController.Reset(null);
            _registerController.gameObject.SetActive(true);
        }
    }
}