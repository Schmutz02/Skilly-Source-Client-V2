using UnityEngine;

namespace UI.TitleScreen
{
    public class TitleScreenController : UIController
    {
        protected override void Reset()
        {
            Camera.main.backgroundColor = BlueColor;
            Camera.main.rect = new Rect(0, 0, 1, 1);
        }
    }
}