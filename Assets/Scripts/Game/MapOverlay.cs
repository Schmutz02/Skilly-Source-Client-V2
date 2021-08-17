using System;
using System.Collections.Generic;
using Game.Entities;
using UnityEngine;

namespace Game
{
    public class MapOverlay : MonoBehaviour
    {
        [SerializeField]
        private CharacterStatusText _characterStatusTextPrefab;

        public void AddStatusText(Entity entity, string text, Color color, int lifetime, int offsetTime = 0)
        {
            var statusText = Instantiate(_characterStatusTextPrefab, transform);;
            statusText.Init(entity, text, color, lifetime, offsetTime);
            statusText.gameObject.SetActive(true);
        }
    }
}