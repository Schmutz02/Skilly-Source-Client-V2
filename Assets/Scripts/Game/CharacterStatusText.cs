using System;
using Game.Entities;
using TMPro;
using UnityEngine;

namespace Game
{
    public class CharacterStatusText : MonoBehaviour
    {
        private const float _MAX_DRIFT = 1;

        [SerializeField]
        private MainCameraManager _mainCamera;
        
        [SerializeField]
        private TextMeshProUGUI _text;

        private Entity _entity;
        private float _zOffset;
        private int _lifetime;

        private int _startTime;

        public void Init(Entity entity, string text, Color color, int lifetime, int offsetTime = 0)
        {
            _entity = entity;
            _text.text = text;
            _text.color = color;
            _zOffset = entity.Desc.TextureData.Texture.rect.height / 8;
            _lifetime = lifetime;

            _startTime = GameTime.Time + offsetTime;
        }

        private void Update()
        {
            if (GameTime.Time < _startTime)
            {
                _text.enabled = false;
                return;
            }

            var aliveTime = GameTime.Time - _startTime;
            if (aliveTime > _lifetime || _entity != null && _entity.Map == null)
            {
                Destroy(gameObject);
                return;
            }

            if (_entity == null)
            {
                _text.enabled = false;
                return;
            }

            transform.rotation = _mainCamera.Camera.transform.rotation;
            _text.enabled = true;
            var newPos = (Vector3) _entity.Position;
            var drift = (float) aliveTime / _lifetime * _MAX_DRIFT;
            newPos.z -= _zOffset + drift;
            transform.position = newPos;
        }
    }
}