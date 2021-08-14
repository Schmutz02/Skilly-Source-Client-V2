using UnityEngine;

namespace UI.GameScreen
{
    public class GameScreenController : UIController
    {
        protected override void Reset()
        {
            Camera.main.backgroundColor = Color.black;
            Camera.main.rect = new Rect(0, 0, .75f, 1);
        }
    }
}