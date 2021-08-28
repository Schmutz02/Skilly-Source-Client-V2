using Game.Entities;
using UnityEngine;

namespace Game.Overlay
{
    public class MapOverlay : MonoBehaviour
    {
        [SerializeField]
        private CharacterStatusText _characterStatusTextPrefab;

        [SerializeField]
        private HealthBar _healthBarPrefab;

        public void AddStatusText(Entity entity, string text, Color color, int lifetime, int offsetTime = 0)
        {
            var statusText = Instantiate(_characterStatusTextPrefab, transform);;
            statusText.Init(entity, text, color, lifetime, offsetTime);
            statusText.gameObject.SetActive(true);
        }

        public HealthBar GetHealthBar()
        {
            var healthBar = Instantiate(_healthBarPrefab, transform);
            return healthBar;
        }
    }
}