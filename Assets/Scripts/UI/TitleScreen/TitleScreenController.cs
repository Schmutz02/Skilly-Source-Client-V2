using UnityEngine;

namespace UI.TitleScreen
{
    public class TitleScreenController : UIController
    {
        [SerializeField]
        private LogInLayoutController _logInController;
        
        public override void Reset(object data)
        {
            base.Reset(data);
            
            _logInController.Reset(data);
        }
    }
}