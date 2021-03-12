using UnityEngine;

namespace UI
{
    public abstract class UIController : MonoBehaviour
    {
        protected static readonly Color BlueColor = new Color(49 / 255f, 77 / 255f, 121 / 255f);
        
        private void OnEnable()
        {
            Reset();
        }

        protected abstract void Reset();
    }
}