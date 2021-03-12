using UnityEngine;

namespace UI.GameScreen
{
    public class GameScreenController : UIController
    {
        protected override void Reset()
        {
            Camera.main.backgroundColor = Color.black;
        }
    }
}