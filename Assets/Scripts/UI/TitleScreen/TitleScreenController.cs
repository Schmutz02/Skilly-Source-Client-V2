using UnityEngine;

namespace UI.TitleScreen
{
    public class TitleScreenController : UIController
    {
        protected override void Reset()
        {
            Camera.main.backgroundColor = BlueColor;
        }
    }
}