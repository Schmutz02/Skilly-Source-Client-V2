using System;
using Game.Entities;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Overlay
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Scrollbar _healthBar;
        
        [SerializeField]
        private MainCameraManager _mainCamera;

        private Entity _entity;

        public void Init(Entity entity)
        {
            _entity = entity;
        }

        public void Draw()
        {
            _healthBar.size = (float) _entity.Hp / _entity.MaxHp;
            ((RectTransform) transform).sizeDelta = new Vector2(_entity.Size / 100f * _entity.SizeMult, 0.2f);

            var pos = _entity.Position;
            pos.z += 0.3f;
            transform.position = pos;
            transform.rotation = _mainCamera.Camera.transform.rotation; 
        }
    }
}