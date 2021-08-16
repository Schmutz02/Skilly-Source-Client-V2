using UnityEngine;

namespace UI
{
    public abstract class UIController : MonoBehaviour
    {
        protected static readonly Color BlueColor = new Color(49 / 255f, 77 / 255f, 121 / 255f);

        public virtual void Reset(object data)
        {
            Camera.main.backgroundColor = BlueColor;
            Camera.main.rect = new Rect(0, 0, 1, 1);
        }
    }
}