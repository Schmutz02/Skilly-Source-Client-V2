using UnityEngine;

namespace UI
{
    public abstract class UIController : MonoBehaviour
    {
        private void OnEnable()
        {
            Reset();
        }

        protected abstract void Reset();
    }
}